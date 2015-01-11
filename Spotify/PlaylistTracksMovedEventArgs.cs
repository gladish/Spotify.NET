using System;
using System.Collections.Generic;

namespace Spotify
{
    public class PlaylistTracksMovedEventArgs : EventArgs
    {
        internal PlaylistTracksMovedEventArgs(IList<int> tracks, int position)
        {
            Tracks = tracks;
            NewPosition = position;
        }
        public readonly int NewPosition;
        public readonly IList<int> Tracks;
    }
}
