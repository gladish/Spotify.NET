using System;
using System.Collections.Generic;

namespace Spotify
{
    public class PlaylistTracksMovedEventArgs : EventArgs
    {
        public PlaylistTracksMovedEventArgs(IList<int> tracks, int position)
        {
            Tracks = tracks;
            NewPosition = position;
        }
        public readonly int NewPosition;
        public readonly IList<int> Tracks;
    }
}
