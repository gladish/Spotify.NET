using System;
using System.Runtime.InteropServices;

namespace Spotify
{
    [Flags]
    public enum ConnectionRules
    {
        Network = 1,
        NetworkIfRoaming = 2,
        AllowSyncOverMobile = 4,
        AllowSyncOverWifi = 8
    }
}
