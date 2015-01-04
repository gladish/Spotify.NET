using System;

namespace Spotify
{
    public class PlaylistMovedEventArgs : EventArgs
    {
        internal PlaylistMovedEventArgs(IntPtr playlist, int position, int newPosition)
        {
            Playlist = new Playlist(playlist, false);
            Position = position;
            NewPosition = newPosition;
        }

        public readonly Playlist Playlist;
        public readonly int Position;
        public readonly int NewPosition;
    }
}
