using System;

namespace Spotify.Internal
{
    internal static class ThrowHelper
    {
        public static void ThrowIfError(Error e)
        {
            if (e != Error.Ok)
                throw new Spotify.Exception(e);
        }

        public static void ThrowIfError(Error e, string message)
        {
            if (e != Error.Ok)
            {
                string s = string.Format("{0}. {0}", message, LibSpotify.sp_error_message(e));
                throw new Spotify.Exception(e, s);
            }
        }

        public static void ThrowIfNull(object obj, string name)
        {
            if (obj == null)
                throw new ArgumentNullException(name);
        }

        public static void ThrowIfZero(IntPtr p, string name)
        {
            if (p == IntPtr.Zero)
                throw new ArgumentOutOfRangeException(name);
        }

        public static T DownCast<T>(object obj, string name)
        {
            if (obj == null)
                throw new ArgumentNullException(name);
            return (T)obj;
        }

        public static void AssertLinkConverstion(LinkType current, LinkType requested)
        {
            if (!IsConvertible(current, requested))
            {
                string message = string.Format("can't convert {0} to {1}", current, requested);
                throw new InvalidOperationException(message);
            }
        }

        private static bool IsConvertible(LinkType a, LinkType b)
        {
            return a == b;
        }
    }
}
