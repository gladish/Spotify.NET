using System;
using System.Threading.Tasks;

using Spotify.Internal;

namespace Spotify
{
    public partial class Session
    {
        public Task<Search> SearchAsync(SearchParameters searchParams, object stateObject)
        {
            return Task.Factory.FromAsync<SearchParameters, Search>(BeginSearch, EndSearch, searchParams, stateObject);
        }

        public IAsyncResult BeginSearch(SearchParameters searchParams, AsyncCallback userCallback, object stateObject)
        {
            ThrowHelper.ThrowIfNull(searchParams, "searchParams");

            AsyncSearchResult searchResult = new AsyncSearchResult(userCallback, stateObject);

            Search search = new Search(LibSpotify.sp_search_create_r(
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
                IntPtr.Zero));

            // searchResult.SearchComplete may be called before ApiClosure is set
            searchResult.ApiClosure = search;
            return searchResult;
        }

        public Search EndSearch(IAsyncResult result)
        {
            AsyncSearchResult searchResult = ThrowHelper.DownCast<AsyncSearchResult>(result, "result");
            searchResult.WaitForCallbackComplete();

            Search search = (Search)searchResult.ApiClosure;
            searchResult.SetCompleted(search.Error);
            searchResult.CheckPendingException();
            return search;
        }
    }
}
