using System;
using System.Collections.Generic;
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


        private class BrowseResult : Internal.AsyncCallbackResult<AlbumBrowse>
        {
            public BrowseResult(AsyncCallback userCallback, object state) : base(userCallback, state)
            { }
           
            public void Complete(IntPtr albumBrowse, IntPtr data)
            {
                if (albumBrowse != IntPtr.Zero)
                    Closure = new AlbumBrowse(albumBrowse);
                SetCallbackComplete();
            }
        }

        public Task<AlbumBrowse> BrowseAsync(Session session, object state)
        {
            return Task.Factory.FromAsync<Session, AlbumBrowse>(BeginBrowse, EndBrowse, session, state);
        }

        public IAsyncResult BeginBrowse(Session session, AsyncCallback userCallback, object state)
        {
            Internal.ThrowHelper.ThrowIfNull(session, "session");
            Internal.ThrowHelper.ThrowIfNull(userCallback, "userCallback");

            BrowseResult browseResult = new BrowseResult(userCallback, state);
            LibSpotify.sp_albumbrowse_create_r(session.Handle, Handle, browseResult.Complete, IntPtr.Zero);

            return browseResult;
        }

        public AlbumBrowse EndBrowse(IAsyncResult result)
        {
            BrowseResult browseResult = Internal.ThrowHelper.DownCast<BrowseResult>(result, "result");
            browseResult.WaitForCallbackComplete();
            browseResult.SetCompleted(browseResult.Closure.Error);
            browseResult.CheckPendingException();
            return browseResult.Closure;
        }

        #endregion
    }
}
