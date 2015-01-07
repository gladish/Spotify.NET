﻿using System;  
using System.Collections.Generic;  
using System.Linq;  
using System.Text;  
using System.Threading.Tasks;

namespace jukebox
{
    class Program
    {
        private static NAudio.Wave.BufferedWaveProvider _audioProvider;
        private static NAudio.Wave.WaveOut _audioSink;
        private static Spotify.Playlist _jukeboxList;
        private static Spotify.Session _session;
        private static Spotify.Track _currentTrack;
        private static string _listname;
        private static int _trackIndex;

        public static void Main(string[] args)
        {
            string username = args[0];
            string password = args[1];
            _listname = args[2];

            InitAudio();

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Spotify.Environment.UnhandledException += Environment_UnhandledException;

            Spotify.SessionConfig sessionConfig = new Spotify.SessionConfig();
            sessionConfig.ApplicationKeyFile = @"c:\Temp\spotify_appkey.key";
            sessionConfig.UserAgent = "jukebox-example";

            _session = Spotify.Session.Create(sessionConfig);
            _session.OnLoggedIn += session_OnLoggedIn;
            _session.OnMusicDelivered += session_OnMusicDelivered;
            _session.OnMetadataUpdated += session_OnMetadataUpdated;
            _session.OnPlayTokenLost += session_OnPlayTokenLost;
            _session.OnLogMessage += session_OnLogMessage;
            _session.OnEndOfTrack += session_OnEndOfTrack;
            _session.LoginAsync(new Spotify.LoginParameters() { UserName = username, Password = password }, null);
            _session.ProcessEvents();
        }

        private static void TryJukeboxStart()
        {
            if (_jukeboxList == null)
                return;

            IList<Spotify.Track> tracks = _jukeboxList.Tracks;
            if (tracks.Count == 0)
            {
                Console.WriteLine("jukebox: No more tracks in playlist. Waiting");
                return;
            }

            if (_trackIndex >= tracks.Count)
            {
                Console.WriteLine("jukebox: Not more tracks in playlist. Waiting");
                return;
            }

            Spotify.Track track = tracks[_trackIndex];

            if (_currentTrack != null && !_currentTrack.IsClone(track))
            {
                _session.PlayerUnload();
                _audioSink.Stop();
                _audioProvider.ClearBuffer();
                _currentTrack = null;
            }

            if (track.Error != Spotify.Error.Ok)
                return;

            if (_currentTrack != null && _currentTrack.IsClone(track))
                return;

            Console.WriteLine("jukebox: Now playing \"{0}\"...", track.Name);
            
            _session.PlayerLoad(track);
            _session.PlayerPlay(true);
            _audioSink.Play();
            _currentTrack = track;
        }

        private static void Environment_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {

            Console.WriteLine("Unhandled Exception");
            Console.WriteLine(e);
        }
        private static void session_OnEndOfTrack(object sender, EventArgs e)
        {
            _audioSink.Stop();
            _session.PlayerUnload();

            // TODO:  
            _trackIndex++;

            TryJukeboxStart();
        }

        private static void session_OnLogMessage(object sender, Spotify.LogMessageEventArgs e)
        {
            Console.WriteLine("LibSpotify: {0}", e.Message);
        }

        private static void session_OnPlayTokenLost(object sender, EventArgs e)
        {
            if (_currentTrack != null)
            {
                _session.PlayerUnload();
                _currentTrack = null;
            }
        }

        private static void session_OnMetadataUpdated(object sender, EventArgs e)
        {
            TryJukeboxStart();    
        }

        private static void session_OnMusicDelivered(object sender, Spotify.MusicDeliveryEventArgs e)
        {
            _audioProvider.AddSamples(e.PcmData, 0, e.PcmData.Length);
        }

