using System;

namespace Spotify
{
    public class PlaylistTrackMessageChangedEventArgs : EventArgs
    {
        internal PlaylistTrackMessageChangedEventArgs(int position, string message)
        {
            Message = message;
            Position = position;
        }
        public readonly int Position;
        public readonly string Message;
    }
}
