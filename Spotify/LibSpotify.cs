
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;

namespace Spotify
{
    internal static class LibSpotify
    {
        private static object _apiLock = new object();

        #region callbacks
        public delegate void SessionCallback(IntPtr session);
        public delegate void SessionCallbackWithError(IntPtr session, Error error);
        public delegate void SessionCallbackWithHandle(IntPtr session, IntPtr data);

        public delegate void AlbumBrowseComplete(IntPtr albumbrowse, IntPtr data);
        public delegate void ArtistBrowseComplete(IntPtr atistbrowse, IntPtr data);
        public delegate void ImageLoaded(IntPtr image, IntPtr data);
        public delegate void SearchComplete(IntPtr search, IntPtr data);
        public delegate void ToplistBrowseComplete(IntPtr search, IntPtr data);
        public delegate void InboxComplete(IntPtr inbox, IntPtr data);

        public delegate int SessionCallbackMusicDelivery(IntPtr session, IntPtr format, IntPtr frames, int num_frames);
        public delegate void SessionCallbackPtivateSessionModeChanged(IntPtr session, bool is_private);

        public delegate void PlaylistTracksAdded(IntPtr pl, int[] tracks, int num_tracks, int position, IntPtr userdata);
        public delegate void PlaylistTracksRemoved(IntPtr pl, int[] tracks, int num_tracks, IntPtr userdata);
        public delegate void PlaylistTracksMoved(IntPtr pl, int[] tracks, int num_tracks, int new_position, IntPtr userdata);
        public delegate void PlaylistRenamed(IntPtr pl, IntPtr userdata);
        public delegate void PlaylistStateChanged(IntPtr pl, IntPtr userdata);
        public delegate void PlaylistUpdateInProgress(IntPtr pl, [MarshalAs(UnmanagedType.U1)] bool done, IntPtr userdata);
        public delegate void PlaylistMetadataUpdated(IntPtr pl, IntPtr userdata);
        public delegate void PlaylistCreatedChanged(IntPtr pl, int position, IntPtr user, int when, IntPtr userdata);
        public delegate void PlaylistSeenChanged(IntPtr pl, int position, [MarshalAs(UnmanagedType.U1)] bool done, IntPtr userdata);
        public delegate void PlaylistDescriptionChanged(IntPtr pl, IntPtr description, IntPtr userdata);
        public delegate void PlaylistImageChanged(IntPtr pl, IntPtr image, IntPtr userdata);
        public delegate void PlaylistMessageChanged(IntPtr pl, int poistion, IntPtr message, IntPtr userdata);
        public delegate void PlaylistSubscribersChanged(IntPtr pl, IntPtr userdata);

        public delegate void PlaylistContainerPlaylistAdded(IntPtr pc, IntPtr playlist, int position, IntPtr userdata);
        public delegate void PlaylistContainerPlaylistRemoved(IntPtr pc, IntPtr playlist, int position, IntPtr userdata);
        public delegate void PlaylistContainerPlaylistMoved(IntPtr pc, IntPtr playlist, int position, int new_position, IntPtr userdata);
        public delegate void PlaylistContainerLoaded(IntPtr pc, IntPtr userdata);
        #endregion

        [StructLayout(LayoutKind.Sequential)]
        public struct sp_session_callbacks
        {
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public SessionCallbackWithError logged_in;

            [MarshalAs(UnmanagedType.FunctionPtr)]
            public SessionCallback logged_out;

            [MarshalAs(UnmanagedType.FunctionPtr)]
            public SessionCallback metadata_updated;

            [MarshalAs(UnmanagedType.FunctionPtr)]
            public SessionCallbackWithError connection_error;

            [MarshalAs(UnmanagedType.FunctionPtr)]
            public SessionCallbackWithHandle message_to_user;

            // dispatched via libspotify internal thread
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public SessionCallback notify_main_thread;

            // dispatched via libspotify internal thread
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public SessionCallbackMusicDelivery music_delivery;

            [MarshalAs(UnmanagedType.FunctionPtr)]
            public SessionCallback play_token_lost;

            [MarshalAs(UnmanagedType.FunctionPtr)]
            public SessionCallbackWithHandle log_message;

            // dispatched via libspotify internal thread
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public SessionCallback end_of_track;

            [MarshalAs(UnmanagedType.FunctionPtr)]
            public SessionCallbackWithError streaming_error;

            [MarshalAs(UnmanagedType.FunctionPtr)]
            public SessionCallback userinfo_updated;

            // dispatched via libspotify internal thread
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public SessionCallback start_playback;

            // dispatched via libspotify internal thread
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public SessionCallback stop_playback;

            // dispatched via libspotify internal thread
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public SessionCallbackWithHandle get_audio_buffer_stats;

            [MarshalAs(UnmanagedType.FunctionPtr)]
            public SessionCallback offline_status_updated;

            [MarshalAs(UnmanagedType.FunctionPtr)]
            public SessionCallbackWithError offline_error;

            [MarshalAs(UnmanagedType.FunctionPtr)]
            public SessionCallbackWithHandle credentials_blob_updated;

            [MarshalAs(UnmanagedType.FunctionPtr)]
            public SessionCallback connectionstate_updated;

            [MarshalAs(UnmanagedType.FunctionPtr)]
            public SessionCallbackWithError scrobble_error;

