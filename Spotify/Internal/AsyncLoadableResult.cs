using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotify.Internal
{
    internal class AsyncLoadableResult<TClosure> : AsyncCallbackResult<TClosure> where TClosure : class, IAsyncLoadable
    {
        public delegate Error ErrorDelegate(TClosure closure);

        public AsyncLoadableResult(TClosure closure, ErrorDelegate error, AsyncCallback userCallback, object state)
            : base(userCallback, state)  
        {
            ThrowHelper.ThrowIfNull(closure, "closure");
            ThrowHelper.ThrowIfNull(error, "error");

            _error = error;

            Closure = closure;
            Closure.Loaded += OnLoaded;            
        }

        private void OnLoaded(object sender, EventArgs e)
        {
            SetCallbackComplete();
            SetCompleted(Error);
        }

        private readonly ErrorDelegate _error;
        private void CheckSynchronousCompletion()
        {
            if (Closure.IsLoaded)
            {
                SetCallbackComplete();
                SetCompleted(Error);
            }
        }

        private Error Error
        {
            get
            {
                return _error(Closure);
            }
        }

        public static AsyncLoadableResult<TClosure> Begin(TClosure closure, ErrorDelegate error, AsyncCallback userCallback, object state)
        {
            var result = new AsyncLoadableResult<TClosure>(closure, error, userCallback, state);
            result.CheckSynchronousCompletion();
            return result;
        }

        public TClosure End()
        {
            WaitForCallbackComplete();
            SetCompleted(Error);
            CheckPendingException();
            return Closure;
        }
    }
}