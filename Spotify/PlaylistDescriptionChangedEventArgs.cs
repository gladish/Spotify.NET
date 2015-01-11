using System;

namespace Spotify
{
    public class PlaylistDescriptionChangedEventArgs : EventArgs
    {
        internal PlaylistDescriptionChangedEventArgs(string description)
        {
            Descripion = description;
        }
        public readonly string Descripion;
    }
}
