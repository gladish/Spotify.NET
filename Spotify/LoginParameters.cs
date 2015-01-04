using System;

namespace Spotify
{
    public class LoginParameters
    {
        public string UserName
        {
            get;
            set;
        }

        public string Password
        {
            get;
            set;
        }

        public bool RememberMe
        {
            get;
            set;
        }

        public byte[] Blob
        {
            get;
            set;
        }
    }
}
