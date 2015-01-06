using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

using Spotify.Internal;

namespace Spotify
{
    public partial class Session : IDisposable
    {
        private Session()
        {
            OnNotifyMainThread += HandleOnNotifyMainThread;
        }

        public event EventHandler OnNotifyMainThread;
        public event EventHandler<LoggedInEventArgs> OnLoggedIn;
        public event EventHandler OnLoggedOut;
        public event EventHandler OnMetadataUpdated;
        public event EventHandler<ConnectionErrorEventArgs> OnConnectionError;
        public event EventHandler<MessageToUserEventArgs> OnMessageToUser;
        public event EventHandler<LogMessageEventArgs> OnLogMessage;
        public event EventHandler OnEndOfTrack;
        public event EventHandler<StreamingErrorEventArgs> OnStreamingError;
        public event EventHandler OnUserInfoUpdated;
        public event EventHandler OnStartPlayback;
        public event EventHandler OnStopPlayback;
        public event EventHandler OnOfflineStatusUpdated;
        public event EventHandler<OfflineErrorEventArgs> OnOfflineError;
        public event EventHandler<CredentialsBlobUpdatedEventArgs> OnCredentialsBlobUpdated;
        public event EventHandler OnConnectionStateUpdated;
        public event EventHandler<ScrobbleErrorEventArgs> OnScrobbleError;
        public event EventHandler<PrivateSessonModeChangedEventArgs> OnPrivateSessionModeChanged;
        public event EventHandler<MusicDeliveryEventArgs> OnMusicDelivered;
        public event EventHandler OnPlayTokenLost;

        #region Properties
        public string RememberedUser
        {
            get
            {
                return LibSpotify.ReadUtf8(Handle, LibSpotify.sp_session_remembered_user_r);
            }
        }

        public string UserName
        {
            get
            {
                return LibSpotify.ReadUtf8(LibSpotify.sp_session_user_name_r(Handle));
            }
        }

        public User User
        {
            get
            {
                return new User(LibSpotify.sp_session_user_r(Handle), false);
            }
        }

        public ConnectionState State
        {
            get
            {
                return LibSpotify.sp_session_connectionstate_r(Handle);
            }
        }

        public int MaximumCacheSize
        {
            set
            {
                ThrowHelper.ThrowIfError(LibSpotify.sp_session_set_cache_size_r(Handle, new IntPtr(value)));
            }
        }

        public PlaylistContainer PlaylistContainer
        {
            get
            {
                IntPtr p = LibSpotify.sp_session_playlistcontainer_r(Handle);
                if (p == IntPtr.Zero)
                    return null;
                return new PlaylistContainer(p, false);
            }
        }

        public bool IsVolumeNormalization
        {
            get
            {
                return LibSpotify.sp_session_get_volume_normalization_r(Handle);
            }
            set
            {
                ThrowHelper.ThrowIfError(LibSpotify.sp_session_set_volume_normalization_r(Handle, value));
            }
        }

        public bool IsPrivateSession
        {
            get
            {
                return LibSpotify.sp_session_is_private_session_r(Handle);
            }
            set
            {
                ThrowHelper.ThrowIfError(LibSpotify.sp_session_set_private_session_r(Handle, value));
            }
        }

        public bool IsScrobblingPossible(SocialProvider provider)
        {
            bool possible = false;
            ThrowHelper.ThrowIfError(LibSpotify.sp_session_is_scrobbling_possible_r(Handle, provider, ref possible));
            return possible;
        }

        public ConnectionType ConnectionType
        {
            set
            {
                ThrowHelper.ThrowIfError(LibSpotify.sp_session_set_connection_type_r(Handle, value));
            }
        }

        public ConnectionRules ConnectionRules
        {
            set
            {
                ThrowHelper.ThrowIfError(LibSpotify.sp_session_set_connection_rules_r(Handle, value));
            }
        }

        public int NumberOfOfflineTracksToSync
        {
            get
            {
                return LibSpotify.sp_offline_tracks_to_sync_r(Handle);
            }
        }


