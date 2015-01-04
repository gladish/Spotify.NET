using System;
using Spotify.Internal;

namespace Spotify
{
    public class ArtistBrowse : DomainObject
    {
        internal ArtistBrowse(IntPtr handle, bool preIncremented = true)
            : base(handle, LibSpotify.sp_artistbrowse_add_ref_r, LibSpotify.sp_artistbrowse_release_r, preIncremented)
        {
        }
    }
}
