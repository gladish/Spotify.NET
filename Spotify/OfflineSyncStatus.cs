using System;

namespace Spotify
{
    public class OfflineSyncStatus
    {
        public int QueuedTracks
        {
            get;
            set;
        }

        public long QueuedBytes
        {
            get;
            set;
        }

        public int DoneTracks
        {
            get;
            set;
        }

        public long DoneBytes
        {
            get;
            set;
        }

        public int CopiedTracks
        {
            get;
            set;
        }

        public long CopiedBytes
        {
            get;
            set;
        }

        public int WillNotCopyTracks
        {
            get;
            set;
        }

        public int ErorrTracks
        {
            get;
            set;
        }

        public bool Syncing
        {
            get;
            set;
        }
    }
}
