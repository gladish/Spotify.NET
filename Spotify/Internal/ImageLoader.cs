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

    internal static class ImageLoader
    {
        public delegate IntPtr ImageLoad(IntPtr p, ImageSize size);

        public static IAsyncResult Begin(ImageLoad load, IntPtr p, Session session, ImageSize size,
            AsyncCallback userCallback, object state)
        {
            AsyncLoadImageResult result = new AsyncLoadImageResult(userCallback, state);
            Image image = new Image(LibSpotify.sp_image_create_r(session.Handle, load(p, size)));
            result.Closure = image;
            image.Loaded += result.HandleImageLoaded;

            // It's possible the image loaded before we registered the result.HandleImageLoaded
            if (image.IsLoaded)
            {
                result.CompletedSynchronously = true;
                result.SetCompleted(image.Error);
            }

            return result;
        }

        public static Image End(IAsyncResult result)
        {
            AsyncLoadImageResult loadImageResult = ThrowHelper.DownCast<AsyncLoadImageResult>(result, "result");
            loadImageResult.WaitForCallbackComplete();            
            loadImageResult.SetCompleted(loadImageResult.Closure.Error);
            loadImageResult.CheckPendingException();
            return loadImageResult.Closure;        
        }
    }
}
