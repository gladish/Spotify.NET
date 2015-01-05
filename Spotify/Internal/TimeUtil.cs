using System;


namespace Spotify.Internal
{
    internal static class TimeUtil
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime FromUnixTimeSeconds(int seconds)
        {
            return UnixEpoch.AddSeconds(seconds);
        }
    }
}
