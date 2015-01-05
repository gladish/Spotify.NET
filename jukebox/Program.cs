using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace jukebox
{
    class Program
    {
        private static byte[] ApplicationKey = 
        {
            // Add application key here, or use File in the SessionConfig
        };

        static void Main(string[] args)
        {
            Program p = new Program(args);
            p.Run();
        }

        private NAudio.Wave.BufferedWaveProvider _audioProvider = new NAudio.Wave.BufferedWaveProvider(
            new NAudio.Wave.WaveFormat(44100, 2));

        private NAudio.Wave.WaveOut _audioSink;
        private string _user;
        private string _password;

        public Program(string[] args)
        {
            _audioProvider.BufferDuration = TimeSpan.FromSeconds(300);
            _audioSink = new NAudio.Wave.WaveOut();
            _audioSink.Init(_audioProvider);
            _user = args[0];
            _password = args[1];
        }

        private void Run()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("BuildId: {0}", Spotify.Environment.BuildId);
            Spotify.Session session = Spotify.Session.Create(new Spotify.SessionConfig()
            {
                TraceFile = Path.Combine(Path.GetTempPath(), "SpotifyTrace.txt"),
                ApplicationKeyFile = @"c:\Temp\spotify_appkey.key",
                UserAgent = "Spotify.NET"
            });

            session.OnMessageToUser += OnMessageToUser;
            session.OnMusicDelivered += OnMusicDelivered;
            session.Start();

            session.LoginAsync(new Spotify.LoginParameters()
                {
                    UserName = _user,
                    Password = _password,
                    RememberMe = false,
                    Blob = null
                }, null).Wait();


            using (Spotify.PlaylistContainer playlistContainer = session.PlaylistContainer)
            {
                foreach (Spotify.Playlist playList in playlistContainer.Playlists)
                {
                    Console.WriteLine("PlayList: {0}", playList.Name);
                    foreach (Spotify.Track track in playList.Tracks)
                        Spotify.Diagnostics.Debug.Print(Console.Out, track);

                    playList.Dispose();
                }
            }
            

            session.Dispose();
          
        }

        private void DoSearch(Spotify.Session session)
        {
            Spotify.SearchParameters searchParams = new Spotify.SearchParameters()
            {
                AlbumCount = 10,
                TrackCount = 3,
                Query = "Leonard Cohen"
            };

            bool done = false;

            Spotify.Track anyTrack = null;

            while (!done)
            {
                var task = session.SearchAsync(searchParams, null);
                task.Wait();

                Spotify.Search search = task.Result;
                task.Dispose();

                foreach (Spotify.Album album in search.Albums)
                {
                    Console.WriteLine("Album: " + album.Name);
                    album.Dispose();
                }

                if (anyTrack == null)
                    anyTrack = search.Tracks[0];

                if (search.NumAlbums == 0)
                    done = true;

                searchParams = search.Page(searchParams);
                search.Dispose();
            }

            Console.Write("\t");
            Spotify.Diagnostics.Debug.Print(Console.Out, anyTrack);

            session.PlayerLoad(anyTrack);
            session.PlayerPlay(true);
            _audioSink.Play();


            // just wait here
            System.Threading.Thread.Sleep(TimeSpan.FromHours(2));
        }

        private void OnMessageToUser(object sender, Spotify.MessageToUserEventArgs e)
        {
            Console.WriteLine("message: " + e.Message);
        }

        private int n = 0;
        private long totalQueued = 0;
        private void OnMusicDelivered(object sender, Spotify.MusicDeliveryEventArgs e)
        {
            totalQueued += e.PcmData.Length;

            if  (n++ % 20 == 0)
            {
                Console.WriteLine("Total: {0:n0} Buffered: {1:n0}", totalQueued, _audioProvider.BufferedBytes);
            }
            _audioProvider.AddSamples(e.PcmData, 0, e.PcmData.Length);
        }
    }
}
