using System;
using Spotify.Internal;

namespace Spotify
{
    public static class Environment
    {
        public static event UnhandledExceptionEventHandler UnhandledException;

        static Environment()
        {
            SpotifyOpenURL = "http://open.spotify.com";
            DefaultSearchSizePageSize = 64;
        }

        public static string BuildId
        {
            get
            {
                return LibSpotify.ReadUtf8(LibSpotify.sp_build_id_r());
            }
        }

        public static string SpotifyOpenURL
        {
            get;
            set;
        }

        public static int DefaultSearchSizePageSize
        {
            get;
            set;
        }

        internal static void RaiseUnhandledException(object sender, Exception err, bool isTerminating)
        {
            UnhandledExceptionEventHandler handler = UnhandledException;
            if (handler != null)
            {
                try
                {
                    handler(sender, new UnhandledExceptionEventArgs(err, isTerminating));
                }
                catch
                {
                    // IGNORED???
                }
            }
        }
    }
}
