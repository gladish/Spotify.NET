using System;
using System.Collections.Generic;
using Spotify.Internal;


namespace Spotify
{
    public class Playlist : DomainObject
    {
        internal Playlist(IntPtr handle, bool preIncremented = true)
            : base(handle, LibSpotify.sp_playlist_add_ref_r, LibSpotify.sp_playlist_release_r, preIncremented)
        {
        }
    }
}
