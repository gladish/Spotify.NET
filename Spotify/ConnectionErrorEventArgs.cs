using System;

namespace Spotify
{
    public class ConnectionErrorEventArgs : ErrorEventArgs
    {
        internal ConnectionErrorEventArgs(Error code)
            : base(code)
        {
        }
    }
}
