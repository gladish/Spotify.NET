using System;


namespace Spotify.Internal
{
    internal class AsyncLoadImageResult : AsyncCallbackResult<Image>
    {
        public AsyncLoadImageResult(AsyncCallback userCallback, object state)
            : base(userCallback, state)
        {
        }

        public void HandleImageLoaded(object sender, EventArgs e)
        {
            SetCallbackComplete();
        }
    }
}