        private static void session_OnLoggedIn(object sender, Spotify.LoggedInEventArgs e)
        {
            Spotify.Session session = (Spotify.Session)sender;
            if (e.ErrorCode != Spotify.Error.Ok)
            {
                Console.WriteLine("jukebox: Login failed: {0}", e.Message);
                Environment.Exit(0);
            }

            Spotify.PlaylistContainer playlistContainer = session.CreatePlaylistContainer();
            playlistContainer.OnPlaylistAdded += playlistContainer_OnPlaylistAdded;
            playlistContainer.OnPlaylistRemoved += playlistContainer_OnPlaylistRemoved;
            playlistContainer.OnLoaded += playlistContainer_OnLoaded;

            foreach (Spotify.Playlist playlist in playlistContainer.Playlists)
            {
                playlist.TracksAdded += playlist_OnTracksAdded;
                playlist.TracksRemoved += playlist_OnTracksRemoved;
                playlist.TracksMoved += playlist_OnTracksMoved;
                playlist.Renamed += playlist_OnRenamed;

                if (playlist.Name.Equals(_listname))
                {
                    _jukeboxList = playlist;

                    TryJukeboxStart();
                }
                else
                {
                    playlist.Dispose();
                }
            }

            if (_jukeboxList == null)
                Console.WriteLine("jukebox: No such playlist. Waiting for one to pop up...");
        }

        private static void playlist_OnRenamed(object sender, EventArgs e)
        {
            Spotify.Playlist playlist = (Spotify.Playlist)sender;
            if (playlist.Name.Equals(_listname))
            {
                _jukeboxList = playlist;
                _trackIndex = 0;
                TryJukeboxStart();
            }
            else
            {
                Console.WriteLine("jukebox: current playlist renamed to: {0}", playlist.Name);
                _jukeboxList = null;
                _currentTrack = null;
                _session.PlayerUnload();
            }
        }

        private static void playlist_OnTracksMoved(object sender, Spotify.PlaylistTracksMovedEventArgs e)
        {
            Spotify.Playlist playlist = (Spotify.Playlist)sender;
            if (!playlist.IsClone(_jukeboxList))
                return;

            Console.WriteLine("jukebox: {0} tracks were moved", e.Tracks.Count);
            TryJukeboxStart();
        }

        private static void playlist_OnTracksRemoved(object sender, Spotify.PlaylistTracksRemovedEventArgs e)
        {
            Spotify.Playlist playlist = (Spotify.Playlist)sender;
            if (!playlist.IsClone(_jukeboxList))
                return;
           
            int k = 0;            
            for (int i = 0; i < e.Tracks.Count; ++i)
            {
                if (e.Tracks[i] < _trackIndex)
                    ++k;
            }

            _trackIndex = k;
            TryJukeboxStart();
        }

        private static void playlist_OnTracksAdded(object sender, Spotify.PlaylistTracksAddedEventArgs e)
        {
            Spotify.Playlist playlist = (Spotify.Playlist)sender;
            if (!playlist.IsClone(_jukeboxList))
                return;

            TryJukeboxStart();
        }

        private static void playlistContainer_OnLoaded(object sender, EventArgs e)
        {
            Spotify.PlaylistContainer playlistContainer = (Spotify.PlaylistContainer)sender;
            Console.WriteLine("jukebox: Rootlist synchronized ({0} playlists)", playlistContainer.Playlists.Count);
        }

        private static void playlistContainer_OnPlaylistRemoved(object sender, Spotify.PlaylistRemovedEventArgs e)
        {
            e.Playlist.TracksAdded -= playlist_OnTracksAdded;
            e.Playlist.TracksRemoved -= playlist_OnTracksRemoved;
            e.Playlist.TracksMoved -= playlist_OnTracksMoved;
            e.Playlist.Renamed -= playlist_OnRenamed;
            
        }

        private static void playlistContainer_OnPlaylistAdded(object sender, Spotify.PlaylistAddedEventArgs e)
        {            
            e.Playlist.TracksAdded += playlist_OnTracksAdded;
            e.Playlist.TracksRemoved += playlist_OnTracksRemoved;
            e.Playlist.TracksMoved += playlist_OnTracksMoved;
            e.Playlist.Renamed += playlist_OnRenamed;

            if (e.Playlist.Name.Equals(_listname))
            {
                _jukeboxList = e.Playlist;
                TryJukeboxStart();
            }
        }

        private static void InitAudio()
        {
            _audioProvider = new NAudio.Wave.BufferedWaveProvider(new NAudio.Wave.WaveFormat(44100, 2));
            _audioProvider.BufferDuration = TimeSpan.FromSeconds(300);
            _audioSink = new NAudio.Wave.WaveOut();
            _audioSink.Init(_audioProvider);
        }
    }
}  
   
 
