using System;
using System.Threading.Tasks;

using Spotify.Internal;

namespace Spotify
{
    public class Artist : DomainObject
    {
        internal Artist(IntPtr handle, bool preIncremented = true)
            : base(handle, LibSpotify.sp_artist_add_ref_r, LibSpotify.sp_artist_release_r, preIncremented)
        {
        }

        #region Properties
        public bool IsLoaded
        {
            get
            {
                return LibSpotify.sp_artist_is_loaded_r(Handle);
            }
        }

        public string Name
        {
            get
            {
                return LibSpotify.ReadUtf8(LibSpotify.sp_artist_name_r(Handle));
            }
        }
        #endregion

        #region Async Methods
        public Task<System.Drawing.Image> SearchAsync(Session session, ImageSize size, AsyncCallback userCallback, object stateObject)
        {
            return Task.Factory.FromAsync<Session, ImageSize, System.Drawing.Image>(BeginLoadPortrait, EndLoadPortrait, session, size, stateObject);
        } 

        public IAsyncResult BeginLoadPortrait(Session session, ImageSize size, AsyncCallback userCallback, object stateObject)
        {
            return ImageLoader.Begin(LibSpotify.sp_artist_portrait_r, Handle, session, size,
                userCallback, stateObject);
        }

        public System.Drawing.Image EndLoadPortrait(IAsyncResult result)
        {
            return ImageLoader.End(result);
        }
        #endregion
    }
}