        public int NumberOfOfflinePlaylists
        {
            get
            {
                return LibSpotify.sp_offline_num_playlists_r(Handle);
            }
        }

        public TimeSpan OfflineTimeLeft
        {
            get
            {
                int i = LibSpotify.sp_offline_time_left_r(Handle);
                return TimeSpan.FromSeconds(i);
            }
        }

        public System.Globalization.RegionInfo UserCountry
        {
            get
            {
                int i = LibSpotify.sp_session_user_country_r(Handle);
               
                StringBuilder builder = new StringBuilder();
                builder.Append(Convert.ToChar((i >> 8)));
                builder.Append(Convert.ToChar((i & 0xff)));
            
                return new System.Globalization.RegionInfo(builder.ToString());
            }
        }

        #endregion

        public void Dispose()
        {
            Stop();
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Start()
        {            
            _thread = new Thread(ProcessEvents);
            _thread.Name = "Spotify.NET Event";
            _thread.Start();
        }

        public void ReLogin()
        {
            ThrowHelper.ThrowIfError(LibSpotify.sp_session_relogin_r(Handle));
        }

        public void ForgetMe()
        {
            ThrowHelper.ThrowIfError(LibSpotify.sp_session_forget_me_r(Handle));
        }

        public void Logout()
        {
            ThrowHelper.ThrowIfError(LibSpotify.sp_session_logout_r(Handle));
        }

        public void FlushCaches()
        {
            ThrowHelper.ThrowIfError(LibSpotify.sp_session_flush_caches_r(Handle));
        }

        public void PlayerLoad(Track track)
        {
            ThrowHelper.ThrowIfNull(track, "track");
            ThrowHelper.ThrowIfError(LibSpotify.sp_session_player_load_r(Handle, track.Handle));
        }

        public void PlayerSeek(int offset)
        {
            ThrowHelper.ThrowIfError(LibSpotify.sp_session_player_seek_r(Handle, offset));
        }

        public void PlayerPlay(bool play)
        {
            ThrowHelper.ThrowIfError(LibSpotify.sp_session_player_play_r(Handle, play));
        }

        public void PlayerUnload()
        {
            ThrowHelper.ThrowIfError(LibSpotify.sp_session_player_unload_r(Handle));
        }

        public void PlayerPrefetch(Track track)
        {
            ThrowHelper.ThrowIfNull(track, "track");
            ThrowHelper.ThrowIfError(LibSpotify.sp_session_player_prefetch_r(Handle, track.Handle));
        }

        public Playlist CreateInbox()
        {
            return new Playlist(LibSpotify.sp_session_inbox_create_r(Handle));
        }

        public Playlist CreateStarred()
        {
            return new Playlist(LibSpotify.sp_session_starred_create_r(Handle));
        }

        public Playlist CreateStarredForUser(string canonicalUser)
        {
            return new Playlist(LibSpotify.sp_session_starred_for_user_create_r(Handle,
                canonicalUser));
        }

        public PlaylistContainer CreatePlaylistContainer()
        {
            return new PlaylistContainer(LibSpotify.sp_session_playlistcontainer_r(Handle), false);
        }

        public PlaylistContainer CreatePublishedContainerForUser(string canonicalUser)
        {
            return new PlaylistContainer(LibSpotify.sp_session_publishedcontainer_for_user_create_r(
                Handle, canonicalUser));
        }

        public void SetPreferredBitrate(BitRate bitRate)
        {
            ThrowHelper.ThrowIfError(LibSpotify.sp_session_preferred_bitrate_r(Handle, bitRate));
        }

        public void SetPreferredOfflineBitrate(BitRate bitRate, bool allowResync)
        {
            ThrowHelper.ThrowIfError(LibSpotify.sp_session_preferred_offline_bitrate_r(Handle, bitRate, allowResync));
        }

        public void SetScrobbling(SocialProvider provider, ScrobblingState state)
        {
            ThrowHelper.ThrowIfError(LibSpotify.sp_session_set_scrobbling_r(Handle, provider, state));
        }

        public ScrobblingState GetScrobbling(SocialProvider provider)
        {
            ScrobblingState state = ScrobblingState.GlobalDisabled;
            ThrowHelper.ThrowIfError(LibSpotify.sp_session_is_scrobbling_r(Handle, provider, ref state));
            return state;
        }    

        public void SetSocialCredentials(SocialProvider provider, string username, string password)
        {
            ThrowHelper.ThrowIfError(LibSpotify.sp_session_set_social_credentials_r(Handle, provider, 
                username, password));
        }

        public OfflineSyncStatus GetOfflineSyncStatus()
        {
            OfflineSyncStatus syncStatus = null;
            
            LibSpotify.sp_offline_sync_status status = new LibSpotify.sp_offline_sync_status();
            if (LibSpotify.sp_offline_sync_get_status_r(Handle, ref status))
            {
                syncStatus = new OfflineSyncStatus()
                {
                    QueuedBytes = Convert.ToInt64(status.queued_bytes),
                    QueuedTracks = status.queued_tracks,
                    DoneBytes = Convert.ToInt64(status.done_bytes),
                    DoneTracks = status.done_tracks,
                    CopiedBytes = Convert.ToInt64(status.copied_bytes),
                    CopiedTracks = status.copied_tracks,
                    WillNotCopyTracks = status.willnotcopy_tracks,
                    ErorrTracks = status.error_tracks,
                    Syncing = status.syncing
                };
            }
            return syncStatus;
        }

        public TrackAvailability GetTrackAvailability(Track track)
        {
            ThrowHelper.ThrowIfNull(track, "track");
            return LibSpotify.sp_track_get_availability_r(Handle, track.Handle);
        }

        public bool IsTrackLocal(Track track)
        {
            ThrowHelper.ThrowIfNull(track, "track");
            return LibSpotify.sp_track_is_local_r(Handle, track.Handle);
        }

        public bool IsTrackAutoLinked(Track track)
        {
            ThrowHelper.ThrowIfNull(track, "track");
            return LibSpotify.sp_track_is_autolinked_r(Handle, track.Handle);
        }

        public Track GetPlayableTrack(Track track)
        {
            ThrowHelper.ThrowIfNull(track, "track");
            return new Track(LibSpotify.sp_track_get_playable_r(Handle, track.Handle));
        }

        public bool IsTrackStarred(Track track)
        {
            ThrowHelper.ThrowIfNull(track, "track");
            return LibSpotify.sp_track_is_starred_r(Handle, track.Handle);
        }

        public void SetTracksStarred(IList<Track> tracks, bool starred)
        {
            ThrowHelper.ThrowIfNull(tracks, "tracks");

            if (tracks.Count > 0)
            {
                IntPtr[] trackHandles = new IntPtr[tracks.Count];
                for (int i = 0; i < tracks.Count; ++i)
                    trackHandles[i] = tracks[i].Handle;

                ThrowHelper.ThrowIfError(LibSpotify.sp_track_set_starred_r(Handle, trackHandles, 
                    trackHandles.Length, starred));
            }
        }

        public static Session Create(SessionConfig sessionConfig)
        {
            Session session = new Session();

            IntPtr applicationKeyHandle = IntPtr.Zero;
            IntPtr callbacksHandle = IntPtr.Zero;

            try
            {
                LibSpotify.sp_session_config config = new LibSpotify.sp_session_config();
                config.api_version = sessionConfig.ApiVersion;

                byte[] key = GetApplicationKey(sessionConfig);
                applicationKeyHandle = Marshal.AllocHGlobal(key.Length);
                Marshal.Copy(key, 0, applicationKeyHandle, key.Length);
                config.application_key = applicationKeyHandle;

                config.application_key_size = new IntPtr(key.Length);
                config.cache_location = sessionConfig.CacheLocation;
                config.compress_playlists = sessionConfig.CompressPlaylists;
                config.device_id = sessionConfig.DeviceId;
                config.dont_dave_metadata_for_playlists = sessionConfig.DontSaveMetadatForPlaylists;
                config.initially_unload_playlist = sessionConfig.InitiallyUnloadPlaylists;
                config.proxy = sessionConfig.Proxy;
                config.proxy_password = sessionConfig.ProxyPassword;
                config.proxy_username = sessionConfig.ProxyUserName;
                config.settings_location = sessionConfig.SettingsLocation;
                config.tracefile = sessionConfig.TraceFile;
                config.user_agent = sessionConfig.UserAgent;
                config.userdata = IntPtr.Zero;

                session._callbacks = new LibSpotify.sp_session_callbacks();
                session._callbacks.logged_in = session.RaiseLoggedInEvent;
                session._callbacks.logged_out = session.RaiseLoggedOutEvent;
                session._callbacks.notify_main_thread = session.RaiseNotifyMainThread;

                session._callbacks.log_message = session.RaiseLogMessage;
                session._callbacks.metadata_updated = session.RaiseMetadataUpdated;
                session._callbacks.connection_error = session.RaiseConnectionError;
                session._callbacks.message_to_user = session.RaiseMessageToUser;
                session._callbacks.music_delivery = session.RaiseMusicDelivery;
                session._callbacks.play_token_lost = session.RaisePlayTokenLost;
                session._callbacks.log_message = session.RaiseLogMessage;
                session._callbacks.end_of_track = session.RaiseEndOfTrack;
                session._callbacks.streaming_error = session.RaiseStreamingError;
                session._callbacks.userinfo_updated = session.RaiseUserInfoUpdated;
                session._callbacks.start_playback = session.RaiseStartPlayback;
                session._callbacks.stop_playback = session.RaiseStopPlayback;
                session._callbacks.offline_status_updated = session.RaiseOfflineStatusUpdated;
                session._callbacks.get_audio_buffer_stats = session.RaiseGetAudioBufferStats;
                session._callbacks.offline_error = session.RaiseOfflineError;
                session._callbacks.credentials_blob_updated = session.RaiseCredentialsBlobUpdated;
                session._callbacks.connectionstate_updated = session.RaiseConnectionStateUpdated;
                session._callbacks.scrobble_error = session.RaiseScrobbleError;
                session._callbacks.private_session_mode_changed = session.RaisePrivateSessionModeChanged;

                callbacksHandle = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(LibSpotify.sp_session_callbacks)));
                Marshal.StructureToPtr(session._callbacks, callbacksHandle, false);
                config.callbacks = callbacksHandle;

                IntPtr ptr = IntPtr.Zero;

                // This call results in a callback to RaiseNotifyMainThread, but the
                // actual _handle has not been set yet.
                session._running = true;
                ThrowHelper.ThrowIfError(LibSpotify.sp_session_create_r(ref config, ref ptr));
                session.Handle = ptr;
            }
            finally
            {
                if (applicationKeyHandle != IntPtr.Zero)
                    Marshal.FreeHGlobal(applicationKeyHandle);

                if (callbacksHandle != IntPtr.Zero)
                    Marshal.FreeHGlobal(callbacksHandle);
            }

