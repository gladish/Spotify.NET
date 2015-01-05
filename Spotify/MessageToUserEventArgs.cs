using System;

namespace Spotify
{
    public class MessageToUserEventArgs : EventArgs
    {
        public MessageToUserEventArgs(string s) 
        { 
            Message = s; 
        }

        public readonly string Message;
    }
}
