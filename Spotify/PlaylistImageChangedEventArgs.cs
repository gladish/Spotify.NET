using System;
using System.Threading.Tasks;

namespace Spotify
{
    using Spotify.Internal;

    public class PlaylistImageChangedEventArgs : EventArgs
    {
        internal PlaylistImageChangedEventArgs(IntPtr imageId)
        {
            ImageId = imageId;
        }

        #region Async Methods
        public Task<Image> LoadImageAsync(Session session, AsyncCallback userCallback, object state)
        {
            return Task.Factory.FromAsync<Session, Image>(BeginLoadImage, EndLoadImage, session, state);
        }

        public IAsyncResult BeginLoadImage(Session session, AsyncCallback userCallback, object state)
        {
            return ImageLoader.Begin((p, s) => { return p; }, ImageId, session, ImageSize.Normal, userCallback, state);
        }

        public Image EndLoadImage(IAsyncResult result)
        {
            return ImageLoader.End(result);
        }
        #endregion

        private readonly IntPtr ImageId;
    }
}
