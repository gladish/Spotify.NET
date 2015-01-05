using System;

namespace Spotify
{
    public class CredentialsBlobUpdatedEventArgs : EventArgs
    {
        public CredentialsBlobUpdatedEventArgs(string s)
        {
            Blob = s;
        }
        public readonly string Blob;
    }
}
