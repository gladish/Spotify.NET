using System;

namespace Spotify
{
    public class ErrorEventArgs : EventArgs
    {
        internal ErrorEventArgs(Error code) 
        { 
            ErrorCode = code; 
        }
        public readonly Error ErrorCode;
    }

}
