using System;
using System.Collections.Generic;

namespace Spotify
{
    public class PlaylistTracksRemovedEventArgs : EventArgs
    {
        public PlaylistTracksRemovedEventArgs(IList<int> tracks)
        {
            Tracks = tracks;
        }
        public readonly IList<int> Tracks;
    }
}
