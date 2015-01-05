using System;

namespace Spotify
{
    public class ErrorEventArgs : EventArgs
    {
        public ErrorEventArgs(Error code) 
        { 
            ErrorCode = code; 
        }
        public readonly Error ErrorCode;
    }

}
