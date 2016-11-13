using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotify
{
    public class AlbumBrowse : DomainObject
    {
        internal AlbumBrowse(IntPtr handle, bool preIncremented = true)
            : base(handle, LibSpotify.sp_artistbrowse_add_ref_r, LibSpotify.sp_artistbrowse_release_r, preIncremented)
        {           
        }


        public IList<String> Copyrights
        {
            get
            {
                return MakeList<string>(p => { return LibSpotify.ReadUtf8(p); }, LibSpotify.sp_albumbrowse_num_copyrights_r,
                       LibSpotify.sp_albumbrowse_copyright_r).AsReadOnly();                
            }
        }

        public string Review
        {
            get
            {
                return LibSpotify.ReadUtf8(LibSpotify.sp_albumbrowse_review_r(Handle));
            }
        }

        public IList<Track> Tracks
        {
            get
            {
                return MakeList(p => { return new Track(p, false); }, LibSpotify.sp_albumbrowse_num_tracks_r,
                    LibSpotify.sp_albumbrowse_track_r).AsReadOnly();
            }
        }

        public Album Album
        {
            get
            {
                return new Album(LibSpotify.sp_albumbrowse_album_r(Handle), false);
            }
        }

        public Error Error
        {
            get
            {
                return LibSpotify.sp_albumbrowse_error_r(Handle);
            }
        }
    }
}
