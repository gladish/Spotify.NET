using System;

namespace Spotify
{
    public class LogMessageEventArgs : EventArgs
    {
        internal LogMessageEventArgs(string s) 
        { 
            Message = s; 
        }

        public readonly string Message;
    }
}
