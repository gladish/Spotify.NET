using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Spotify.Internal;

namespace Spotify
{
    public class Track : DomainObject
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
    }
}
