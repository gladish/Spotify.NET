using System;
using System.Collections.Generic;

namespace Spotify
{
    public class PlaylistTracksRemovedEventArgs : EventArgs
    {
        internal PlaylistTracksRemovedEventArgs(IList<int> tracks)
        {
            Tracks = tracks;
        }
        public readonly IList<int> Tracks;
    }
}
