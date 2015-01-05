using System;

namespace Spotify
{
    public class PlaylistTrackMessageChangedEventArgs : EventArgs
    {
        public PlaylistTrackMessageChangedEventArgs(int position, string message)
        {
            Message = message;
            Position = position;
        }
        public readonly int Position;
        public readonly string Message;
    }
}