            return session;
        }

        internal Internal.Image CreateImage(IntPtr imageId)
        {
            return new Internal.Image(LibSpotify.sp_image_create_r(Handle, imageId));
        }

        public void ProcessEvents()
        {
            TimeSpan nextTimeout = TimeSpan.FromHours(24);

            while (true)
            {
                _notifyEvent.WaitOne(nextTimeout);
                if (!_running)
                    return;

                do
                {
                    nextTimeout = ProcessEventsInternal();
                }
                while (nextTimeout == TimeSpan.MinValue);
            }
        }

        private void Stop()
        {
            _running = false;
            _notifyEvent.Set();
            _thread.Join();
            _thread = null;
        }

        private void HandleOnNotifyMainThread(object sender, EventArgs e)
        {
            _notifyEvent.Set();
        }

        private TimeSpan ProcessEventsInternal()
        {
            int nextTimeout = 0;
            lock (_lock)
            {
                ThrowHelper.ThrowIfError(LibSpotify.sp_session_process_events_r(Handle, ref nextTimeout));
            }
            return TimeSpan.FromMilliseconds(nextTimeout);
        }

        private int RaiseMusicDelivery(IntPtr sessionHandle, IntPtr audioFormatHandle, IntPtr frames, int numFrames)
        {
            if (numFrames == 0)
                return 0;

            if (frames == IntPtr.Zero)
                return 0;

            AudioFormat audioFormat = LibSpotify.AudioFormatFromHandle(audioFormatHandle);

            int n = numFrames * FrameSize(audioFormat);
            byte[] pcmData = new byte[n];

            Marshal.Copy(frames, pcmData, 0, n);

            EventDispatcher.Dispatch(this, sessionHandle, OnMusicDelivered,
                new MusicDeliveryEventArgs(pcmData, audioFormat));

            pcmData = null;
        
            return numFrames;
        }
  
