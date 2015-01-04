using System;

namespace Spotify.Internal
{
    internal static class ImageLoader
    {
        public delegate IntPtr ImageLoad(IntPtr p, ImageSize size);

        public static IAsyncResult Begin(ImageLoad load, IntPtr p, Session session, ImageSize size, 
            AsyncCallback userCallback, object stateObject)
        {
            AsyncLoadImageResult result = new AsyncLoadImageResult(userCallback, stateObject);
            Image image = session.CreateImage(load(p, size));
            result.ApiClosure = image;
            image.OnLoaded += result.HandleImageLoaded;
            return result;
        }

        public static System.Drawing.Image End(IAsyncResult result)
        {
            AsyncLoadImageResult loadImageResult = ThrowHelper.DownCast<AsyncLoadImageResult>(result, "result");
            loadImageResult.WaitForCallbackComplete();

            using (Image image = (Image)loadImageResult.ApiClosure)
            {
                loadImageResult.ApiClosure = null;
                loadImageResult.SetCompleted(image.Error);
                loadImageResult.CheckPendingException();

                // don't Dispose() the MemoryStream, the Image will own it.
                return System.Drawing.Image.FromStream(new System.IO.MemoryStream(image.Data));
            }
        } 
    }
}
