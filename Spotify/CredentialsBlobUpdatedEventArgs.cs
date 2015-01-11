using System;

namespace Spotify
{
    public class CredentialsBlobUpdatedEventArgs : EventArgs
    {
        internal CredentialsBlobUpdatedEventArgs(string s)
        {
            Blob = s;
        }
        public readonly string Blob;
    }
}
