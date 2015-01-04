using System;


namespace Spotify.Internal
{
    internal class AsyncLoadImageResult : AsyncCallbackResult
    {
        public AsyncLoadImageResult(AsyncCallback userCallback, object stateObject)
            : base(userCallback, stateObject)
        {
        }

        public void HandleImageLoaded(object sender, EventArgs e)
        {
            SetCallbackComplete();
        }
    }
}