        private static int FrameSize(AudioFormat audioFormat)
        {
            int sampleTypeSize = 0;
            switch (audioFormat.SampleType)
            {
                case SampleType.Int16NativeEndian:
                    sampleTypeSize = 2;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("sampleTypeSize");
            }
            return audioFormat.Channels * sampleTypeSize;
        }

        #region RaiseEvents
        private void RaiseLoggedInEvent(IntPtr sessionHandle, Error error)
        {
            EventDispatcher.Dispatch(this, sessionHandle, OnLoggedIn,
                new LoggedInEventArgs(error));
        }

        private void RaiseLoggedOutEvent(IntPtr sessionHandle)
        {
            EventDispatcher.Dispatch(this, sessionHandle, OnLoggedOut,
                EventArgs.Empty);
        }

        private void RaiseMetadataUpdated(IntPtr sessionHandle)
        {
            EventDispatcher.Dispatch(this, sessionHandle, OnMetadataUpdated,
                EventArgs.Empty);
        } 

        private void RaiseConnectionError(IntPtr sessionHandle, Error e)
        {
            EventDispatcher.Dispatch(this, sessionHandle, OnConnectionError,
                new ConnectionErrorEventArgs(e));
        }

        private void RaiseMessageToUser(IntPtr sessionHandle, IntPtr message)
        {
            EventDispatcher.Dispatch(this, sessionHandle, OnMessageToUser,
                new MessageToUserEventArgs(Marshal.PtrToStringAnsi(message)));
        }

