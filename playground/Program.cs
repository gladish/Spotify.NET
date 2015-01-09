using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace playground
{
    class Program
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
            
            RunAsync(session, username, password);            

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

        private static async Task RunAsync(Spotify.Session session, string username, string password)
        {
            await session.LoginAsync(new Spotify.LoginParameters() { UserName = username, Password = password }, null);
            
            var query = new Spotify.SearchParameters();
            query.ArtistCount = 10;
            query.Query = "leonard cohen";
            query.SearchType = Spotify.SearchType.Standard;
            query.AlbumCount = 1;
          
            var search = await session.SearchAsync(query, null);
            var artistBrowse = await session.BrowseAristAsync(search.Artists[0], Spotify.ArtistBrowseType.NoTracks, null);
            var portraits = await artistBrowse.LoadPortraitsAsync(session, null);

            int id = 0;
            foreach (var p in portraits)
            {
                string file = string.Format("c:/Temp/Image{0}.jpg", id++);
                p.ToImage().Save(file);
            }

            session.Shutdown();
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
