using System;


namespace Spotify.Internal
{
    internal static class EventDispatcher
    {
        public static void Dispatch(object sender, IntPtr domainObject, EventHandler handler, EventArgs e)
        {
            if (handler != null)
            {
                try
                {
                    handler(sender, e);
                }
                catch (Exception err)
                {
                    Environment.RaiseUnhandledException(sender, err, false);
                }
            }
        }

        public static void Dispatch<TEventArgs>(object sender, IntPtr session, EventHandler<TEventArgs> handler, TEventArgs e)
        {
            if (handler != null)
            {
                try
                {
                    handler(sender, e);
                }
                catch (Exception err)
                {
                    Environment.RaiseUnhandledException(sender, err, false);
                }
            }
        }
    }
}
