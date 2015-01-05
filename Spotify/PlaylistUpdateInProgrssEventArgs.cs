using System;

namespace Spotify
{
    public class PlaylistUpdateInProgrssEventArgs : EventArgs
    {
        public PlaylistUpdateInProgrssEventArgs(bool done)
        {
            Done = done;
        }
        public readonly bool Done;
    }
}
