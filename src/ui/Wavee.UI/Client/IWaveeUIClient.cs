﻿using Wavee.UI.Client.Album;
using Wavee.UI.Client.Artist;
using Wavee.UI.Client.ExtendedMetadata;
using Wavee.UI.Client.Home;
using Wavee.UI.Client.Library;
using Wavee.UI.Client.Lyrics;
using Wavee.UI.Client.Playback;
using Wavee.UI.Client.Playlist;
using Wavee.UI.Client.Previews;
using Wavee.UI.Client.Search;

namespace Wavee.UI.Client;

public interface IWaveeUIClient : IDisposable
{
    IWaveeUIHomeClient Home { get; }
    IWaveeUIPlaybackClient Playback { get; }
    IWaveeUILibraryClient Library { get; }
    IWaveeUIAlbumClient Album { get; }
    IWaveeUIPreviewClient Previews { get; }
    IWaveeUIArtistClient Artist { get; }
    IWaveeUILyricsClient Lyrics { get; }
    IWaveeUIPlaylistClient Playlist { get; }
    IWaveeUIExtendedMetadataClient ExtendedMetadata { get; }
    IWaveeUISearchClient Search { get; }
}
