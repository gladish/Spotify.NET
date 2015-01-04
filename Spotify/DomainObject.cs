using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Linq;

using Spotify.Internal;

namespace Spotify
{
    public abstract class DomainObject : IDisposable
    {
        private static HashSet<DomainObject> _objectsToDispose = new HashSet<DomainObject>();

        public delegate Error AddRef(IntPtr handle);
        public delegate Error DelRef(IntPtr handle);

        internal DomainObject(IntPtr handle, AddRef addRef, DelRef delRef, bool preIncremented)
        {
            ThrowHelper.ThrowIfNull(addRef, "addRef");
            ThrowHelper.ThrowIfNull(delRef, "delRef");
            ThrowHelper.ThrowIfZero(handle, "handle");

            _addRef = addRef;
            _delRef = delRef;

            if (!preIncremented)
                ThrowHelper.ThrowIfError(_addRef(handle));

            Handle = handle;

            lock (_objectsToDispose)
            {
                _objectsToDispose.Add(this);
            }
        }

        ~DomainObject()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);

            lock (_objectsToDispose)
            {
                _objectsToDispose.Remove(this);
            }
        }

        internal static void DisposeAllRemaingObjects()
        {
            lock (_objectsToDispose)
            {
                // I know this is completely natural, but the Dispose() on the DomainObject will remove
                // the current instance frmo the _objectsToDispose collection.
                // This is here because calling Dipose() on the session before Disposing() other DomainObjects
                // leaves the job to the finalizer. I was getting exceptions from the unmanaged layer when 
                // trying to release unmanaged objects after the session is released. This ensures that all
                // objects are disposed and is invoked from the Session.Dispose()
                while (_objectsToDispose.Count > 0)
                {
                    DomainObject obj = _objectsToDispose.First();
                    obj.Dispose();
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            DelRef release = _delRef;

            if (disposing)
            {
                _delRef = null;
                _addRef = null;
            }
            

            if (release != null && Handle != IntPtr.Zero)
                release(Handle);

            Handle = IntPtr.Zero;
            _disposed = true;
        }

        protected bool IsDisposed
        {
            get
            {
                return _disposed;
            }
        }

        private bool _disposed;
        private AddRef _addRef;
        private DelRef _delRef;

        private IntPtr _handle;
        internal IntPtr Handle
        {
            get
            {
                System.Diagnostics.Debug.Assert(_handle != IntPtr.Zero);

                if (_disposed)
                    throw new ObjectDisposedException("handle");

                return _handle;
            }
            private set
            {
                _handle = value;
            }
        }

        protected delegate int MakeListGetCount(IntPtr p);
        protected delegate IntPtr MakeListGetItem<T>(IntPtr p, int index);
        protected delegate T MakeListConstructor<T>(IntPtr p);

        protected IList<T> MakeList<T>(MakeListConstructor<T> make, MakeListGetCount count, MakeListGetItem<T> get)
        {
            ThrowHelper.ThrowIfNull(count, "count");
            ThrowHelper.ThrowIfNull(get, "get");

            IntPtr p = Handle;

            int n = count(p);
            if (n == 0)
                return new List<T>();

            List<T> items = new List<T>(n);
            for (int i = 0; i < n; ++i)
                items.Add(make(get(p, i)));

            return items;
        }
    }
}
