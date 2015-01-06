using System;

namespace Spotify
{
    public class LoggedInEventArgs : ErrorEventArgs
    {
        public LoggedInEventArgs(Error code)
            : base(code)
        {
        }

        public string Message
        {
            get
            {
                return LibSpotify.ReadUtf8(LibSpotify.sp_error_message_r(ErrorCode));
            }
        }
    }
}
