﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Spotify.Internal
{
    internal abstract class AbstractAsyncResult : IAsyncResult
    {
        private static HashSet<IAsyncResult> _outstandingOperations = new HashSet<IAsyncResult>();

        private object _lock = new object();
        private ManualResetEvent _waitHandle = new ManualResetEvent(false);
        private AsyncCallback _userCallback;

        public AbstractAsyncResult(AsyncCallback userCallback, object state)
        {
            _userCallback = userCallback;
            AsyncState = state;

            lock (_outstandingOperations)
            {
                _outstandingOperations.Add(this);
            }
        }

        public void SetCompleted(Error code)
        {            
            Exception ex = null;
            if (code != Error.Ok)
                ex = new Spotify.Exception(code);
            SetCompleted(ex);                        
        }

        public void SetCompleted(System.Exception pendingException)
        {
            PendingException = pendingException;

            Exception ex = null;

            lock (_lock)
            {              
                try
                {
                    IsCompleted = true;
                    _waitHandle.Set();
                }
                catch (Exception err)
                {
                    ex = err;
                }
            }

            if (ex != null)
                Environment.RaiseUnhandledException(this, ex, false);
        }       

        public void CheckPendingException()
        {
            lock (_lock)
            {
                if (PendingException != null)
                    throw PendingException;
            }
        }

        public void WaitAndCheckPendingException()
        {
            AsyncWaitHandle.WaitOne();
            CheckPendingException();
        }

        public object AsyncState
        {
            get;
            set;
        }

        public WaitHandle AsyncWaitHandle
        {
            get
            {
                return _waitHandle;
            }
        }

        public bool CompletedSynchronously
        {
            get;
            internal set;
        }

        public bool IsCompleted
        {
            get;
            set;
        }

        public System.Exception PendingException
        {
            get;
            private set;
        }

        protected void InvokeUserCallack()
        {
            lock (_outstandingOperations)
            {
                bool removed = _outstandingOperations.Remove(this);
                System.Diagnostics.Debug.Assert(removed);
            }

            if (_userCallback != null)
            {
                Task.Run(() => { _userCallback(this); });
            }
        }
    }
}
