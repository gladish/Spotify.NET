using System;

namespace Spotify
{
    public class ConnectionErrorEventArgs : ErrorEventArgs
    {
        public ConnectionErrorEventArgs(Error code) : base(code) { }
    }
}
