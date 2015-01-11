using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace playground
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            string username = args[0];
            string password = args[1];

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Spotify.Environment.UnhandledException += Environment_UnhandledException;

            var sessionConfig = new Spotify.SessionConfig();
            sessionConfig.ApplicationKeyFile = @"c:\Temp\spotify_appkey.key";
            sessionConfig.UserAgent = "playground";            

            var session = Spotify.Session.Create(sessionConfig);
            session.LoggedIn += session_LoggedIn;
            session.MessageToUser += session_MessageToUser;
            session.LogMessage += session_LogMessage;

            GetArtistProtraits(session, username, password)
                .ContinueWith((continuation) => { session.Shutdown(); });

            Console.WriteLine("Processing Events");
            session.ProcessEvents();
        }

        private static void session_LogMessage(object sender, Spotify.LogMessageEventArgs e)
        {
            Console.WriteLine("LibSpotify: {0}", e.Message);
        }

        private static void session_MessageToUser(object sender, Spotify.MessageToUserEventArgs e)
        {
            Console.WriteLine("Message: {0}", e.Message);
        }

        private static void session_LoggedIn(object sender, Spotify.LoggedInEventArgs e)
        {
            Console.WriteLine("Logged In: {0}: {1}", e.ErrorCode, e.Message);
        }

        private static void Environment_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine("Unhandled Excetion");
            Console.WriteLine(e.ExceptionObject);
        }
    }
}
