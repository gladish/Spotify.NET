using System;
using System.Threading.Tasks;

using Spotify.Internal;

namespace Spotify
{
    public partial class Session
    {
        private class AsyncLoginResult : AbstractAsyncResult
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

        public Task LoginAsync(LoginParameters loginParams, object stateObject)
        {
            return Task.Factory.FromAsync<LoginParameters>(BeginLogin, EndLogin, loginParams, stateObject);
        } 

        public IAsyncResult BeginLogin(LoginParameters loginParams, AsyncCallback userCallback, object stateObject)
        {
            AsyncLoginResult result = new AsyncLoginResult(userCallback, stateObject);
            this.LoggedIn += result.HandleLoggedIn;

            ThrowHelper.ThrowIfError(LibSpotify.sp_session_login_r(Handle, loginParams.UserName,
                loginParams.Password, loginParams.RememberMe, IntPtr.Zero));

            return result;
        }

        public void EndLogin(IAsyncResult result)
        {
            AsyncLoginResult loginResult = ThrowHelper.DownCast<AsyncLoginResult>(result, "result");
            loginResult.WaitAndCheckPendingException();
        }
    }
}
