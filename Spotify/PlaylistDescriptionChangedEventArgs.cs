using System;

namespace Spotify
{
    public class PlaylistDescriptionChangedEventArgs : EventArgs
    {
        public PlaylistDescriptionChangedEventArgs(string description)
        {
            Descripion = description;
        }
        public readonly string Descripion;
    }
}
