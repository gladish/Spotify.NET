using System;

namespace Spotify
{
    public class PlaylistUpdateInProgrssEventArgs : EventArgs
    {
        internal PlaylistUpdateInProgrssEventArgs(bool done)
        {
            Done = done;
        }
        public readonly bool Done;
    }
}
