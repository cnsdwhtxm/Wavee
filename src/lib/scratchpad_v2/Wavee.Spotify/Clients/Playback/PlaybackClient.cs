﻿using Eum.Spotify.connectstate;
using Eum.Spotify.context;
using Google.Protobuf;
using LanguageExt.UnsafeValueAccess;
using Spotify.Metadata;
using Wavee.Infrastructure.Traits;
using Wavee.Player;
using Wavee.Player.States;
using Wavee.Spotify.Clients.Info;
using Wavee.Spotify.Clients.Mercury;
using Wavee.Spotify.Clients.Mercury.Key;
using Wavee.Spotify.Clients.Mercury.Metadata;
using Wavee.Spotify.Configs;
using Wavee.Spotify.Id;
using Wavee.Spotify.Infrastructure.Sys;

namespace Wavee.Spotify.Clients.Playback;

internal readonly struct PlaybackClient<RT> : IPlaybackClient
    where RT : struct, HasLog<RT>, HasHttp<RT>, HasAudioOutput<RT>
{
    private static AtomHashMap<Guid, Action<SpotifyPlaybackInfo>> _onPlaybackInfo =
        LanguageExt.AtomHashMap<Guid, Action<SpotifyPlaybackInfo>>.Empty;

    private readonly Guid _mainConnectionId;

    private readonly Func<ValueTask<string>> _getBearer;

    private readonly Func<SpotifyId, ByteString, CancellationToken, Aff<RT, Either<AesKeyError, AudioKey>>>
        _fetchAudioKeyFunc;

    private readonly IMercuryClient _mercury;
    private readonly RT _runtime;
    private readonly PreferredQualityType _preferredQuality;
    private readonly bool _autoplay;

    public PlaybackClient(Guid mainConnectionId,
        Func<ValueTask<string>> getBearer,
        Func<SpotifyId, ByteString, CancellationToken, Aff<RT, Either<AesKeyError, AudioKey>>> fetchAudioKeyFunc,
        IMercuryClient mercury, RT runtime,
        Action<SpotifyPlaybackInfo> onPlaybackInfo,
        PreferredQualityType preferredQuality, bool autoplay)
    {
        _mainConnectionId = mainConnectionId;
        _getBearer = getBearer;
        _fetchAudioKeyFunc = fetchAudioKeyFunc;
        _mercury = mercury;
        _runtime = runtime;
        _preferredQuality = preferredQuality;
        _autoplay = autoplay;
        _onPlaybackInfo.AddOrUpdate(mainConnectionId, onPlaybackInfo);
    }


    public Guid Listen(Action<SpotifyPlaybackInfo> onPlaybackInfo)
    {
        var g = Guid.NewGuid();
        _onPlaybackInfo.AddOrUpdate(g, onPlaybackInfo);
        return g;
    }

    public async Task<SpotifyPlaybackInfo> PlayTrack(string uri,
        Option<PreferredQualityType> preferredQualityOverride,
        CancellationToken ct = default)
    {
        var baseInfo = new SpotifyPlaybackInfo(Option<ProvidedTrack>.None,
            None,
            uri,
            Empty,
            None, None, false, true);

        _onPlaybackInfo.Iter(x => x(baseInfo));

        //start loading track
        var trackStreamAff = await SpotifyPlayback<RT>.LoadTrack(
            SpotifyId.FromUri(uri),
            preferredQualityOverride.IfNone(_preferredQuality),
            _getBearer,
            _fetchAudioKeyFunc,
            _mercury, ct).Run(_runtime);

        var stream = trackStreamAff.ThrowIfFail();
        _onPlaybackInfo.Iter(x =>
        {
            baseInfo = baseInfo
                .EnrichFrom(stream.Metadata)
                .WithBuffering();
            x(baseInfo);
        });

        //start playing track
        IDisposable listener = default;
        bool autoplay = _autoplay;
        var mercury = _mercury;
        PlaybackClient<RT> tmpThis = this;
        listener = WaveePlayer.Instance.StateObservable.Subscribe(async state =>
        {
            switch (state)
            {
                case WaveePlayingState playing:
                    _onPlaybackInfo.Iter(x =>
                    {
                        baseInfo = baseInfo
                            .EnrichFrom(stream.Metadata)
                            .WithPosition(playing.Position)
                            .WithPaused(false);
                        x(baseInfo);
                    });
                    break;
                case WaveePausedState paused:
                    _onPlaybackInfo.Iter(x =>
                    {
                        baseInfo = baseInfo
                            .EnrichFrom(stream.Metadata)
                            .WithPosition(paused.Position)
                            .WithPaused(true);
                        x(baseInfo);
                    });
                    break;
                case WaveeEndOfTrackState eot:
                    if (!eot.GoingToNextTrackAlready)
                    {
                        //start loading next track
                        _onPlaybackInfo.Iter(x =>
                        {
                            baseInfo = baseInfo
                                .EnrichFrom(stream.Metadata)
                                .WithPosition(eot.Position)
                                .WithPaused(true);
                            x(baseInfo);
                        });

                        if (autoplay)
                        {
                            var autoPlayContext = await mercury.AutoplayQuery(uri, ct);
                            await tmpThis.PlayContext(autoPlayContext, 0, CancellationToken.None);
                        }
                    }

                    listener?.Dispose();
                    break;
            }
        });
        await WaveePlayer.Instance.Play(stream);
        return baseInfo;
    }

    public async Task<SpotifyPlaybackInfo> PlayContext(string uri, int startFrom, CancellationToken ct = default)
    {
        var baseInfo = new SpotifyPlaybackInfo(
            Option<ProvidedTrack>.None,
            None,
            uri,
            Empty,
            None, None, false, true);

        _onPlaybackInfo.Iter(x => x(baseInfo));

        var ctx = await _mercury.ContextResolve(uri, ct);
        Option<ContextTrack> track = None;
        while (track.IsNone)
        {
            if (ctx.Pages.Count == 0)
                throw new Exception("ContextResolve returned empty pages");

            //so we want to start at index 30 for example
            //but the first page only has 20 items
            //so we need to skip the first page
            int depth = 0;
            var totalTracks = ctx.Pages.Sum(x => x.Tracks.Count);
            if (startFrom < totalTracks)
            {
                foreach (var page in ctx.Pages)
                {
                    //first lets check if we have the track in any of the pages
                    //check if index is in range
                    if (startFrom < page.Tracks.Count)
                    {
                        var item = page.Tracks[startFrom - depth];
                        track = Some(item);
                        break;
                    }

                    //lets check if we we have a next page
                    if (page.HasNextPageUrl)
                    {
                        //we need to load the next page
                        var nextPage = await _mercury.Get(page.NextPageUrl, ct);

                        //TODO:
                    }

                    //we need to skip this page
                    depth += page.Tracks.Count;
                    continue;
                }
            }
            else
            {
                //lets check if we have more pages
            }

            //we need to load the next page
        }

        //start loading track
        var tr = track.ValueUnsafe();
        var trackStreamAff = await SpotifyPlayback<RT>.LoadTrack(
            tr.HasUri ? SpotifyId.FromUri(tr.Uri) : SpotifyId.FromRaw(tr.Gid.Span, AudioItemType.Track),
            _preferredQuality,
            _getBearer,
            _fetchAudioKeyFunc,
            _mercury, ct).Run(_runtime);

        var stream = trackStreamAff.ThrowIfFail();
        _onPlaybackInfo.Iter(x =>
        {
            baseInfo = baseInfo
                .EnrichFrom(stream.Metadata)
                .EnrichContext(track.ValueUnsafe(), ctx.Metadata)
                .WithBuffering();
            x(baseInfo);
        });

        //start playing track
        IDisposable listener = default;
        bool autoplay = _autoplay;
        var mercury = _mercury;
        PlaybackClient<RT> tmpThis = this;
        listener = WaveePlayer.Instance.StateObservable.Subscribe(async state =>
        {
            switch (state)
            {
                case WaveePlayingState playing:
                    _onPlaybackInfo.Iter(x =>
                    {
                        baseInfo = baseInfo
                            .WithPosition(playing.Position)
                            .WithPaused(false);
                        x(baseInfo);
                    });
                    break;
                case WaveePausedState paused:
                    _onPlaybackInfo.Iter(x =>
                    {
                        baseInfo = baseInfo
                            .WithPosition(paused.Position)
                            .WithPaused(true);
                        x(baseInfo);
                    });
                    break;
                case WaveeEndOfTrackState eot:
                    if (!eot.GoingToNextTrackAlready)
                    {
                        //start loading next track
                        _onPlaybackInfo.Iter(x =>
                        {
                            baseInfo = baseInfo
                                .WithPosition(eot.Position)
                                .WithPaused(true);
                            x(baseInfo);
                        });

                        if (autoplay)
                        {
                            var autoPlayContext = await mercury.AutoplayQuery(uri, ct);
                            await tmpThis.PlayContext(autoPlayContext, 0, CancellationToken.None);
                        }
                    }

                    listener?.Dispose();
                    break;
            }
        });
        await WaveePlayer.Instance.Play(stream);
        return baseInfo;

        return baseInfo;
    }

    public async Task<bool> Pause(CancellationToken ct = default)
    {
        return await WaveePlayer.Instance.Pause();
    }

    public async Task<bool> Seek(TimeSpan to, CancellationToken ct = default)
    {
        return await WaveePlayer.Instance.Seek(to);
    }
}

