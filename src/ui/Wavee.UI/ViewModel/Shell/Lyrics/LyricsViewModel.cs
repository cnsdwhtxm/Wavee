﻿using CommunityToolkit.Mvvm.ComponentModel;
using Eum.Spotify.playlist4;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using LanguageExt.UnsafeValueAccess;
using ReactiveUI;
using Wavee.UI.Client.Lyrics;
using Wavee.UI.Client.Playback;
using Wavee.UI.ViewModel.Playback;
using Timer = System.Threading.Timer;
using ReactiveUI;
using System.Reactive.Linq;
using System.Security.Cryptography;

namespace Wavee.UI.ViewModel.Shell.Lyrics;

public partial class LyricsViewModel : ObservableObject
{
    private Guid _positioncallbackId;
    [ObservableProperty] private bool _hasLyrics;
    [ObservableProperty] private bool _loading;
    [ObservableProperty] private List<LyricsLineViewModel> _lyrics;
    [ObservableProperty] private LyricsLineViewModel? _activeLyricsLine;
    private PlaybackViewModel _playbackViewModel;
    private readonly IWaveeUILyricsClient _lyricsClient;
    private readonly Action<Action> _invokeOnUiThread;
    private string? _currentTrackId;
    private IDisposable _subscription;
    private readonly PlaybackViewModel playbackViewModel;
    public LyricsViewModel(IWaveeUILyricsClient lyricsProvider, PlaybackViewModel playbackViewModel,
        Action<Action> _invokeOnUiThread)
    {
        _lyricsClient = lyricsProvider;
        this.playbackViewModel = playbackViewModel;
        _playbackViewModel = playbackViewModel;
        this._invokeOnUiThread = _invokeOnUiThread;
        //TimeSpan.FromMilliseconds(20)
    }

    public void Create()
    {
        Destroy();
        _subscription = playbackViewModel
            .CreateListener()
            .ObserveOn(RxApp.MainThreadScheduler)
            .SelectMany(async t => await OnPlaybackEvent(t))
            .Subscribe();
        _positioncallbackId = playbackViewModel.RegisterPositionCallback(10, Callback);
    }

    public void Destroy()
    {
        _subscription?.Dispose();
        _playbackViewModel.ClearPositionCallback(_positioncallbackId);
        //clear lyrics
        Lyrics = new List<LyricsLineViewModel>(0);
    }
    private void Callback(int state)
    {
        var closestLyrics = FindClosestLyricsLine(state);
        var different = closestLyrics != _activeLyricsLine;
        if (different)
        {
            Debug.WriteLine($"Callback: {state}, startat: {closestLyrics.StartsAt}, words: {closestLyrics?.Words}");
            _invokeOnUiThread(() =>
            {
                SeekToLyrics(closestLyrics);
            });
        }
    }

    public async Task<Unit> OnPlaybackEvent(WaveeUIPlaybackState state)
    {
        if (state.Metadata.IsNone || state.PlaybackState is WaveeUIPlayerState.NotPlayingAnything)
        {
            //clear
            return Unit.Default;
        }
        var metadata = state.Metadata.ValueUnsafe();
        if (!string.Equals(metadata.Id, _currentTrackId))
        {
            _currentTrackId = metadata.Id;
            var result = await TryFetchLyrics(metadata.Id, CancellationToken.None);
            if (result is null or false)
            {
                return Unit.Default;
            }
        }

        //TODO: invoke to go to accurate lyrics line
        var closestLyrics = FindClosestLyricsLine(state.Position.TotalMilliseconds);

        _invokeOnUiThread(() =>
        {
            SeekToLyrics(closestLyrics);
        });
        return Unit.Default;
    }


    public async Task<bool?> TryFetchLyrics(string actualId, CancellationToken ct = default)
    {
        Loading = true;
        try
        {
            var lines = await _lyricsClient.GetLyrics(actualId, ct);
            if (lines.Length > 0)
            {
                Lyrics = lines.Select((a, i) => new LyricsLineViewModel
                {
                    Words = a.Words,
                    IsActive = i == 0,
                    StartsAt = a.StartTimeMs,
                }).ToList();
                HasLyrics = true;

                return true;
            }
            else
            {
                HasLyrics = false;
                return false;
            }
        }
        catch (Exception x)
        {
            HasLyrics = false;
            Log.Error(x, "An error occurred while fetching lyrics.");
            return null;
        }
        finally
        {
            Loading = false;
        }
    }
    //
    // private bool ShouldChangeLyricsLine(double ms)
    // {
    //     //check if the current line is the last one
    //     if (_lyrics == null || _lyrics.Count == 0 || _activeLyricsLine == _lyrics[^1])
    //     {
    //         return false;
    //     }
    //
    //     //We advance to the next lyrics line if the current one is over with a small margin of 10ms.
    //     //get the next line and compare the start time with the current time
    //     var nextLine = _lyrics[_lyrics.IndexOf(_activeLyricsLine) + 1];
    //     //if the small margin exceeds the difference, just say yes (but only if > 0)
    //     if (nextLine.StartsAt - ms < 10)
    //     {
    //         return true;
    //     }
    //     return false;
    // }

    private LyricsLineViewModel? FindClosestLyricsLine(double ms)
    {
        LyricsLineViewModel? currentLine = null;

        for (int i = 0; i < _lyrics.Count; i++)
        {
            double endTime = (i < _lyrics.Count - 1) ? _lyrics[i + 1].StartsAt : double.MaxValue;

            if (_lyrics[i].StartsAt <= ms && endTime > ms)
            {
                currentLine = _lyrics[i];
                break;
            }
        }

        return currentLine;
    }

    private void SeekToLyrics(LyricsLineViewModel? l)
    {
        foreach (var lr in Lyrics)
        {
            lr.IsActive = l == lr;
        }

        ActiveLyricsLine = l;
    }
}