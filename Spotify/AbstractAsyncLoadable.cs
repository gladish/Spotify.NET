using System;

namespace Spotify
{
    public abstract class AbstractAsyncLoadable : DomainObject
    {
        #region Events
        public abstract event EventHandler Loaded;
        #endregion

        internal AbstractAsyncLoadable(IntPtr handle, AddRef addRef, DelRef delRef, bool preIncremented)
            : base(handle, addRef, delRef, preIncremented)
        {
        }

        public abstract bool IsLoaded
        {
            get;
        }

        internal void RegisterLoadedHandler(EventHandler h)
        {
            Loaded += h;
        }
    }
}
