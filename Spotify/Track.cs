using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Spotify.Internal;

namespace Spotify
{
    public sealed class Track : DomainObject
    {
        internal Track(IntPtr handle, bool preIncremented = true)
            : base(handle, LibSpotify.sp_track_add_ref_r, LibSpotify.sp_track_release_r, preIncremented)
        {

        }

        public bool IsLoaded
        {
            get
            {
                return LibSpotify.sp_track_is_loaded_r(Handle);
            }
        }

        public Error Error
        {
            get
            {
                return LibSpotify.sp_track_error_r(Handle);
            }
        }

        public TrackOfflineStatus OfflineStatus
        {
            get
            {
                return LibSpotify.sp_track_offline_get_status_r(Handle);
            }
        }

        public bool IsPlaceHolder
        {
            get
            {
                return LibSpotify.sp_track_is_placeholder_r(Handle);
            }
        }

        public IList<Artist> Artists
        {
            get
            {
                return MakeList(p => { return new Artist(p, false); }, LibSpotify.sp_track_num_artists_r,
                    LibSpotify.sp_track_artist_r);
            }
        }

        public Album Album
        {
            get
            {
                return new Album(LibSpotify.sp_track_album_r(Handle), false);
            }
        }

        public string Name
        {
            get
            {
                return LibSpotify.ReadUtf8(LibSpotify.sp_track_name_r(Handle));
            }
        }

        public TimeSpan Duration
        {
            get
            {
                return TimeSpan.FromMilliseconds(LibSpotify.sp_track_duration_r(Handle));
            }
        }

        public int Popularity
        {
            get
            {
                return LibSpotify.sp_track_popularity_r(Handle);
            }
        }

        public int Disc
        {
            get
            {
                return LibSpotify.sp_track_disc_r(Handle);
            }
        }

        public int Index
        {
            get
            {
                return LibSpotify.sp_track_index_r(Handle);
            }
        }

        #region Public Methods
        public TrackAvailability GetAvailability(Session session, Track track)
        {
            ThrowHelper.ThrowIfNull(track, "track");
            return LibSpotify.sp_track_get_availability_r(session.Handle, track.Handle);
        }

        public bool IsLocal(Session session)
        {
            ThrowHelper.ThrowIfNull(session, "session");            
            return LibSpotify.sp_track_is_local_r(session.Handle, Handle);
        }

        public bool IsAutoLinked(Session session)
        {
            ThrowHelper.ThrowIfNull(session, "session");            
            return LibSpotify.sp_track_is_autolinked_r(session.Handle, Handle);
        }

        public Track GetPlayable(Session session)
        {
            ThrowHelper.ThrowIfNull(session, "session");            
            return new Track(LibSpotify.sp_track_get_playable_r(session.Handle, Handle));
        }

        public bool IsStarred(Session session)
        {
            ThrowHelper.ThrowIfNull(session, "session");            
            return LibSpotify.sp_track_is_starred_r(session.Handle, Handle);
        }

        public static void SetStarred(Session session, IList<Track> tracks, bool starred)
        {
            ThrowHelper.ThrowIfNull(session, "session");
            ThrowHelper.ThrowIfNull(tracks, "tracks");

            if (tracks.Count > 0)
            {
                IntPtr[] trackHandles = new IntPtr[tracks.Count];
                for (int i = 0; i < tracks.Count; ++i)
                    trackHandles[i] = tracks[i].Handle;

                ThrowHelper.ThrowIfError(LibSpotify.sp_track_set_starred_r(session.Handle, trackHandles,
                    trackHandles.Length, starred));
            }
        }
        #endregion
    }
}
