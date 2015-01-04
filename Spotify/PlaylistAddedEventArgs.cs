using System;

namespace Spotify
{
    public class PlaylistAddedEventArgs : EventArgs
    {
        internal PlaylistAddedEventArgs(IntPtr playlist, int position)
        {
            Playlist = new Playlist(playlist, false);
            Position = position;
        }

        public readonly Playlist Playlist;
        public readonly int Position;
    }
}
