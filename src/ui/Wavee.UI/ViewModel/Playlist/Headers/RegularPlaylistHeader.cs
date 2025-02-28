﻿using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Eum.Spotify.playlist4;
using LanguageExt;

namespace Wavee.UI.ViewModel.Playlist.Headers;

public sealed class RegularPlaylistHeader : ObservableObject, IPlaylistHeader
{
    private bool _mozaicCreated;
    private FutereTracksHolder _futureTracks;

    public RegularPlaylistHeader(
        string id,
        SelectedListContent listContent, ObservableStringHolder tracksDurationStirng, ObservableStringHolder tracksCountString, AsyncRelayCommand<string> saveCommand)
    {
        Id = id;
        TracksDurationStirng = tracksDurationStirng;
        TracksCountString = tracksCountString;
        SaveCommand = saveCommand;
        ImageUrl = listContent.Attributes.PictureSize.SingleOrDefault(x => x.TargetName is "large")?.Url
                   ?? listContent.Attributes.PictureSize.SingleOrDefault(x => x.TargetName is "default")?.Url;
        ShouldShowMozaic = string.IsNullOrEmpty(ImageUrl);

        Name = listContent.Attributes.Name;
        Description = listContent.Attributes.Description;
        IsCollab = listContent.Attributes.Collaborative;
        var shouldShowMadeFor = listContent.Attributes.FormatAttributes
                                    .Any(x => x.Key is "madeFor.displayed")
                                && listContent.Attributes.FormatAttributes.Single(x => x.Key is "madeFor.displayed")
                                    .Value is "1";

        var madeForUsername = shouldShowMadeFor
            ? listContent.Attributes.FormatAttributes.Single(x => x.Key is "madeFor.username").Value
            : null;

        ShouldShowMadeFor = shouldShowMadeFor;
        MadeForUsername = madeForUsername;

        Owner = listContent.OwnerUsername;
    }
    public string Owner { get; }
    public string Name { get; }
    public string Description { get; }

    public string? ImageUrl { get; }
    public bool ShouldShowMozaic { get; }

    public string? MadeForUsername { get; }
    public bool ShouldShowMadeFor { get; }

    public bool IsCollab { get; }

    public FutereTracksHolder FutureTracks
    {
        get => _futureTracks;
        set => SetProperty(ref _futureTracks, value);
    }

    public bool MozaicCreated
    {
        get => _mozaicCreated;
        set => SetProperty(ref _mozaicCreated, value);
    }

    public bool Negate(bool b)
    {
        return !b;
    }

    public string HtmlEscape(string s)
    {
        var doc = new HtmlAgilityPack.HtmlDocument();
        doc.LoadHtml(s);

        return doc.DocumentNode.InnerText;
    }

    public ObservableStringHolder TracksDurationStirng { get; }
    public ObservableStringHolder TracksCountString { get; }
    public bool HasDescription => !string.IsNullOrEmpty(Description);
    public AsyncRelayCommand<string> SaveCommand { get; }
    public string Id { get; }

    public void MozaicImageControl_OnImageLoadedChanged(object? sender, bool e)
    {
        MozaicCreated = e;
    }
}

public class FutereTracksHolder
{
    public FutereTracksHolder(TaskCompletionSource<Seq<Either<WaveeUIEpisode, WaveeUITrack>>> Trakcs)
    {
        this.Trakcs = Trakcs;
    }

    public TaskCompletionSource<Seq<Either<WaveeUIEpisode, WaveeUITrack>>> Trakcs { get; init; }

    public void Deconstruct(out TaskCompletionSource<Seq<Either<WaveeUIEpisode, WaveeUITrack>>> Trakcs)
    {
        Trakcs = this.Trakcs;
    }
}

public sealed class ObservableStringHolder : ObservableObject
{
    private string? _value;

    public string Value
    {
        get => _value ?? string.Empty;
        set => SetProperty(ref _value, value);
    }
}