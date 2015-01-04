using System;
using System.Threading.Tasks;

namespace Spotify
{
    public partial class Session
    {
        public Task LoginAsync(LoginParameters loginParams, object stateObject)
        {
            return Task.Factory.FromAsync<LoginParameters>(BeginLogin, EndLogin, loginParams, stateObject);
        } 

        public IAsyncResult BeginLogin(LoginParameters loginParams, AsyncCallback userCallback, object stateObject)
        {
            Internal.AsyncLoginResult result = new Internal.AsyncLoginResult(userCallback, stateObject);
            this.OnLoggedIn += result.HandleLoggedIn;

            Internal.ThrowHelper.ThrowIfError(LibSpotify.sp_session_login_r(Handle, loginParams.UserName,
                loginParams.Password, loginParams.RememberMe, IntPtr.Zero));

            return result;
        }

        public void EndLogin(IAsyncResult result)
        {
            Internal.AsyncLoginResult loginResult = Internal.ThrowHelper.DownCast<Internal.AsyncLoginResult>(result, "result");
            loginResult.WaitAndCheckPendingException();
        }
    }
}
