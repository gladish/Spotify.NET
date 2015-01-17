using System;

namespace Spotify
{
    interface IAsyncLoadable
    {
        event EventHandler Loaded;

        bool IsLoaded
        {
            get;
        }
    }
}
