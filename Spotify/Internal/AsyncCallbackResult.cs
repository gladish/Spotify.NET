using System;
using System.Threading;

namespace Spotify.Internal
{
    internal class AsyncCallbackResult : AsyncResult
    {
        private ManualResetEvent _callbackCompletedEvent = new ManualResetEvent(false);

        public AsyncCallbackResult(AsyncCallback userCallback, object stateObject) :
            base(userCallback, stateObject)
        {

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

        public object ApiClosure
        {
            get;
            set;
        }
    }
}
