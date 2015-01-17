using System;
using System.Collections.Generic;
using Spotify.Internal;

namespace Spotify
{
    public sealed class Search : DomainObject
    {
        internal Search(IntPtr handle, bool preIncremented = true)
            : base(handle, LibSpotify.sp_search_add_ref_r, LibSpotify.sp_search_release_r, preIncremented)
        {
        }

        public Error Error
        {
            get
            {
                return LibSpotify.sp_search_error_r(Handle);
            }
        }

        public string Query
        {
            get
            {
                return LibSpotify.ReadUtf8(LibSpotify.sp_search_query_r(Handle));
            }
        }

        public string DidYouMean
        {
            get
            {
                return LibSpotify.ReadUtf8(LibSpotify.sp_search_did_you_mean_r(Handle));
            }
        }

        public int TotalTracks
        {
            get
            {
                return LibSpotify.sp_search_total_tracks_r(Handle);
            }
        }

        public int TotalPlaylists
        {
            get
            {
                return LibSpotify.sp_search_total_playlists_r(Handle);
            }
        }

        public int TotalArtists
        {
            get
            {
                return LibSpotify.sp_search_total_artists_r(Handle);
            }
        }

        public int TotalAlbums
        {
            get
            {
                return LibSpotify.sp_search_total_albums_r(Handle);
            }
        }

        public SearchParameters Page(SearchParameters searchParams)
        {
            searchParams.AlbumCount = searchParams.AlbumCount;
            searchParams.AlbumOffset = searchParams.AlbumOffset + NumAlbums;
            searchParams.ArtistCount = searchParams.ArtistCount;
            searchParams.ArtistOffset = searchParams.ArtistOffset + NumArtists;
            searchParams.PlaylistCount = searchParams.PlaylistCount;
            searchParams.PlaylistOffset = searchParams.PlaylistOffset + NumPlaylists;
            searchParams.TrackCount = searchParams.TrackCount;
            searchParams.TrackOffset = searchParams.TrackOffset + NumTracks;
            return searchParams;
        }

        public IList<Track> Tracks
        {
            get
            {
                return MakeList(p => { return new Track(p, false); },
                    LibSpotify.sp_search_num_tracks_r, LibSpotify.sp_search_track_r);
            }
        }

        public IList<Album> Albums
        {
            get
            {
                return MakeList(p => { return new Album(p, false); },
                    LibSpotify.sp_search_num_albums_r, LibSpotify.sp_search_album_r);
            }
        }

        public IList<Artist> Artists
        {
            get
            {
                return MakeList(p => { return new Artist(p, false); },
                    LibSpotify.sp_search_num_artists_r, LibSpotify.sp_search_artist_r);
            }
        }

        public IList<Playlist> Playlists
        {
            get
            {
                return MakeList(p => { return new Playlist(p, false); },
                    LibSpotify.sp_search_num_playlists_r, LibSpotify.sp_search_playlist_r);
            }
        }

        public bool IsLoaded
        {
            get
            {
                return LibSpotify.sp_search_is_loaded_r(Handle);
            }
        }

        internal int NumTracks
        {
            get
            {
                return LibSpotify.sp_search_num_tracks_r(Handle);
            }
        }

        internal int NumArtists
        {
            get
            {
                return LibSpotify.sp_search_num_artists_r(Handle);
            }
        }

        public int NumAlbums
        {
            get
            {
                return LibSpotify.sp_search_num_albums_r(Handle);
            }
        }

        internal int NumPlaylists
        {
            get
            {
                return LibSpotify.sp_search_num_playlists_r(Handle);
            }
        }
    }
}
