using System;
using System.Collections.Generic;

namespace Spotify
{
    public class PlaylistTracksRemovedEventArgs : EventArgs
    {
        public PlaylistTracksRemovedEventArgs(IList<Track> tracks)
        {
            Tracks = tracks;
        }
        public readonly IList<Track> Tracks;
    }
}
