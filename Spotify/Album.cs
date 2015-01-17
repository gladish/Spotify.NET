using System;
using System.Threading.Tasks;

namespace Spotify
{
    public sealed class Album : DomainObject
    {
        internal Album(IntPtr handle, bool preIncremented = true)
            : base(handle, LibSpotify.sp_album_add_ref_r, LibSpotify.sp_album_release_r, preIncremented)
        {
        }

        #region Properties
        public Artist Artist
        {
            get
            {
                return new Artist(LibSpotify.sp_album_artist_r(Handle), false);
            }
        }

        public bool IsAvailable
        {
            get
            {
                return LibSpotify.sp_album_is_available_r(Handle);
            }
        }

        public bool IsLoaded
        {
            get
            {
                return LibSpotify.sp_album_is_loaded_r(Handle);
            }
        }

        public string Name
        {
            get
            {
                return LibSpotify.ReadUtf8(LibSpotify.sp_album_name_r(Handle));
            }
        }

        public AlbumType Type
        {
            get
            {
                return LibSpotify.sp_album_type_r(Handle);
            }
        }

        public int Year
        {
            get
            {
                return LibSpotify.sp_album_year_r(Handle);
            }
        }
        #endregion

        #region Async Methods
        public Task<Image> LoadCoverAsync(Session session, ImageSize size, object state)
        {
            return Task.Factory.FromAsync<Session, ImageSize, Image>(BeginLoadCover, EndLoadCover, session,
                size, state);
        } 

        public IAsyncResult BeginLoadCover(Session session, ImageSize size, AsyncCallback userCallback,
            object state)
        {
            return Internal.ImageLoader.Begin(LibSpotify.sp_album_cover_r, Handle, session, size,
                userCallback, state);
        }

        public Image EndLoadCover(IAsyncResult result)
        {
            return Internal.ImageLoader.End(result);
        }
        #endregion
    }
}
