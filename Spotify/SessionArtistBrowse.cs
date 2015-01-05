using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotify
{
    public partial class Session
    {
        private class AsyncArtistBrowseResult : Internal.AsyncCallbackResult<ArtistBrowse>
        {
            public AsyncArtistBrowseResult(AsyncCallback callback, object state)
                : base(callback, state)
            {
            }

            public void HandleBrowseComplete(IntPtr artistBrowse, IntPtr state)
            {
                if (artistBrowse != IntPtr.Zero)
                    Closure = new ArtistBrowse(artistBrowse);
                SetCallbackComplete();
            }
        }

        public Task<ArtistBrowse> BrowseAristAsync(Artist artist, ArtistBrowseType browseType, object state)
        {
            return Task.Factory.FromAsync<Artist, ArtistBrowseType, ArtistBrowse>(BeginArtistBrowse, EndArtistBrowse,
                artist, browseType, state);
        }

        public IAsyncResult BeginArtistBrowse(Artist artist, ArtistBrowseType browseType, AsyncCallback userCallback, object state)
        {
            AsyncArtistBrowseResult result = new AsyncArtistBrowseResult(userCallback, state);
            LibSpotify.sp_artistbrowse_create_r(Handle, artist.Handle, browseType, result.HandleBrowseComplete, IntPtr.Zero);
            return result;
        }

        public ArtistBrowse EndArtistBrowse(IAsyncResult result)
        {
            AsyncArtistBrowseResult artistBrowseResult = Internal.ThrowHelper.DownCast<AsyncArtistBrowseResult>(result, "result");
            artistBrowseResult.WaitForCallbackComplete();
            artistBrowseResult.SetCompleted(artistBrowseResult.Closure.Error);
            artistBrowseResult.CheckPendingException();
            return artistBrowseResult.Closure;
        }
    }
}
