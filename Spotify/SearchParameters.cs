using System;

namespace Spotify
{
    public class SearchParameters
    {
        public static SearchParameters WhatsNew
        {
            get
            {
                return new SearchParameters() { Query = "tag:new" };
            }
        }

        public string Query
        {
            get;
            set;
        }

        public int TrackOffset
        {
            get;
            set;
        }

        public int TrackCount
        {
            get;
            set;
        }

        public int AlbumOffset
        {
            get;
            set;
        }

        public int AlbumCount
        {
            get;
            set;
        }

        public int ArtistOffset
        {
            get;
            set;
        }

        public int ArtistCount
        {
            get;
            set;
        }

        public int PlaylistOffset
        {
            get;
            set;
        }

        public int PlaylistCount
        {
            get;
            set;
        }

        public SearchType SearchType
        {
            get;
            set;
        }
    }
}
