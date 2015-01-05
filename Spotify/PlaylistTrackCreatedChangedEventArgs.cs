using System;

namespace Spotify
{
    public class PlaylistTrackCreatedChangedEventArgs : EventArgs
    {
        public PlaylistTrackCreatedChangedEventArgs(int position, User user, DateTime when)
        {
            Position = position;
            User = user;
            When = when;
        }

        public readonly int Position;
        public readonly User User;
        public readonly DateTime When;
    }
}
