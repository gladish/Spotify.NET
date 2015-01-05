using System;

namespace Spotify
{
    public class LogMessageEventArgs : EventArgs
    {
        public LogMessageEventArgs(string s) 
        { 
            Message = s; 
        }

        public readonly string Message;
    }
}
