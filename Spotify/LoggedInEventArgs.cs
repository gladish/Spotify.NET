using System;

namespace Spotify
{
    public class LoggedInEventArgs : ErrorEventArgs
    {
        public LoggedInEventArgs(Error code)
            : base(code)
        {
        }
    }
}