public readonly record struct SpotifyPlaybackInfo(
    Option<ProvidedTrack> Track,
    Option<int> Duration,
    Option<string> ContextUri,
    HashMap<string, string> ContextMetadata,
    Option<string> PlaybackId,
    Option<TimeSpan> Position,
    bool Paused,
    bool Buffering)
{
    public SpotifyPlaybackInfo WithPosition(TimeSpan playingPosition)
    {
        return this with
        {
            Position = playingPosition
        };
    }

    public SpotifyPlaybackInfo WithPaused(bool b)
    {
        return this with
        {
            Paused = b,
            Buffering = false
        };
    }

    public SpotifyPlaybackInfo WithBuffering()
    {
        return this with
        {
            Buffering = true,
        };
    }

    public SpotifyPlaybackInfo EnrichContext(ContextTrack ctxTrack, HashMap<string, string> ctxMetadata)
    {
        var track = Track.IfNone(new ProvidedTrack());
        track.Provider = "context";

        return this with
        {
            Track = track,
            ContextMetadata = ctxMetadata
        };
    }

    public SpotifyPlaybackInfo EnrichFrom(TrackOrEpisode streamMetadata)
    {
        var track = new ProvidedTrack();

        return this with
        {
            Track = streamMetadata
                .Value.Match(
                    Left: e => EnrichFromEpisode(e, track),
                    Right: t => EnrichFromTrack(t, track)
                ),
            Duration = streamMetadata.Duration
        };
    }

    private static ProvidedTrack EnrichFromTrack(Track track, ProvidedTrack providedTrack)
    {
        if (track.HasPopularity)
            providedTrack.Metadata["popularity"] = track.Popularity.ToString();
        if (track.HasExplicit)
            providedTrack.Metadata["explicit"] = track.Explicit.ToString();
        if (track.HasHasLyrics)
            providedTrack.Metadata["has_lyrics"] = track.HasLyrics.ToString();
        if (track.HasName)
            providedTrack.Metadata["title"] = track.Name;
        if (track.HasDiscNumber)
            providedTrack.Metadata["album_disc_number"] = track.DiscNumber.ToString();

        for (int i = 0; i < track.Artist.Count; i++)
        {
            var artist = track.Artist[i];
            if (artist.HasName)
            {
                providedTrack.Metadata["atist_name" + (i == 0 ? string.Empty : $":{i}")] = artist.Name;
            }

            if (artist.HasGid)
            {
                providedTrack.Metadata["artist_uri" + (i == 0 ? string.Empty : $":{i}")] =
                    SpotifyId.FromRaw(artist.Gid.Span, AudioItemType.Artist).ToUri();
            }
        }

        var album = track.Album;
        if (album.Disc.Count > 0)
        {
            providedTrack.Metadata["album_track_count"] = album.Disc.Sum(x => x.Track.Count).ToString();
            providedTrack.Metadata["album_disc_count"] = album.Disc.Count.ToString();
        }

        if (album.HasName)
            providedTrack.Metadata["album_title"] = album.Name;
        if (album.HasGid)
            providedTrack.Metadata["album_uri"] = SpotifyId.FromRaw(album.Gid.Span, AudioItemType.Album).ToUri();

        for (int i = 0; i < album.Artist.Count; i++)
        {
            var artist = album.Artist[i];
            if (artist.HasName)
            {
                providedTrack.Metadata["album_atist_name" + (i == 0 ? string.Empty : $":{i}")] = artist.Name;
            }

            if (artist.HasGid)
            {
                providedTrack.Metadata["album_artist_uri" + (i == 0 ? string.Empty : $":{i}")] =
                    SpotifyId.FromRaw(artist.Gid.Span, AudioItemType.Artist).ToUri();
            }
        }

        if (track.HasDiscNumber)
        {
            foreach (var disc in album.Disc)
            {
                if (disc.Number != track.DiscNumber) continue;

                for (int i = 0; i < disc.Track.Count; i++)
                {
                    if (disc.Track[i].Gid.Span.SequenceEqual(track.Gid.Span))
                    {
                        providedTrack.Metadata["album_track_number"] = (i + 1).ToString();
                        break;
                    }
                }
            }
        }

        if (album.CoverGroup is not null)
        {
            ImageId.PutAsMetadata(providedTrack, album.CoverGroup);
        }

        providedTrack.Uri = SpotifyId.FromRaw(track.Gid.Span, AudioItemType.Track).ToUri();
        return providedTrack;
    }

    private static ProvidedTrack EnrichFromEpisode(Episode episode, ProvidedTrack track)
    {
        throw new NotImplementedException();
    }
}