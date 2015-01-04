using System;
using System.Threading;

namespace Spotify.Internal
{
    internal class AsyncSearchResult : AsyncCallbackResult
    {
        public AsyncSearchResult(AsyncCallback userCallback, object stateObject)
            : base(userCallback, stateObject)
        {
        }

        public void SearchComplete(IntPtr search, IntPtr data)
        {
            SetCallbackComplete();
        }
    }
}