            [MarshalAs(UnmanagedType.FunctionPtr)]
            public SessionCallbackPtivateSessionModeChanged private_session_mode_changed;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct sp_session_config
        {
            public int api_version;

            [MarshalAs(UnmanagedType.LPStr)]
            public string cache_location;

            [MarshalAs(UnmanagedType.LPStr)]
            public string settings_location;

            public IntPtr application_key;
            public IntPtr application_key_size;

            [MarshalAs(UnmanagedType.LPStr)]
            public string user_agent;

            public IntPtr callbacks;
            public IntPtr userdata;

            [MarshalAs(UnmanagedType.I1)]
            public bool compress_playlists;

            [MarshalAs(UnmanagedType.I1)]
            public bool dont_dave_metadata_for_playlists;

            [MarshalAs(UnmanagedType.I1)]
            public bool initially_unload_playlist;

            [MarshalAs(UnmanagedType.LPStr)]
            public string device_id;

            [MarshalAs(UnmanagedType.LPStr)]
            public string proxy;

            [MarshalAs(UnmanagedType.LPStr)]
            public string proxy_username;

            [MarshalAs(UnmanagedType.LPStr)]
            public string proxy_password;

            [MarshalAs(UnmanagedType.LPStr)]
            public string tracefile;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct sp_offline_sync_status
        {
            public int queued_tracks;
            public ulong queued_bytes;

            public int done_tracks;
            public ulong done_bytes;

            public int copied_tracks;
            public ulong copied_bytes;

            public int willnotcopy_tracks;

            public int error_tracks;

            [MarshalAs(UnmanagedType.U1)]
            public bool syncing;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct sp_playlist_callbacks
        {
            public PlaylistTracksAdded tracks_added;
            public PlaylistTracksRemoved tracks_removed;
            public PlaylistTracksMoved tracks_moved;
            public PlaylistRenamed playlist_renamed;
            public PlaylistStateChanged playlist_state_changed;
            public PlaylistUpdateInProgress playlist_update_in_progress;
            public PlaylistMetadataUpdated playlist_metadata_updated;
            public PlaylistCreatedChanged track_created_changed;
            public PlaylistSeenChanged track_seen_changed;
            public PlaylistDescriptionChanged description_changed;
            public PlaylistImageChanged image_changed;
            public PlaylistMessageChanged track_message_changed;
            public PlaylistSubscribersChanged subscribers_changed;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct sp_playlistcontainer_callbacks
        {
            public PlaylistContainerPlaylistAdded playlist_added;
            public PlaylistContainerPlaylistRemoved playlist_removed;
            public PlaylistContainerPlaylistMoved playlist_moved;
            public PlaylistContainerLoaded container_loaded;
        }

        #region sp_album
        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_album_add_ref(System.IntPtr album);
        public static Spotify.Error sp_album_add_ref_r(System.IntPtr album)
        {
            lock (_apiLock)
                return sp_album_add_ref(album);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_album_artist(System.IntPtr album);
        public static System.IntPtr sp_album_artist_r(System.IntPtr album)
        {
            lock (_apiLock)
                return sp_album_artist(album);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_album_cover(System.IntPtr album, Spotify.ImageSize size);
        public static System.IntPtr sp_album_cover_r(System.IntPtr album, Spotify.ImageSize size)
        {
            lock (_apiLock)
                return sp_album_cover(album, size);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Boolean sp_album_is_available(System.IntPtr album);
        public static System.Boolean sp_album_is_available_r(System.IntPtr album)
        {
            lock (_apiLock)
                return sp_album_is_available(album);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Boolean sp_album_is_loaded(System.IntPtr album);
        public static System.Boolean sp_album_is_loaded_r(System.IntPtr album)
        {
            lock (_apiLock)
                return sp_album_is_loaded(album);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_album_name(System.IntPtr album);
        public static System.IntPtr sp_album_name_r(System.IntPtr album)
        {
            lock (_apiLock)
                return sp_album_name(album);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_album_release(System.IntPtr album);
        public static Spotify.Error sp_album_release_r(System.IntPtr album)
        {
            lock (_apiLock)
                return sp_album_release(album);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.AlbumType sp_album_type(System.IntPtr album);
        public static Spotify.AlbumType sp_album_type_r(System.IntPtr album)
        {
            lock (_apiLock)
                return sp_album_type(album);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Int32 sp_album_year(System.IntPtr album);
        public static System.Int32 sp_album_year_r(System.IntPtr album)
        {
            lock (_apiLock)
                return sp_album_year(album);
        }

        #endregion

        #region sp_albumbrowse
        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_albumbrowse_add_ref(System.IntPtr alb);
        public static Spotify.Error sp_albumbrowse_add_ref_r(System.IntPtr alb)
        {
            lock (_apiLock)
                return sp_albumbrowse_add_ref(alb);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_albumbrowse_album(System.IntPtr alb);
        public static System.IntPtr sp_albumbrowse_album_r(System.IntPtr alb)
        {
            lock (_apiLock)
                return sp_albumbrowse_album(alb);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_albumbrowse_artist(System.IntPtr alb);
        public static System.IntPtr sp_albumbrowse_artist_r(System.IntPtr alb)
        {
            lock (_apiLock)
                return sp_albumbrowse_artist(alb);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Int32 sp_albumbrowse_backend_request_duration(System.IntPtr alb);
        public static System.Int32 sp_albumbrowse_backend_request_duration_r(System.IntPtr alb)
        {
            lock (_apiLock)
                return sp_albumbrowse_backend_request_duration(alb);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_albumbrowse_copyright(System.IntPtr alb, System.Int32 index);
        public static System.IntPtr sp_albumbrowse_copyright_r(System.IntPtr alb, System.Int32 index)
        {
            lock (_apiLock)
                return sp_albumbrowse_copyright(alb, index);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_albumbrowse_create(System.IntPtr session, System.IntPtr album, LibSpotify.AlbumBrowseComplete callback, System.IntPtr userdata);
        public static System.IntPtr sp_albumbrowse_create_r(System.IntPtr session, System.IntPtr album, LibSpotify.AlbumBrowseComplete callback, System.IntPtr userdata)
        {
            lock (_apiLock)
                return sp_albumbrowse_create(session, album, callback, userdata);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_albumbrowse_error(System.IntPtr alb);
        public static Spotify.Error sp_albumbrowse_error_r(System.IntPtr alb)
        {
            lock (_apiLock)
                return sp_albumbrowse_error(alb);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Boolean sp_albumbrowse_is_loaded(System.IntPtr alb);
        public static System.Boolean sp_albumbrowse_is_loaded_r(System.IntPtr alb)
        {
            lock (_apiLock)
                return sp_albumbrowse_is_loaded(alb);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Int32 sp_albumbrowse_num_copyrights(System.IntPtr alb);
        public static System.Int32 sp_albumbrowse_num_copyrights_r(System.IntPtr alb)
        {
            lock (_apiLock)
                return sp_albumbrowse_num_copyrights(alb);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Int32 sp_albumbrowse_num_tracks(System.IntPtr alb);
        public static System.Int32 sp_albumbrowse_num_tracks_r(System.IntPtr alb)
        {
            lock (_apiLock)
                return sp_albumbrowse_num_tracks(alb);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_albumbrowse_release(System.IntPtr alb);
        public static Spotify.Error sp_albumbrowse_release_r(System.IntPtr alb)
        {
            lock (_apiLock)
                return sp_albumbrowse_release(alb);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_albumbrowse_review(System.IntPtr alb);
        public static System.IntPtr sp_albumbrowse_review_r(System.IntPtr alb)
        {
            lock (_apiLock)
                return sp_albumbrowse_review(alb);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_albumbrowse_track(System.IntPtr alb, System.Int32 index);
        public static System.IntPtr sp_albumbrowse_track_r(System.IntPtr alb, System.Int32 index)
        {
            lock (_apiLock)
                return sp_albumbrowse_track(alb, index);
        }

        #endregion

        #region sp_artist
        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_artist_add_ref(System.IntPtr artist);
        public static Spotify.Error sp_artist_add_ref_r(System.IntPtr artist)
        {
            lock (_apiLock)
                return sp_artist_add_ref(artist);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Boolean sp_artist_is_loaded(System.IntPtr artist);
        public static System.Boolean sp_artist_is_loaded_r(System.IntPtr artist)
        {
            lock (_apiLock)
                return sp_artist_is_loaded(artist);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_artist_name(System.IntPtr artist);
        public static System.IntPtr sp_artist_name_r(System.IntPtr artist)
        {
            lock (_apiLock)
                return sp_artist_name(artist);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_artist_portrait(System.IntPtr artist, Spotify.ImageSize size);
        public static System.IntPtr sp_artist_portrait_r(System.IntPtr artist, Spotify.ImageSize size)
        {
            lock (_apiLock)
                return sp_artist_portrait(artist, size);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_artist_release(System.IntPtr artist);
        public static Spotify.Error sp_artist_release_r(System.IntPtr artist)
        {
            lock (_apiLock)
                return sp_artist_release(artist);
        }

        #endregion

        #region sp_artistbrowse
        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_artistbrowse_add_ref(System.IntPtr arb);
        public static Spotify.Error sp_artistbrowse_add_ref_r(System.IntPtr arb)
        {
            lock (_apiLock)
                return sp_artistbrowse_add_ref(arb);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_artistbrowse_album(System.IntPtr arb, System.Int32 index);
        public static System.IntPtr sp_artistbrowse_album_r(System.IntPtr arb, System.Int32 index)
        {
            lock (_apiLock)
                return sp_artistbrowse_album(arb, index);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_artistbrowse_artist(System.IntPtr arb);
        public static System.IntPtr sp_artistbrowse_artist_r(System.IntPtr arb)
        {
            lock (_apiLock)
                return sp_artistbrowse_artist(arb);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Int32 sp_artistbrowse_backend_request_duration(System.IntPtr arb);
        public static System.Int32 sp_artistbrowse_backend_request_duration_r(System.IntPtr arb)
        {
            lock (_apiLock)
                return sp_artistbrowse_backend_request_duration(arb);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_artistbrowse_biography(System.IntPtr arb);
        public static System.IntPtr sp_artistbrowse_biography_r(System.IntPtr arb)
        {
            lock (_apiLock)
                return sp_artistbrowse_biography(arb);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_artistbrowse_create(System.IntPtr session, System.IntPtr artist, Spotify.ArtistBrowseType type, LibSpotify.ArtistBrowseComplete callback, System.IntPtr userdata);
        public static System.IntPtr sp_artistbrowse_create_r(System.IntPtr session, System.IntPtr artist, Spotify.ArtistBrowseType type, LibSpotify.ArtistBrowseComplete callback, System.IntPtr userdata)
        {
            lock (_apiLock)
                return sp_artistbrowse_create(session, artist, type, callback, userdata);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_artistbrowse_error(System.IntPtr arb);
        public static Spotify.Error sp_artistbrowse_error_r(System.IntPtr arb)
        {
            lock (_apiLock)
                return sp_artistbrowse_error(arb);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Boolean sp_artistbrowse_is_loaded(System.IntPtr arb);
        public static System.Boolean sp_artistbrowse_is_loaded_r(System.IntPtr arb)
        {
            lock (_apiLock)
                return sp_artistbrowse_is_loaded(arb);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Int32 sp_artistbrowse_num_albums(System.IntPtr arb);
        public static System.Int32 sp_artistbrowse_num_albums_r(System.IntPtr arb)
        {
            lock (_apiLock)
                return sp_artistbrowse_num_albums(arb);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Int32 sp_artistbrowse_num_portraits(System.IntPtr arb);
        public static System.Int32 sp_artistbrowse_num_portraits_r(System.IntPtr arb)
        {
            lock (_apiLock)
                return sp_artistbrowse_num_portraits(arb);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Int32 sp_artistbrowse_num_similar_artists(System.IntPtr arb);
        public static System.Int32 sp_artistbrowse_num_similar_artists_r(System.IntPtr arb)
        {
            lock (_apiLock)
                return sp_artistbrowse_num_similar_artists(arb);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Int32 sp_artistbrowse_num_tophit_tracks(System.IntPtr arb);
        public static System.Int32 sp_artistbrowse_num_tophit_tracks_r(System.IntPtr arb)
        {
            lock (_apiLock)
                return sp_artistbrowse_num_tophit_tracks(arb);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Int32 sp_artistbrowse_num_tracks(System.IntPtr arb);
        public static System.Int32 sp_artistbrowse_num_tracks_r(System.IntPtr arb)
        {
            lock (_apiLock)
                return sp_artistbrowse_num_tracks(arb);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_artistbrowse_portrait(System.IntPtr arb, System.Int32 index);
        public static System.IntPtr sp_artistbrowse_portrait_r(System.IntPtr arb, System.Int32 index)
        {
            lock (_apiLock)
                return sp_artistbrowse_portrait(arb, index);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_artistbrowse_release(System.IntPtr arb);
        public static Spotify.Error sp_artistbrowse_release_r(System.IntPtr arb)
        {
            lock (_apiLock)
                return sp_artistbrowse_release(arb);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_artistbrowse_similar_artist(System.IntPtr arb, System.Int32 index);
        public static System.IntPtr sp_artistbrowse_similar_artist_r(System.IntPtr arb, System.Int32 index)
        {
            lock (_apiLock)
                return sp_artistbrowse_similar_artist(arb, index);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_artistbrowse_tophit_track(System.IntPtr arb, System.Int32 index);
        public static System.IntPtr sp_artistbrowse_tophit_track_r(System.IntPtr arb, System.Int32 index)
        {
            lock (_apiLock)
                return sp_artistbrowse_tophit_track(arb, index);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_artistbrowse_track(System.IntPtr arb, System.Int32 index);
        public static System.IntPtr sp_artistbrowse_track_r(System.IntPtr arb, System.Int32 index)
        {
            lock (_apiLock)
                return sp_artistbrowse_track(arb, index);
        }

        #endregion

        #region sp_build
        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_build_id();
        public static System.IntPtr sp_build_id_r()
        {
            lock (_apiLock)
                return sp_build_id();
        }

        #endregion

        #region sp_error
        [System.Runtime.InteropServices.DllImport("libspotify")]
        public static extern System.IntPtr sp_error_message(Spotify.Error e);
        public static System.IntPtr sp_error_message_r(Spotify.Error e)
        {
            lock (_apiLock)
                return sp_error_message(e);
        }
        #endregion

        #region sp_image
        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_image_add_load_callback(System.IntPtr image, LibSpotify.ImageLoaded callback, System.IntPtr data);
        public static Spotify.Error sp_image_add_load_callback_r(System.IntPtr image, LibSpotify.ImageLoaded callback, System.IntPtr data)
        {
            lock (_apiLock)
                return sp_image_add_load_callback(image, callback, data);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_image_add_ref(System.IntPtr image);
        public static Spotify.Error sp_image_add_ref_r(System.IntPtr image)
        {
            lock (_apiLock)
                return sp_image_add_ref(image);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_image_create(System.IntPtr session, IntPtr image_id);
        public static System.IntPtr sp_image_create_r(System.IntPtr session, IntPtr image_id)
        {
            lock (_apiLock)
                return sp_image_create(session, image_id);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_image_create_from_link(System.IntPtr session, System.IntPtr link);
        public static System.IntPtr sp_image_create_from_link_r(System.IntPtr session, System.IntPtr link)
        {
            lock (_apiLock)
                return sp_image_create_from_link(session, link);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_image_data(System.IntPtr image, ref System.IntPtr data_size);
        public static System.IntPtr sp_image_data_r(System.IntPtr image, ref System.IntPtr data_size)
        {
            lock (_apiLock)
                return sp_image_data(image, ref data_size);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_image_error(System.IntPtr image);
        public static Spotify.Error sp_image_error_r(System.IntPtr image)
        {
            lock (_apiLock)
                return sp_image_error(image);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.ImageFormat sp_image_format(System.IntPtr image);
        public static Spotify.ImageFormat sp_image_format_r(System.IntPtr image)
        {
            lock (_apiLock)
                return sp_image_format(image);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_image_image_id(System.IntPtr image);
        public static System.IntPtr sp_image_image_id_r(System.IntPtr image)
        {
            lock (_apiLock)
                return sp_image_image_id(image);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Boolean sp_image_is_loaded(System.IntPtr image);
        public static System.Boolean sp_image_is_loaded_r(System.IntPtr image)
        {
            lock (_apiLock)
                return sp_image_is_loaded(image);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_image_release(System.IntPtr image);
        public static Spotify.Error sp_image_release_r(System.IntPtr image)
        {
            lock (_apiLock)
                return sp_image_release(image);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_image_remove_load_callback(System.IntPtr image, LibSpotify.ImageLoaded callback, System.IntPtr data);
        public static Spotify.Error sp_image_remove_load_callback_r(System.IntPtr image, LibSpotify.ImageLoaded callback, System.IntPtr data)
        {
            lock (_apiLock)
                return sp_image_remove_load_callback(image, callback, data);
        }
        #endregion

        #region sp_inbox
        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_inbox_add_ref(System.IntPtr inbox);
        public static Spotify.Error sp_inbox_add_ref_r(System.IntPtr inbox)
        {
            lock (_apiLock)
                return sp_inbox_add_ref(inbox);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_inbox_error(System.IntPtr inbox);
        public static Spotify.Error sp_inbox_error_r(System.IntPtr inbox)
        {
            lock (_apiLock)
                return sp_inbox_error(inbox);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_inbox_post_tracks(System.IntPtr session, System.String user, System.IntPtr[] tracks, System.Int32 num_tracks, System.String message, LibSpotify.InboxComplete callback, System.IntPtr userdata);
        public static System.IntPtr sp_inbox_post_tracks_r(System.IntPtr session, System.String user, System.IntPtr[] tracks, System.Int32 num_tracks, System.String message, LibSpotify.InboxComplete callback, System.IntPtr userdata)
        {
            lock (_apiLock)
                return sp_inbox_post_tracks(session, user, tracks, num_tracks, message, callback, userdata);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_inbox_release(System.IntPtr inbox);
        public static Spotify.Error sp_inbox_release_r(System.IntPtr inbox)
        {
            lock (_apiLock)
                return sp_inbox_release(inbox);
        }
        #endregion

        #region sp_link
        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_link_add_ref(System.IntPtr link);
        public static Spotify.Error sp_link_add_ref_r(System.IntPtr link)
        {
            lock (_apiLock)
                return sp_link_add_ref(link);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_link_as_album(System.IntPtr link);
        public static System.IntPtr sp_link_as_album_r(System.IntPtr link)
        {
            lock (_apiLock)
                return sp_link_as_album(link);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_link_as_artist(System.IntPtr link);
        public static System.IntPtr sp_link_as_artist_r(System.IntPtr link)
        {
            lock (_apiLock)
                return sp_link_as_artist(link);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Int32 sp_link_as_string(System.IntPtr link, System.IntPtr buff, System.Int32 n);
        public static System.Int32 sp_link_as_string_r(System.IntPtr link, System.IntPtr buff, System.Int32 n)
        {
            lock (_apiLock)
                return sp_link_as_string(link, buff, n);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_link_as_track(System.IntPtr link);
        public static System.IntPtr sp_link_as_track_r(System.IntPtr link)
        {
            lock (_apiLock)
                return sp_link_as_track(link);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_link_as_track_and_offset(System.IntPtr link, ref System.Int32 offset);
        public static System.IntPtr sp_link_as_track_and_offset_r(System.IntPtr link, ref System.Int32 offset)
        {
            lock (_apiLock)
                return sp_link_as_track_and_offset(link, ref offset);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_link_as_user(System.IntPtr link);
        public static System.IntPtr sp_link_as_user_r(System.IntPtr link)
        {
            lock (_apiLock)
                return sp_link_as_user(link);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_link_create_from_album(System.IntPtr album);
        public static System.IntPtr sp_link_create_from_album_r(System.IntPtr album)
        {
            lock (_apiLock)
                return sp_link_create_from_album(album);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_link_create_from_album_cover(System.IntPtr album, Spotify.ImageSize size);
        public static System.IntPtr sp_link_create_from_album_cover_r(System.IntPtr album, Spotify.ImageSize size)
        {
            lock (_apiLock)
                return sp_link_create_from_album_cover(album, size);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_link_create_from_artist(System.IntPtr artist);
        public static System.IntPtr sp_link_create_from_artist_r(System.IntPtr artist)
        {
            lock (_apiLock)
                return sp_link_create_from_artist(artist);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_link_create_from_artist_portrait(System.IntPtr artist, Spotify.ImageSize size);
        public static System.IntPtr sp_link_create_from_artist_portrait_r(System.IntPtr artist, Spotify.ImageSize size)
        {
            lock (_apiLock)
                return sp_link_create_from_artist_portrait(artist, size);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_link_create_from_artistbrowse_portrait(System.IntPtr artist_browse, System.Int32 index);
        public static System.IntPtr sp_link_create_from_artistbrowse_portrait_r(System.IntPtr artist_browse, System.Int32 index)
        {
            lock (_apiLock)
                return sp_link_create_from_artistbrowse_portrait(artist_browse, index);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_link_create_from_image(System.IntPtr image);
        public static System.IntPtr sp_link_create_from_image_r(System.IntPtr image)
        {
            lock (_apiLock)
                return sp_link_create_from_image(image);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_link_create_from_playlist(System.IntPtr playlist);
        public static System.IntPtr sp_link_create_from_playlist_r(System.IntPtr playlist)
        {
            lock (_apiLock)
                return sp_link_create_from_playlist(playlist);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_link_create_from_search(System.IntPtr search);
        public static System.IntPtr sp_link_create_from_search_r(System.IntPtr search)
        {
            lock (_apiLock)
                return sp_link_create_from_search(search);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_link_create_from_string(System.String link);
        public static System.IntPtr sp_link_create_from_string_r(System.String link)
        {
            lock (_apiLock)
                return sp_link_create_from_string(link);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_link_create_from_track(System.IntPtr track, System.Int32 offset);
        public static System.IntPtr sp_link_create_from_track_r(System.IntPtr track, System.Int32 offset)
        {
            lock (_apiLock)
                return sp_link_create_from_track(track, offset);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_link_create_from_user(System.IntPtr user);
        public static System.IntPtr sp_link_create_from_user_r(System.IntPtr user)
        {
            lock (_apiLock)
                return sp_link_create_from_user(user);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_link_release(System.IntPtr link);
        public static Spotify.Error sp_link_release_r(System.IntPtr link)
        {
            lock (_apiLock)
                return sp_link_release(link);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.LinkType sp_link_type(System.IntPtr link);
        public static Spotify.LinkType sp_link_type_r(System.IntPtr link)
        {
            lock (_apiLock)
                return sp_link_type(link);
        }
        #endregion

        #region sp_localtrack
        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_localtrack_create(System.String artist, System.String title, System.String album, System.Int32 n);
        public static System.IntPtr sp_localtrack_create_r(System.String artist, System.String title, System.String album, System.Int32 n)
        {
            lock (_apiLock)
                return sp_localtrack_create(artist, title, album, n);
        }
        #endregion

        #region sp_offline
        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Int32 sp_offline_num_playlists(System.IntPtr session);
        public static System.Int32 sp_offline_num_playlists_r(System.IntPtr session)
        {
            lock (_apiLock)
                return sp_offline_num_playlists(session);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Boolean sp_offline_sync_get_status(System.IntPtr session, ref LibSpotify.sp_offline_sync_status status);
        public static System.Boolean sp_offline_sync_get_status_r(System.IntPtr session, ref LibSpotify.sp_offline_sync_status status)
        {
            lock (_apiLock)
                return sp_offline_sync_get_status(session, ref status);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Int32 sp_offline_time_left(System.IntPtr session);
        public static System.Int32 sp_offline_time_left_r(System.IntPtr session)
        {
            lock (_apiLock)
                return sp_offline_time_left(session);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Int32 sp_offline_tracks_to_sync(System.IntPtr session);
        public static System.Int32 sp_offline_tracks_to_sync_r(System.IntPtr session)
        {
            lock (_apiLock)
                return sp_offline_tracks_to_sync(session);
        }
        #endregion

        #region sp_playlist
        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_playlist_add_callbacks(System.IntPtr playlist, ref LibSpotify.sp_playlist_callbacks callbacks, System.IntPtr userdata);
        public static Spotify.Error sp_playlist_add_callbacks_r(System.IntPtr playlist, ref LibSpotify.sp_playlist_callbacks callbacks, System.IntPtr userdata)
        {
            lock (_apiLock)
                return sp_playlist_add_callbacks(playlist, ref callbacks, userdata);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_playlist_add_ref(System.IntPtr playlist);
        public static Spotify.Error sp_playlist_add_ref_r(System.IntPtr playlist)
        {
            lock (_apiLock)
                return sp_playlist_add_ref(playlist);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_playlist_add_tracks(System.IntPtr playlist, System.IntPtr[] tracks, System.Int32 num_tracks, System.Int32 position, System.IntPtr session);
        public static Spotify.Error sp_playlist_add_tracks_r(System.IntPtr playlist, System.IntPtr[] tracks, System.Int32 num_tracks, System.Int32 position, System.IntPtr session)
        {
            lock (_apiLock)
                return sp_playlist_add_tracks(playlist, tracks, num_tracks, position, session);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_playlist_create(System.IntPtr session, System.IntPtr link);
        public static System.IntPtr sp_playlist_create_r(System.IntPtr session, System.IntPtr link)
        {
            lock (_apiLock)
                return sp_playlist_create(session, link);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_playlist_get_description(System.IntPtr playlist);
        public static System.IntPtr sp_playlist_get_description_r(System.IntPtr playlist)
        {
            lock (_apiLock)
                return sp_playlist_get_description(playlist);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Boolean sp_playlist_get_image(System.IntPtr playlist, System.Byte[] image);
        public static System.Boolean sp_playlist_get_image_r(System.IntPtr playlist, System.Byte[] image)
        {
            lock (_apiLock)
                return sp_playlist_get_image(playlist, image);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Int32 sp_playlist_get_offline_download_completed(System.IntPtr session, System.IntPtr playlist);
        public static System.Int32 sp_playlist_get_offline_download_completed_r(System.IntPtr session, System.IntPtr playlist)
        {
            lock (_apiLock)
                return sp_playlist_get_offline_download_completed(session, playlist);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.PlaylistOfflineStatus sp_playlist_get_offline_status(System.IntPtr session, System.IntPtr playlist);
        public static Spotify.PlaylistOfflineStatus sp_playlist_get_offline_status_r(System.IntPtr session, System.IntPtr playlist)
        {
            lock (_apiLock)
                return sp_playlist_get_offline_status(session, playlist);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Boolean sp_playlist_has_pending_changes(System.IntPtr playlist);
        public static System.Boolean sp_playlist_has_pending_changes_r(System.IntPtr playlist)
        {
            lock (_apiLock)
                return sp_playlist_has_pending_changes(playlist);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Boolean sp_playlist_is_collaborative(System.IntPtr playlist);
        public static System.Boolean sp_playlist_is_collaborative_r(System.IntPtr playlist)
        {
            lock (_apiLock)
                return sp_playlist_is_collaborative(playlist);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Boolean sp_playlist_is_in_ram(System.IntPtr session, System.IntPtr subscribers);
        public static System.Boolean sp_playlist_is_in_ram_r(System.IntPtr session, System.IntPtr subscribers)
        {
            lock (_apiLock)
                return sp_playlist_is_in_ram(session, subscribers);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Boolean sp_playlist_is_loaded(System.IntPtr playlist);
        public static System.Boolean sp_playlist_is_loaded_r(System.IntPtr playlist)
        {
            lock (_apiLock)
                return sp_playlist_is_loaded(playlist);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_playlist_name(System.IntPtr playlist);
        public static System.IntPtr sp_playlist_name_r(System.IntPtr playlist)
        {
            lock (_apiLock)
                return sp_playlist_name(playlist);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.UInt32 sp_playlist_num_subscribers(System.IntPtr playlist);
        public static System.UInt32 sp_playlist_num_subscribers_r(System.IntPtr playlist)
        {
            lock (_apiLock)
                return sp_playlist_num_subscribers(playlist);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Int32 sp_playlist_num_tracks(System.IntPtr playlist);
        public static System.Int32 sp_playlist_num_tracks_r(System.IntPtr playlist)
        {
            lock (_apiLock)
                return sp_playlist_num_tracks(playlist);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_playlist_owner(System.IntPtr playlist);
        public static System.IntPtr sp_playlist_owner_r(System.IntPtr playlist)
        {
            lock (_apiLock)
                return sp_playlist_owner(playlist);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_playlist_release(System.IntPtr playlist);
        public static Spotify.Error sp_playlist_release_r(System.IntPtr playlist)
        {
            lock (_apiLock)
                return sp_playlist_release(playlist);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_playlist_remove_callbacks(System.IntPtr playlist, ref LibSpotify.sp_playlist_callbacks callbacks, System.IntPtr userdata);
        public static Spotify.Error sp_playlist_remove_callbacks_r(System.IntPtr playlist, ref LibSpotify.sp_playlist_callbacks callbacks, System.IntPtr userdata)
        {
            lock (_apiLock)
                return sp_playlist_remove_callbacks(playlist, ref callbacks, userdata);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_playlist_remove_tracks(System.IntPtr playlist, System.Int32[] tracks, System.Int32 num_tracks);
        public static Spotify.Error sp_playlist_remove_tracks_r(System.IntPtr playlist, System.Int32[] tracks, System.Int32 num_tracks)
        {
            lock (_apiLock)
                return sp_playlist_remove_tracks(playlist, tracks, num_tracks);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_playlist_rename(System.IntPtr playlist, System.String new_new);
        public static Spotify.Error sp_playlist_rename_r(System.IntPtr playlist, System.String new_new)
        {
            lock (_apiLock)
                return sp_playlist_rename(playlist, new_new);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_playlist_reorder_tracks(System.IntPtr playlist, System.Int32[] tracks, System.Int32 num_tracks, System.Int32 new_position);
        public static Spotify.Error sp_playlist_reorder_tracks_r(System.IntPtr playlist, System.Int32[] tracks, System.Int32 num_tracks, System.Int32 new_position)
        {
            lock (_apiLock)
                return sp_playlist_reorder_tracks(playlist, tracks, num_tracks, new_position);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_playlist_set_autolink_tracks(System.IntPtr playlist, System.Boolean link);
        public static Spotify.Error sp_playlist_set_autolink_tracks_r(System.IntPtr playlist, System.Boolean link)
        {
            lock (_apiLock)
                return sp_playlist_set_autolink_tracks(playlist, link);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_playlist_set_collaborative(System.IntPtr playlist, System.Boolean collaborative);
        public static Spotify.Error sp_playlist_set_collaborative_r(System.IntPtr playlist, System.Boolean collaborative)
        {
            lock (_apiLock)
                return sp_playlist_set_collaborative(playlist, collaborative);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_playlist_set_in_ram(System.IntPtr session, System.IntPtr playlist, System.Boolean in_ram);
        public static Spotify.Error sp_playlist_set_in_ram_r(System.IntPtr session, System.IntPtr playlist, System.Boolean in_ram)
        {
            lock (_apiLock)
                return sp_playlist_set_in_ram(session, playlist, in_ram);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_playlist_set_offline_mode(System.IntPtr session, System.IntPtr playlist, System.Boolean offline);
        public static Spotify.Error sp_playlist_set_offline_mode_r(System.IntPtr session, System.IntPtr playlist, System.Boolean offline)
        {
            lock (_apiLock)
                return sp_playlist_set_offline_mode(session, playlist, offline);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_playlist_subscribers(System.IntPtr playlist);
        public static System.IntPtr sp_playlist_subscribers_r(System.IntPtr playlist)
        {
            lock (_apiLock)
                return sp_playlist_subscribers(playlist);
        }

        internal static IList<string> GetPlaylistSubscribers(IntPtr playlist)
        {
            List<string> subscribers = new List<string>();

            IntPtr p = LibSpotify.sp_playlist_subscribers_r(playlist);
            if (p != IntPtr.Zero)
            {
                int n = Marshal.ReadInt32(p);

                int offset = Marshal.SizeOf(typeof(Int32));
                for (int i = 0; i < n; ++i)
                {
                    subscribers.Add(ReadUtf8(Marshal.ReadIntPtr(p, offset)));
                    offset += Marshal.SizeOf(typeof(IntPtr));
                }

                LibSpotify.sp_playlist_subscribers_free_r(p);
            }

            return subscribers;
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_playlist_subscribers_free(System.IntPtr subscribers);
        public static Spotify.Error sp_playlist_subscribers_free_r(System.IntPtr subscribers)
        {
            lock (_apiLock)
                return sp_playlist_subscribers_free(subscribers);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_playlist_update_subscribers(System.IntPtr session, System.IntPtr subscribers);
        public static Spotify.Error sp_playlist_update_subscribers_r(System.IntPtr session, System.IntPtr subscribers)
        {
            lock (_apiLock)
                return sp_playlist_update_subscribers(session, subscribers);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_playlist_track(System.IntPtr playlist, System.Int32 index);
        public static System.IntPtr sp_playlist_track_r(System.IntPtr playlist, System.Int32 index)
        {
            lock (_apiLock)
                return sp_playlist_track(playlist, index);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Int32 sp_playlist_track_create_time(System.IntPtr playlist, System.Int32 index);
        public static System.Int32 sp_playlist_track_create_time_r(System.IntPtr playlist, System.Int32 index)
        {
            lock (_apiLock)
                return sp_playlist_track_create_time(playlist, index);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_playlist_track_creator(System.IntPtr playlist, System.Int32 index);
        public static System.IntPtr sp_playlist_track_creator_r(System.IntPtr playlist, System.Int32 index)
        {
            lock (_apiLock)
                return sp_playlist_track_creator(playlist, index);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_playlist_track_message(System.IntPtr playlist, System.Int32 index);
        public static System.IntPtr sp_playlist_track_message_r(System.IntPtr playlist, System.Int32 index)
        {
            lock (_apiLock)
                return sp_playlist_track_message(playlist, index);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Boolean sp_playlist_track_seen(System.IntPtr playlist, System.Int32 index);
        public static System.Boolean sp_playlist_track_seen_r(System.IntPtr playlist, System.Int32 index)
        {
            lock (_apiLock)
                return sp_playlist_track_seen(playlist, index);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_playlist_track_set_seen(System.IntPtr playlist, System.Int32 index, System.Boolean seen);
        public static Spotify.Error sp_playlist_track_set_seen_r(System.IntPtr playlist, System.Int32 index, System.Boolean seen)
        {
            lock (_apiLock)
                return sp_playlist_track_set_seen(playlist, index, seen);
        }
        #endregion

        #region sp_playlistcontainer
        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_playlistcontainer_add_callbacks(System.IntPtr pc, ref LibSpotify.sp_playlistcontainer_callbacks callbacks, System.IntPtr userdata);
        public static Spotify.Error sp_playlistcontainer_add_callbacks_r(System.IntPtr pc, ref LibSpotify.sp_playlistcontainer_callbacks callbacks, System.IntPtr userdata)
        {
            lock (_apiLock)
                return sp_playlistcontainer_add_callbacks(pc, ref callbacks, userdata);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_playlistcontainer_add_folder(System.IntPtr pc, System.Int32 index, System.String name);
        public static Spotify.Error sp_playlistcontainer_add_folder_r(System.IntPtr pc, System.Int32 index, System.String name)
        {
            lock (_apiLock)
                return sp_playlistcontainer_add_folder(pc, index, name);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_playlistcontainer_add_new_playlist(System.IntPtr pc, System.String name);
        public static System.IntPtr sp_playlistcontainer_add_new_playlist_r(System.IntPtr pc, System.String name)
        {
            lock (_apiLock)
                return sp_playlistcontainer_add_new_playlist(pc, name);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
                                            
        private static extern System.IntPtr sp_playlistcontainer_add_playlist(System.IntPtr pc, System.IntPtr link);
        public static System.IntPtr sp_playlistcontainer_add_playlist_r(System.IntPtr pc, System.IntPtr link)
        {
            lock (_apiLock)
                return sp_playlistcontainer_add_playlist(pc, link);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_playlistcontainer_add_ref(System.IntPtr pc);
        public static Spotify.Error sp_playlistcontainer_add_ref_r(System.IntPtr pc)
        {
            lock (_apiLock)
                return sp_playlistcontainer_add_ref(pc);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Int32 sp_playlistcontainer_clear_unseen_tracks(System.IntPtr pc, System.IntPtr playlist);
        public static System.Int32 sp_playlistcontainer_clear_unseen_tracks_r(System.IntPtr pc, System.IntPtr playlist)
        {
            lock (_apiLock)
                return sp_playlistcontainer_clear_unseen_tracks(pc, playlist);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Int32 sp_playlistcontainer_get_unseen_tracks(System.IntPtr pc, System.IntPtr playlist, System.IntPtr[] tracks, System.Int32 num_tracks);
        public static System.Int32 sp_playlistcontainer_get_unseen_tracks_r(System.IntPtr pc, System.IntPtr playlist, System.IntPtr[] tracks, System.Int32 num_tracks)
        {
            lock (_apiLock)
                return sp_playlistcontainer_get_unseen_tracks(pc, playlist, tracks, num_tracks);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Boolean sp_playlistcontainer_is_loaded(System.IntPtr pc);
        public static System.Boolean sp_playlistcontainer_is_loaded_r(System.IntPtr pc)
        {
            lock (_apiLock)
                return sp_playlistcontainer_is_loaded(pc);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_playlistcontainer_move_playlist(System.IntPtr pc, System.Int32 index, System.Int32 new_position, System.Boolean dry_run);
        public static Spotify.Error sp_playlistcontainer_move_playlist_r(System.IntPtr pc, System.Int32 index, System.Int32 new_position, System.Boolean dry_run)
        {
            lock (_apiLock)
                return sp_playlistcontainer_move_playlist(pc, index, new_position, dry_run);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Int32 sp_playlistcontainer_num_playlists(System.IntPtr pc);
        public static System.Int32 sp_playlistcontainer_num_playlists_r(System.IntPtr pc)
        {
            lock (_apiLock)
                return sp_playlistcontainer_num_playlists(pc);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_playlistcontainer_owner(System.IntPtr pc);
        public static System.IntPtr sp_playlistcontainer_owner_r(System.IntPtr pc)
        {
            lock (_apiLock)
                return sp_playlistcontainer_owner(pc);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_playlistcontainer_playlist(System.IntPtr pc, System.Int32 index);
        public static System.IntPtr sp_playlistcontainer_playlist_r(System.IntPtr pc, System.Int32 index)
        {
            lock (_apiLock)
                return sp_playlistcontainer_playlist(pc, index);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.UInt64 sp_playlistcontainer_playlist_folder_id(System.IntPtr pc, System.Int32 index);
        public static System.UInt64 sp_playlistcontainer_playlist_folder_id_r(System.IntPtr pc, System.Int32 index)
        {
            lock (_apiLock)
                return sp_playlistcontainer_playlist_folder_id(pc, index);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_playlistcontainer_playlist_folder_name(System.IntPtr pc, System.Int32 index, System.Text.StringBuilder buffer, System.Int32 buffer_size);
        public static Spotify.Error sp_playlistcontainer_playlist_folder_name_r(System.IntPtr pc, System.Int32 index, System.Text.StringBuilder buffer, System.Int32 buffer_size)
        {
            lock (_apiLock)
                return sp_playlistcontainer_playlist_folder_name(pc, index, buffer, buffer_size);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.PlaylistType sp_playlistcontainer_playlist_type(System.IntPtr pc, System.Int32 index);
        public static Spotify.PlaylistType sp_playlistcontainer_playlist_type_r(System.IntPtr pc, System.Int32 index)
        {
            lock (_apiLock)
                return sp_playlistcontainer_playlist_type(pc, index);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_playlistcontainer_release(System.IntPtr pc);
        public static Spotify.Error sp_playlistcontainer_release_r(System.IntPtr pc)
        {
            lock (_apiLock)
                return sp_playlistcontainer_release(pc);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_playlistcontainer_remove_callbacks(System.IntPtr pc, ref LibSpotify.sp_playlistcontainer_callbacks callbacks, System.IntPtr userdata);
        public static Spotify.Error sp_playlistcontainer_remove_callbacks_r(System.IntPtr pc, ref LibSpotify.sp_playlistcontainer_callbacks callbacks, System.IntPtr userdata)
        {
            lock (_apiLock)
                return sp_playlistcontainer_remove_callbacks(pc, ref callbacks, userdata);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_playlistcontainer_remove_playlist(System.IntPtr pc, System.Int32 index);
        public static Spotify.Error sp_playlistcontainer_remove_playlist_r(System.IntPtr pc, System.Int32 index)
        {
            lock (_apiLock)
                return sp_playlistcontainer_remove_playlist(pc, index);
        }
        #endregion

        #region sp_search
        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_search_add_ref(System.IntPtr search);
        public static Spotify.Error sp_search_add_ref_r(System.IntPtr search)
        {
            lock (_apiLock)
                return sp_search_add_ref(search);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_search_album(System.IntPtr search, System.Int32 index);
        public static System.IntPtr sp_search_album_r(System.IntPtr search, System.Int32 index)
        {
            lock (_apiLock)
                return sp_search_album(search, index);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_search_artist(System.IntPtr search, System.Int32 index);
        public static System.IntPtr sp_search_artist_r(System.IntPtr search, System.Int32 index)
        {
            lock (_apiLock)
                return sp_search_artist(search, index);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_search_create(System.IntPtr session, System.String query, System.Int32 track_offset, System.Int32 track_count, System.Int32 album_offset, System.Int32 album_count, System.Int32 artist_offset, System.Int32 artist_count, System.Int32 playlist_offset, System.Int32 playlist_count, Spotify.SearchType search_type, LibSpotify.SearchComplete callback, System.IntPtr userdata);
        public static System.IntPtr sp_search_create_r(System.IntPtr session, System.String query, System.Int32 track_offset, System.Int32 track_count, System.Int32 album_offset, System.Int32 album_count, System.Int32 artist_offset, System.Int32 artist_count, System.Int32 playlist_offset, System.Int32 playlist_count, Spotify.SearchType search_type, LibSpotify.SearchComplete callback, System.IntPtr userdata)
        {
            lock (_apiLock)
                return sp_search_create(session, query, track_offset, track_count, album_offset, album_count, artist_offset, artist_count, playlist_offset, playlist_count, search_type, callback, userdata);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_search_did_you_mean(System.IntPtr search);
        public static System.IntPtr sp_search_did_you_mean_r(System.IntPtr search)
        {
            lock (_apiLock)
                return sp_search_did_you_mean(search);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_search_error(System.IntPtr search);
        public static Spotify.Error sp_search_error_r(System.IntPtr search)
        {
            lock (_apiLock)
                return sp_search_error(search);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Boolean sp_search_is_loaded(System.IntPtr search);
        public static System.Boolean sp_search_is_loaded_r(System.IntPtr search)
        {
            lock (_apiLock)
                return sp_search_is_loaded(search);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Int32 sp_search_num_albums(System.IntPtr search);
        public static System.Int32 sp_search_num_albums_r(System.IntPtr search)
        {
            lock (_apiLock)
                return sp_search_num_albums(search);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Int32 sp_search_num_artists(System.IntPtr search);
        public static System.Int32 sp_search_num_artists_r(System.IntPtr search)
        {
            lock (_apiLock)
                return sp_search_num_artists(search);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Int32 sp_search_num_playlists(System.IntPtr search);
        public static System.Int32 sp_search_num_playlists_r(System.IntPtr search)
        {
            lock (_apiLock)
                return sp_search_num_playlists(search);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Int32 sp_search_num_tracks(System.IntPtr search);
        public static System.Int32 sp_search_num_tracks_r(System.IntPtr search)
        {
            lock (_apiLock)
                return sp_search_num_tracks(search);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_search_playlist(System.IntPtr search, System.Int32 index);
        public static System.IntPtr sp_search_playlist_r(System.IntPtr search, System.Int32 index)
        {
            lock (_apiLock)
                return sp_search_playlist(search, index);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_search_playlist_image_uri(System.IntPtr search, System.Int32 index);
        public static System.IntPtr sp_search_playlist_image_uri_r(System.IntPtr search, System.Int32 index)
        {
            lock (_apiLock)
                return sp_search_playlist_image_uri(search, index);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_search_playlist_uri(System.IntPtr search, System.Int32 index);
        public static System.IntPtr sp_search_playlist_uri_r(System.IntPtr search, System.Int32 index)
        {
            lock (_apiLock)
                return sp_search_playlist_uri(search, index);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_search_query(System.IntPtr search);
        public static System.IntPtr sp_search_query_r(System.IntPtr search)
        {
            lock (_apiLock)
                return sp_search_query(search);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_search_release(System.IntPtr search);
        public static Spotify.Error sp_search_release_r(System.IntPtr search)
        {
            lock (_apiLock)
                return sp_search_release(search);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Int32 sp_search_total_albums(System.IntPtr search);
        public static System.Int32 sp_search_total_albums_r(System.IntPtr search)
        {
            lock (_apiLock)
                return sp_search_total_albums(search);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Int32 sp_search_total_artists(System.IntPtr search);
        public static System.Int32 sp_search_total_artists_r(System.IntPtr search)
        {
            lock (_apiLock)
                return sp_search_total_artists(search);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Int32 sp_search_total_playlists(System.IntPtr search);
        public static System.Int32 sp_search_total_playlists_r(System.IntPtr search)
        {
            lock (_apiLock)
                return sp_search_total_playlists(search);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Int32 sp_search_total_tracks(System.IntPtr search);
        public static System.Int32 sp_search_total_tracks_r(System.IntPtr search)
        {
            lock (_apiLock)
                return sp_search_total_tracks(search);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_search_track(System.IntPtr search, System.Int32 index);
        public static System.IntPtr sp_search_track_r(System.IntPtr search, System.Int32 index)
        {
            lock (_apiLock)
                return sp_search_track(search, index);
        }
        #endregion

        #region sp_session
        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.ConnectionState sp_session_connectionstate(System.IntPtr session);
        public static Spotify.ConnectionState sp_session_connectionstate_r(System.IntPtr session)
        {
            lock (_apiLock)
                return sp_session_connectionstate(session);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_session_create(ref LibSpotify.sp_session_config config, ref System.IntPtr session);
        public static Spotify.Error sp_session_create_r(ref LibSpotify.sp_session_config config, ref System.IntPtr session)
        {
            lock (_apiLock)
                return sp_session_create(ref config, ref session);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_session_flush_caches(System.IntPtr session);
        public static Spotify.Error sp_session_flush_caches_r(System.IntPtr session)
        {
            lock (_apiLock)
                return sp_session_flush_caches(session);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_session_forget_me(System.IntPtr session);
        public static Spotify.Error sp_session_forget_me_r(System.IntPtr session)
        {
            lock (_apiLock)
                return sp_session_forget_me(session);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Boolean sp_session_get_volume_normalization(System.IntPtr session);
        public static System.Boolean sp_session_get_volume_normalization_r(System.IntPtr session)
        {
            lock (_apiLock)
                return sp_session_get_volume_normalization(session);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_session_inbox_create(System.IntPtr session);
        public static System.IntPtr sp_session_inbox_create_r(System.IntPtr session)
        {
            lock (_apiLock)
                return sp_session_inbox_create(session);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Boolean sp_session_is_private_session(System.IntPtr session);
        public static System.Boolean sp_session_is_private_session_r(System.IntPtr session)
        {
            lock (_apiLock)
                return sp_session_is_private_session(session);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_session_is_scrobbling(System.IntPtr session, Spotify.SocialProvider provider, ref Spotify.ScrobblingState state);
        public static Spotify.Error sp_session_is_scrobbling_r(System.IntPtr session, Spotify.SocialProvider provider, ref Spotify.ScrobblingState state)
        {
            lock (_apiLock)
                return sp_session_is_scrobbling(session, provider, ref state);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_session_is_scrobbling_possible(System.IntPtr session, Spotify.SocialProvider provider, ref System.Boolean scrobbling);
        public static Spotify.Error sp_session_is_scrobbling_possible_r(System.IntPtr session, Spotify.SocialProvider provider, ref System.Boolean scrobbling)
        {
            lock (_apiLock)
                return sp_session_is_scrobbling_possible(session, provider, ref scrobbling);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_session_login(System.IntPtr session, System.String username, System.String password, System.Boolean remember_me, System.IntPtr blob);
        public static Spotify.Error sp_session_login_r(System.IntPtr session, System.String username, System.String password, System.Boolean remember_me, System.IntPtr blob)
        {
            lock (_apiLock)
                return sp_session_login(session, username, password, remember_me, blob);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_session_logout(System.IntPtr session);
        public static Spotify.Error sp_session_logout_r(System.IntPtr session)
        {
            lock (_apiLock)
                return sp_session_logout(session);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_session_player_load(System.IntPtr session, System.IntPtr track);
        public static Spotify.Error sp_session_player_load_r(System.IntPtr session, System.IntPtr track)
        {
            lock (_apiLock)
                return sp_session_player_load(session, track);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_session_player_play(System.IntPtr session, System.Boolean play);
        public static Spotify.Error sp_session_player_play_r(System.IntPtr session, System.Boolean play)
        {
            lock (_apiLock)
                return sp_session_player_play(session, play);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_session_player_prefetch(System.IntPtr session, System.IntPtr track);
        public static Spotify.Error sp_session_player_prefetch_r(System.IntPtr session, System.IntPtr track)
        {
            lock (_apiLock)
                return sp_session_player_prefetch(session, track);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_session_player_seek(System.IntPtr session, System.Int32 offset);
        public static Spotify.Error sp_session_player_seek_r(System.IntPtr session, System.Int32 offset)
        {
            lock (_apiLock)
                return sp_session_player_seek(session, offset);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_session_player_unload(System.IntPtr session);
        public static Spotify.Error sp_session_player_unload_r(System.IntPtr session)
        {
            lock (_apiLock)
                return sp_session_player_unload(session);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_session_playlistcontainer(System.IntPtr session);
        public static System.IntPtr sp_session_playlistcontainer_r(System.IntPtr session)
        {
            lock (_apiLock)
                return sp_session_playlistcontainer(session);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_session_preferred_bitrate(System.IntPtr session, Spotify.BitRate bitrate);
        public static Spotify.Error sp_session_preferred_bitrate_r(System.IntPtr session, Spotify.BitRate bitrate)
        {
            lock (_apiLock)
                return sp_session_preferred_bitrate(session, bitrate);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_session_preferred_offline_bitrate(System.IntPtr session, Spotify.BitRate bitrate, System.Boolean allow_resync);
        public static Spotify.Error sp_session_preferred_offline_bitrate_r(System.IntPtr session, Spotify.BitRate bitrate, System.Boolean allow_resync)
        {
            lock (_apiLock)
                return sp_session_preferred_offline_bitrate(session, bitrate, allow_resync);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_session_process_events(System.IntPtr session, ref System.Int32 next_timeout);
        public static Spotify.Error sp_session_process_events_r(System.IntPtr session, ref System.Int32 next_timeout)
        {
            lock (_apiLock)
                return sp_session_process_events(session, ref next_timeout);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_session_publishedcontainer_for_user_create(System.IntPtr session, System.String canonical_username);
        public static System.IntPtr sp_session_publishedcontainer_for_user_create_r(System.IntPtr session, System.String canonical_username)
        {
            lock (_apiLock)
                return sp_session_publishedcontainer_for_user_create(session, canonical_username);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_session_release(System.IntPtr session);
        public static Spotify.Error sp_session_release_r(System.IntPtr session)
        {
            lock (_apiLock)
                return sp_session_release(session);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_session_relogin(System.IntPtr session);
        public static Spotify.Error sp_session_relogin_r(System.IntPtr session)
        {
            lock (_apiLock)
                return sp_session_relogin(session);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Int32 sp_session_remembered_user(System.IntPtr session, System.IntPtr buff, System.Int32 len);
        public static System.Int32 sp_session_remembered_user_r(System.IntPtr session, System.IntPtr buff, System.Int32 len)
        {
            lock (_apiLock)
                return sp_session_remembered_user(session, buff, len);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_session_set_cache_size(System.IntPtr session, System.IntPtr size);
        public static Spotify.Error sp_session_set_cache_size_r(System.IntPtr session, System.IntPtr size)
        {
            lock (_apiLock)
                return sp_session_set_cache_size(session, size);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_session_set_connection_rules(System.IntPtr session, Spotify.ConnectionRules rules);
        public static Spotify.Error sp_session_set_connection_rules_r(System.IntPtr session, Spotify.ConnectionRules rules)
        {
            lock (_apiLock)
                return sp_session_set_connection_rules(session, rules);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_session_set_connection_type(System.IntPtr session, Spotify.ConnectionType type);
        public static Spotify.Error sp_session_set_connection_type_r(System.IntPtr session, Spotify.ConnectionType type)
        {
            lock (_apiLock)
                return sp_session_set_connection_type(session, type);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_session_set_private_session(System.IntPtr session, System.Boolean enabled);
        public static Spotify.Error sp_session_set_private_session_r(System.IntPtr session, System.Boolean enabled)
        {
            lock (_apiLock)
                return sp_session_set_private_session(session, enabled);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_session_set_scrobbling(System.IntPtr session, Spotify.SocialProvider provider, Spotify.ScrobblingState state);
        public static Spotify.Error sp_session_set_scrobbling_r(System.IntPtr session, Spotify.SocialProvider provider, Spotify.ScrobblingState state)
        {
            lock (_apiLock)
                return sp_session_set_scrobbling(session, provider, state);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_session_set_social_credentials(System.IntPtr session, Spotify.SocialProvider provider, System.String username, System.String password);
        public static Spotify.Error sp_session_set_social_credentials_r(System.IntPtr session, Spotify.SocialProvider provider, System.String username, System.String password)
        {
            lock (_apiLock)
                return sp_session_set_social_credentials(session, provider, username, password);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_session_set_volume_normalization(System.IntPtr session, System.Boolean on);
        public static Spotify.Error sp_session_set_volume_normalization_r(System.IntPtr session, System.Boolean on)
        {
            lock (_apiLock)
                return sp_session_set_volume_normalization(session, on);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_session_starred_create(System.IntPtr session);
        public static System.IntPtr sp_session_starred_create_r(System.IntPtr session)
        {
            lock (_apiLock)
                return sp_session_starred_create(session);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_session_starred_for_user_create(System.IntPtr session, System.String canonical_username);
        public static System.IntPtr sp_session_starred_for_user_create_r(System.IntPtr session, System.String canonical_username)
        {
            lock (_apiLock)
                return sp_session_starred_for_user_create(session, canonical_username);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Int32 sp_session_tracks_to_sync(System.IntPtr session);
        public static System.Int32 sp_session_tracks_to_sync_r(System.IntPtr session)
        {
            lock (_apiLock)
                return sp_session_tracks_to_sync(session);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_session_user(System.IntPtr session);
        public static System.IntPtr sp_session_user_r(System.IntPtr session)
        {
            lock (_apiLock)
                return sp_session_user(session);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Int32 sp_session_user_country(System.IntPtr session);
        public static System.Int32 sp_session_user_country_r(System.IntPtr session)
        {
            lock (_apiLock)
                return sp_session_user_country(session);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_session_user_name(System.IntPtr session);
        public static System.IntPtr sp_session_user_name_r(System.IntPtr session)
        {
            lock (_apiLock)
                return sp_session_user_name(session);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_session_userdata(System.IntPtr session);
        public static System.IntPtr sp_session_userdata_r(System.IntPtr session)
        {
            lock (_apiLock)
                return sp_session_userdata(session);
        }

        #endregion

        #region sp_toplistbrowse
        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_toplistbrowse_add_ref(System.IntPtr tlb);
        public static Spotify.Error sp_toplistbrowse_add_ref_r(System.IntPtr tlb)
        {
            lock (_apiLock)
                return sp_toplistbrowse_add_ref(tlb);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_toplistbrowse_album(System.IntPtr tlb, System.Int32 index);
        public static System.IntPtr sp_toplistbrowse_album_r(System.IntPtr tlb, System.Int32 index)
        {
            lock (_apiLock)
                return sp_toplistbrowse_album(tlb, index);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_toplistbrowse_artist(System.IntPtr tlb, System.Int32 index);
        public static System.IntPtr sp_toplistbrowse_artist_r(System.IntPtr tlb, System.Int32 index)
        {
            lock (_apiLock)
                return sp_toplistbrowse_artist(tlb, index);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Int32 sp_toplistbrowse_backend_request_duration(System.IntPtr tlb);
        public static System.Int32 sp_toplistbrowse_backend_request_duration_r(System.IntPtr tlb)
        {
            lock (_apiLock)
                return sp_toplistbrowse_backend_request_duration(tlb);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_toplistbrowse_create(System.IntPtr session, Spotify.ToplistType type, Spotify.ToplistRegion region, System.String username, LibSpotify.ToplistBrowseComplete callback, System.IntPtr userdata);
        public static System.IntPtr sp_toplistbrowse_create_r(System.IntPtr session, Spotify.ToplistType type, Spotify.ToplistRegion region, System.String username, LibSpotify.ToplistBrowseComplete callback, System.IntPtr userdata)
        {
            lock (_apiLock)
                return sp_toplistbrowse_create(session, type, region, username, callback, userdata);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_toplistbrowse_error(System.IntPtr tlb);
        public static Spotify.Error sp_toplistbrowse_error_r(System.IntPtr tlb)
        {
            lock (_apiLock)
                return sp_toplistbrowse_error(tlb);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Boolean sp_toplistbrowse_is_loaded(System.IntPtr tlb);
        public static System.Boolean sp_toplistbrowse_is_loaded_r(System.IntPtr tlb)
        {
            lock (_apiLock)
                return sp_toplistbrowse_is_loaded(tlb);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Int32 sp_toplistbrowse_num_albums(System.IntPtr tlb);
        public static System.Int32 sp_toplistbrowse_num_albums_r(System.IntPtr tlb)
        {
            lock (_apiLock)
                return sp_toplistbrowse_num_albums(tlb);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Int32 sp_toplistbrowse_num_artists(System.IntPtr tlb);
        public static System.Int32 sp_toplistbrowse_num_artists_r(System.IntPtr tlb)
        {
            lock (_apiLock)
                return sp_toplistbrowse_num_artists(tlb);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Int32 sp_toplistbrowse_num_tracks(System.IntPtr tlb);
        public static System.Int32 sp_toplistbrowse_num_tracks_r(System.IntPtr tlb)
        {
            lock (_apiLock)
                return sp_toplistbrowse_num_tracks(tlb);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_toplistbrowse_release(System.IntPtr tlb);
        public static Spotify.Error sp_toplistbrowse_release_r(System.IntPtr tlb)
        {
            lock (_apiLock)
                return sp_toplistbrowse_release(tlb);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_toplistbrowse_track(System.IntPtr tlb, System.Int32 index);
        public static System.IntPtr sp_toplistbrowse_track_r(System.IntPtr tlb, System.Int32 index)
        {
            lock (_apiLock)
                return sp_toplistbrowse_track(tlb, index);
        }

        #endregion

        #region sp_track
        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_track_add_ref(System.IntPtr track);
        public static Spotify.Error sp_track_add_ref_r(System.IntPtr track)
        {
            lock (_apiLock)
                return sp_track_add_ref(track);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_track_album(System.IntPtr track);
        public static System.IntPtr sp_track_album_r(System.IntPtr track)
        {
            lock (_apiLock)
                return sp_track_album(track);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_track_artist(System.IntPtr track, System.Int32 index);
        public static System.IntPtr sp_track_artist_r(System.IntPtr track, System.Int32 index)
        {
            lock (_apiLock)
                return sp_track_artist(track, index);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Int32 sp_track_disc(System.IntPtr track);
        public static System.Int32 sp_track_disc_r(System.IntPtr track)
        {
            lock (_apiLock)
                return sp_track_disc(track);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Int32 sp_track_duration(System.IntPtr track);
        public static System.Int32 sp_track_duration_r(System.IntPtr track)
        {
            lock (_apiLock)
                return sp_track_duration(track);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_track_error(System.IntPtr track);
        public static Spotify.Error sp_track_error_r(System.IntPtr track)
        {
            lock (_apiLock)
                return sp_track_error(track);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.TrackAvailability sp_track_get_availability(System.IntPtr session, System.IntPtr track);
        public static Spotify.TrackAvailability sp_track_get_availability_r(System.IntPtr session, System.IntPtr track)
        {
            lock (_apiLock)
                return sp_track_get_availability(session, track);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_track_get_playable(System.IntPtr session, System.IntPtr track);
        public static System.IntPtr sp_track_get_playable_r(System.IntPtr session, System.IntPtr track)
        {
            lock (_apiLock)
                return sp_track_get_playable(session, track);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Int32 sp_track_index(System.IntPtr track);
        public static System.Int32 sp_track_index_r(System.IntPtr track)
        {
            lock (_apiLock)
                return sp_track_index(track);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Boolean sp_track_is_autolinked(System.IntPtr session, System.IntPtr track);
        public static System.Boolean sp_track_is_autolinked_r(System.IntPtr session, System.IntPtr track)
        {
            lock (_apiLock)
                return sp_track_is_autolinked(session, track);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Boolean sp_track_is_loaded(System.IntPtr track);
        public static System.Boolean sp_track_is_loaded_r(System.IntPtr track)
        {
            lock (_apiLock)
                return sp_track_is_loaded(track);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Boolean sp_track_is_local(System.IntPtr session, System.IntPtr track);
        public static System.Boolean sp_track_is_local_r(System.IntPtr session, System.IntPtr track)
        {
            lock (_apiLock)
                return sp_track_is_local(session, track);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Boolean sp_track_is_placeholder(System.IntPtr track);
        public static System.Boolean sp_track_is_placeholder_r(System.IntPtr track)
        {
            lock (_apiLock)
                return sp_track_is_placeholder(track);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Boolean sp_track_is_starred(System.IntPtr session, System.IntPtr track);
        public static System.Boolean sp_track_is_starred_r(System.IntPtr session, System.IntPtr track)
        {
            lock (_apiLock)
                return sp_track_is_starred(session, track);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_track_name(System.IntPtr track);
        public static System.IntPtr sp_track_name_r(System.IntPtr track)
        {
            lock (_apiLock)
                return sp_track_name(track);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Int32 sp_track_num_artists(System.IntPtr track);
        public static System.Int32 sp_track_num_artists_r(System.IntPtr track)
        {
            lock (_apiLock)
                return sp_track_num_artists(track);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.TrackOfflineStatus sp_track_offline_get_status(System.IntPtr track);
        public static Spotify.TrackOfflineStatus sp_track_offline_get_status_r(System.IntPtr track)
        {
            lock (_apiLock)
                return sp_track_offline_get_status(track);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Int32 sp_track_popularity(System.IntPtr track);
        public static System.Int32 sp_track_popularity_r(System.IntPtr track)
        {
            lock (_apiLock)
                return sp_track_popularity(track);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_track_release(System.IntPtr track);
        public static Spotify.Error sp_track_release_r(System.IntPtr track)
        {
            lock (_apiLock)
                return sp_track_release(track);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_track_set_starred(System.IntPtr session, System.IntPtr[] tracks, System.Int32 num_tracks, System.Boolean star);
        public static Spotify.Error sp_track_set_starred_r(System.IntPtr session, System.IntPtr[] tracks, System.Int32 num_tracks, System.Boolean star)
        {
            lock (_apiLock)
                return sp_track_set_starred(session, tracks, num_tracks, star);
        }

        #endregion

        #region sp_user
        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_user_add_ref(System.IntPtr user);
        public static Spotify.Error sp_user_add_ref_r(System.IntPtr user)
        {
            lock (_apiLock)
                return sp_user_add_ref(user);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_user_canonical_name(System.IntPtr user);
        public static System.IntPtr sp_user_canonical_name_r(System.IntPtr user)
        {
            lock (_apiLock)
                return sp_user_canonical_name(user);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.IntPtr sp_user_display_name(System.IntPtr user);
        public static System.IntPtr sp_user_display_name_r(System.IntPtr user)
        {
            lock (_apiLock)
                return sp_user_display_name(user);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern System.Boolean sp_user_is_loaded(System.IntPtr user);
        public static System.Boolean sp_user_is_loaded_r(System.IntPtr user)
        {
            lock (_apiLock)
                return sp_user_is_loaded(user);
        }

        [System.Runtime.InteropServices.DllImport("libspotify")]
        private static extern Spotify.Error sp_user_release(System.IntPtr user);
        public static Spotify.Error sp_user_release_r(System.IntPtr user)
        {
            lock (_apiLock)
                return sp_user_release(user);
        }
        #endregion

        #region helpers
       

        public static AudioFormat AudioFormatFromHandle(IntPtr p)
        {
            return (AudioFormat)Marshal.PtrToStructure(p, typeof(AudioFormat));
        }

        public static AudioBufferStats AudioBufferStatsFromHandle(IntPtr p)
        {
            return (AudioBufferStats)Marshal.PtrToStructure(p, typeof(AudioBufferStats));
        }

        public static string ReadUtf8(IntPtr p)
        {
            if (p == IntPtr.Zero)
                return null;

            List<byte> bytes = new List<byte>(256);

            byte b;
            while ((b = Marshal.ReadByte(p, bytes.Count)) != 0)
                bytes.Add(b);

            return bytes.Count > 0
                ? Encoding.UTF8.GetString(bytes.ToArray())
                : string.Empty;
        }

        public delegate int StringGetterWithBuffer(IntPtr p, IntPtr buff, int n);
        public static string ReadUtf8(IntPtr handle, StringGetterWithBuffer get)
        {
            int n = 0;
            int size = 256;
            string s = null;

            IntPtr p = IntPtr.Zero;

            do
            {
                p = Marshal.AllocHGlobal(size);

                try
                {
                    if ((n = get(handle, p, size)) == -1)
                        return null;

                    if (n < size)
                        s = LibSpotify.ReadUtf8(p);
                    else
                        size = n + 1;
                }
                finally
                {
                    Marshal.FreeHGlobal(p);
                }
            }
            while (s == null);

            return s;
        }
        #endregion
    }
}
