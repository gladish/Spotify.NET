using System;
using System.Collections.Generic;

namespace Spotify
{
    public class PlaylistTracksAddedEventArgs : EventArgs
    {
        public PlaylistTracksAddedEventArgs(IList<int> tracks, int position)
        {
            Tracks = tracks;
            Position = position;
        }
        public readonly int Position;
        public readonly IList<int> Tracks;
    }
}
