
using Spotify.Internal;

namespace Spotify
{
    public class Exception : System.Exception 
    {
        internal Exception(Error code)
            : base(LibSpotify.ReadUtf8(LibSpotify.sp_error_message_r(code)))
        {
            ErrorCode = code;
        }

        internal Exception(Error code, string s) : base(s)
        {
            ErrorCode = code;
        }

        public Error ErrorCode
        {
            get;
            private set;
        }
    }
}

