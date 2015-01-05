using System;

namespace Spotify
{
    public class ScrobbleErrorEventArgs : ErrorEventArgs
    {
        public ScrobbleErrorEventArgs(Error code)
            : base(code)
        {
        }
    }
}
