﻿using System;

namespace Spotify
{
    public enum ConnectionState
    {
        LoggedOut = 0,
        LoggedIn = 1,
        Disconnected = 2,
        Undefined = 3,
        Offline = 4
    }
}
