using System;

namespace Spotify
{
    public class MessageToUserEventArgs : EventArgs
    {
        internal MessageToUserEventArgs(string s) 
        { 
            Message = s; 
        }

        public readonly string Message;
    }
}
