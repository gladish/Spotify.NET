using System;
using System.Threading.Tasks;

using Spotify.Internal;

namespace Spotify
{
    public partial class Session
    {
        private class AsyncSearchResult : AsyncCallbackResult<Search>
        {
            public AsyncSearchResult(AsyncCallback userCallback, object state)
                : base(userCallback, state)
            {
            }

            public void SearchComplete(IntPtr search, IntPtr data)
            {
                if (search != IntPtr.Zero)
                    Closure = new Search(search);
                SetCallbackComplete();
            }
        }

        public Task<Search> SearchAsync(SearchParameters searchParams, object state)
        {
            return Task.Factory.FromAsync<SearchParameters, Search>(BeginSearch, EndSearch, searchParams, state);
        }

        public IAsyncResult BeginSearch(SearchParameters searchParams, AsyncCallback userCallback, object state)
        {
            ThrowHelper.ThrowIfNull(searchParams, "searchParams");
            AsyncSearchResult searchResult = new AsyncSearchResult(userCallback, state);

            LibSpotify.sp_search_create_r(
                Handle,
                searchParams.Query,
                searchParams.TrackOffset,
                searchParams.TrackCount,
                searchParams.AlbumOffset,
                searchParams.AlbumCount,
                searchParams.ArtistOffset,
                searchParams.AlbumCount,
                searchParams.PlaylistOffset,
                searchParams.PlaylistCount,
                searchParams.SearchType,
                searchResult.SearchComplete,
                IntPtr.Zero);

            return searchResult;
        }

        public Search EndSearch(IAsyncResult result)
        {            
            AsyncSearchResult searchResult = ThrowHelper.DownCast<AsyncSearchResult>(result, "result");
            searchResult.WaitForCallbackComplete();
            searchResult.SetCompleted(searchResult.Closure.Error);
            searchResult.CheckPendingException();
            return searchResult.Closure;
        }


        // Don't use. I need to come up with a way to cleany fetch the Error property wihtout using reflection/dynamic typing.
        // I can't seem to come up with an Interface name for a class that has a property named 'Error'
        private TClosure BasicEnd<TClosure, TResult>(TResult result) where TResult : AsyncCallbackResult<TClosure>
            where TClosure : class
        {
            TResult basicResult = ThrowHelper.DownCast<TResult>(result, "result");
            basicResult.WaitForCallbackComplete();
            // TODO IClosure needs Error property
            // basicResult.SetCompleted(basicResult.Closure.Error);
            basicResult.CheckPendingException();
            return basicResult.Closure;
        }
    }
}