        private void RaiseNotifyMainThread(IntPtr sessionHandle)
        {
            EventDispatcher.Dispatch(this, sessionHandle, OnNotifyMainThread,
                EventArgs.Empty);
        }

        private void RaisePlayTokenLost(IntPtr sessionHandle)
        {
            EventDispatcher.Dispatch(this, sessionHandle, OnPlayTokenLost, 
                EventArgs.Empty);
        }

        private void RaiseLogMessage(IntPtr sessionHandle, IntPtr message)
        {
            EventDispatcher.Dispatch(this, sessionHandle, OnLogMessage,
                new LogMessageEventArgs(Marshal.PtrToStringAnsi(message)));
        }

        private void RaiseEndOfTrack(IntPtr sessionHandle)
        {
            EventDispatcher.Dispatch(this, sessionHandle, OnEndOfTrack, 
                EventArgs.Empty);
        }

        private void RaiseStreamingError(IntPtr sessionHandle, Error e)
        {
            EventDispatcher.Dispatch(this, sessionHandle, OnStreamingError, 
                new StreamingErrorEventArgs(e));
        }

        private void RaiseUserInfoUpdated(IntPtr sessionHandle)
        {
            EventDispatcher.Dispatch(this, sessionHandle, OnUserInfoUpdated, 
                EventArgs.Empty);
        }

