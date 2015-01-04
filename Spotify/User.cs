using System;
using Spotify.Internal;

namespace Spotify
{
    // complete
    public class User : DomainObject
    {
        internal User(IntPtr handle, bool preIncremented = true)
            : base(handle, LibSpotify.sp_user_add_ref_r, LibSpotify.sp_user_release_r, preIncremented)
        {

        }

        public string CanonicalName
        {
            get
            {
                return LibSpotify.ReadUtf8(LibSpotify.sp_user_canonical_name_r(Handle));
            }
        }

        public string DisplayName
        {
            get
            {
                return LibSpotify.ReadUtf8(LibSpotify.sp_user_display_name_r(Handle));
            }
        }

        public bool IsLoaded
        {
            get
            {
                return LibSpotify.sp_user_is_loaded_r(Handle);
            }
        }
    }
}
