using System;

namespace Spotify
{
    public class OfflineErrorEventArgs : ErrorEventArgs
    {
        public OfflineErrorEventArgs(Error code) : base(code) 
        { 
        }
    }
}