        private void RaiseStartPlayback(IntPtr sessionHandle)
        {
            EventDispatcher.Dispatch(this, sessionHandle, OnStartPlayback, 
                EventArgs.Empty);
        }

        private void RaiseStopPlayback(IntPtr sessionHandle)
        {
            EventDispatcher.Dispatch(this, sessionHandle, OnStopPlayback, 
                EventArgs.Empty);
        }

        private void RaiseGetAudioBufferStats(IntPtr sessionHandle, IntPtr statsHandle)
        {
            AudioBufferStats stats = LibSpotify.AudioBufferStatsFromHandle(statsHandle);
            //Dispatch(sessionHandle, OnAud)
        }

        private void RaiseOfflineStatusUpdated(IntPtr sessionHandle)
        {
            EventDispatcher.Dispatch(this, sessionHandle, OnOfflineStatusUpdated, 
                EventArgs.Empty);
        }

        private void RaiseOfflineError(IntPtr sessionHandle, Error e)
        {
            EventDispatcher.Dispatch(this, sessionHandle, OnOfflineError, 
                new OfflineErrorEventArgs(e));
        }

        private void RaiseCredentialsBlobUpdated(IntPtr sessionHandle, IntPtr blob)
        {
            EventDispatcher.Dispatch(this, sessionHandle, OnCredentialsBlobUpdated, 
                new CredentialsBlobUpdatedEventArgs(Marshal.PtrToStringAnsi(blob)));
        }

        private void RaiseConnectionStateUpdated(IntPtr sessionHandle)
        {
            EventDispatcher.Dispatch(this, sessionHandle, OnConnectionStateUpdated, 
                EventArgs.Empty);
        }

        private void RaiseScrobbleError(IntPtr sessionHandle, Error e)
        {
            EventDispatcher.Dispatch(this, sessionHandle, OnScrobbleError, 
                new ScrobbleErrorEventArgs(e));
        }

        private void RaisePrivateSessionModeChanged(IntPtr sessionHandle, bool b)
        {
            EventDispatcher.Dispatch(this, sessionHandle, OnPrivateSessionModeChanged, 
                new PrivateSessonModeChangedEventArgs(b));
        }
        #endregion

        ~Session()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            // make sure we dispose of any DomainOjects that are still around instead of 
            // waiting for the finalizer, otherwise we get exceptions from the unmanaged
            // layer when we try to release objects after releasing the session
            DomainObject.DisposeAllRemaingObjects();

            System.Diagnostics.Debug.Assert(Handle != IntPtr.Zero);
            if (Handle != IntPtr.Zero)
                LibSpotify.sp_session_release_r(Handle);


            Handle = IntPtr.Zero;
            _disposed = true;
        }

        internal IntPtr Handle
        {
            get
            {
                System.Diagnostics.Debug.Assert(_handle != IntPtr.Zero);

                if (_disposed)
                    throw new ObjectDisposedException("handle");

                return _handle;
            }
            set
            {
                _handle = value;
            }
        }

        private static byte[] GetApplicationKey(SessionConfig config)
        {
            ThrowHelper.ThrowIfNull(config, "config");
            if (string.IsNullOrEmpty(config.ApplicationKeyFile) && config.ApplicationKey == null)
                throw new ArgumentException("both ApplicationKey and ApplicationKeyFile are unset in SessionConfig");
          
            byte[] key = config.ApplicationKey;
            if (key == null)
                key = File.ReadAllBytes(config.ApplicationKeyFile);
            return key;
        }

        private Thread _thread;
        private volatile bool _running;
        private AutoResetEvent _notifyEvent = new AutoResetEvent(true);
        private IntPtr _handle;
        private bool _disposed;

        /// <summary>
        /// We maintain a handle to the callbacks structure since these callbacks are passed
        /// over the unmanaged boundary. This prevents the delegates from getting gc'd.
        /// </summary>
        private LibSpotify.sp_session_callbacks _callbacks;
        private object _lock = new object();
    }
}
