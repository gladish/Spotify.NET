using System;

namespace Spotify
{
    public class PlaylistTrackSeenChangedEventArgs : EventArgs
    {
        internal PlaylistTrackSeenChangedEventArgs(int position, bool seen)
        {
            Seen = seen;
            Position = position;
        }
        public readonly int Position;
        public readonly bool Seen;
    }
}
