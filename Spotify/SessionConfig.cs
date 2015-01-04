using System;
using System.IO;

namespace Spotify
{
    public class SessionConfig
    {
        public SessionConfig()
        {
            ApiVersion = 12;
            SettingsLocation = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
                "libspotify");
            CacheLocation = Path.GetTempPath();
        }

        public int ApiVersion
        {
            get;
            private set;
        }

        public string CacheLocation
        {
            get;
            set;
        }

        public string SettingsLocation
        {
            get;
            set;
        }

        public byte[] ApplicationKey
        {
            get;
            set;
        }

        public string UserAgent
        {
            get;
            set;
        }

        public object UserData
        {
            get;
            set;
        }

        public bool CompressPlaylists
        {
            get;
            set;
        }

        public bool DontSaveMetadatForPlaylists
        {
            get;
            set;
        }

        public bool InitiallyUnloadPlaylists
        {
            get;
            set;
        }

        public string DeviceId
        {
            get;
            set;
        }

        public string Proxy
        {
            get;
            set;
        }

        public string ProxyUserName
        {
            get;
            set;
        }

        public string ProxyPassword
        {
            get;
            set;
        }

        public string TraceFile
        {
            get;
            set;
        }
    }
}
