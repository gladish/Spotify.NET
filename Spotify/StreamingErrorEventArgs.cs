using System;

namespace Spotify
{
    public class StreamingErrorEventArgs : ErrorEventArgs
    {
        public StreamingErrorEventArgs(Error code) : base(code) 
        { 
        }
    }
}
