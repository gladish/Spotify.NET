using System;
using System.Threading;

namespace Spotify.Internal
{
    internal class AsyncCallbackResult<TClosure> : AbstractAsyncResult where TClosure : class
    {
        private ManualResetEvent _callbackCompletedEvent = new ManualResetEvent(false);

        public AsyncCallbackResult(AsyncCallback userCallback, object state) :
            base(userCallback, state)
        {
            // empty
        }

        public void WaitForCallbackComplete()
        {
            _callbackCompletedEvent.WaitOne();
            _callbackCompletedEvent.Dispose();
            _callbackCompletedEvent = null;
        }

        public void SetCallbackComplete()
        {
            _callbackCompletedEvent.Set();
            InvokeUserCallack();
        }

        public TClosure Closure
        {
            get;
            set;
        }
    }
}
