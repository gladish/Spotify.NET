using System;


namespace Spotify.Internal
{
    internal class AsyncLoginResult : AsyncResult
    {
        public AsyncLoginResult(AsyncCallback userCallback, object stateObject)
            : base(userCallback, stateObject)
        {
        }

        public void HandleLoggedIn(object sender, LoggedInEventArgs e)
        {
            SetCompleted(e.ErrorCode);
            InvokeUserCallack();
        }
    }
}
