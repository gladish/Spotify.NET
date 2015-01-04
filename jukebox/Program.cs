using System;
using System.IO;
using System.Threading.Tasks;

namespace jukebox
{
    class Program
    {
        private static byte[] ApplicationKey = 
        {
            // Add application key here
        };

        static void Main(string[] args)
        {
            Program p = new Program();
            p.Run();
        }

        private NAudio.Wave.BufferedWaveProvider _audioProvider = new NAudio.Wave.BufferedWaveProvider(
            new NAudio.Wave.WaveFormat(44100, 2));

        private NAudio.Wave.WaveOut _audioSink;

        public Program()
        {
            _audioProvider.BufferDuration = TimeSpan.FromSeconds(300);
            _audioSink = new NAudio.Wave.WaveOut();
            _audioSink.Init(_audioProvider);
        }

        private void Run()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("BuildId: {0}", Spotify.Environment.BuildId);
            Spotify.Session session = Spotify.Session.Create(new Spotify.SessionConfig()
            {
                TraceFile = Path.Combine(Path.GetTempPath(), "SpotifyTrace.txt"),
                ApplicationKey = ApplicationKey,
                UserAgent = "Spotify.NET"
            });

            session.OnMessageToUser += OnMessageToUser;
            session.OnMusicDelivered += OnMusicDelivered;
            session.Start();

            session.LoginAsync(new Spotify.LoginParameters()
                {
                    UserName = "<your username>",
                    Password = "<your password>",
                    RememberMe = false,
                    Blob = null
                }, null).Wait();


            Spotify.SearchParameters searchParams = new Spotify.SearchParameters()
            {
                AlbumCount = 10,
                Query = "Leonard Cohen"
            };

            bool done = false;
            while (!done)
            {
                var task = session.SearchAsync(searchParams, null);
                task.Wait();

                Spotify.Search search = task.Result;
                task.Dispose();

                foreach (Spotify.Album album in search.Albums)
                {
                    Console.WriteLine("Album: " + album.Name);
                    //album.Dispose();
                }

                if (search.NumAlbums == 0)
                    done = true;

                searchParams = search.Page(searchParams);
                search.Dispose();
            }

            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2));


            session.Dispose();

            //session.PlayerLoad(track);
            //session.PlayerPlay(true);
            //_audioSink.Play();
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
