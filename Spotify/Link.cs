using System;
using Spotify.Internal;

namespace Spotify
{
    public sealed class Link : DomainObject
    {
        internal Link(IntPtr handle, bool preIncremented = true)
            : base(handle, LibSpotify.sp_link_add_ref_r, LibSpotify.sp_link_release_r, preIncremented)
        {

        }

        public override string ToString()
        {
            return LibSpotify.ReadUtf8(Handle, LibSpotify.sp_link_as_string_r);
        }

        public Uri ToHttp()
        {
            string s = ToString();

            // spotify:track:63eDWEPplirGz51yew1vFt
            if (s.StartsWith("spotify"))
                s = s.Substring(7);

            s = s.Replace(':', '/');
            s = Environment.SpotifyOpenURL + s;
            return new Uri(s, UriKind.Absolute);
        }

        public LinkType LinkType
        {
            get
            {
                return LibSpotify.sp_link_type_r(Handle);
            }
        }

        public Track ToTrack()
        {
            ThrowHelper.AssertLinkConverstion(LinkType, LinkType.Track);
            return new Track(LibSpotify.sp_link_as_track_r(Handle), false);
        }

        public Track ToTrack(ref TimeSpan offset)
        {
            ThrowHelper.AssertLinkConverstion(LinkType, LinkType.Track);

            int n = 0;
            Track t = new Track(LibSpotify.sp_link_as_track_and_offset_r(Handle, ref n), false);
            offset = TimeSpan.FromMilliseconds(n);
            return t;
        }

        public Album ToAlbum()
        {
            ThrowHelper.AssertLinkConverstion(LinkType, LinkType.Album);
            return new Album(LibSpotify.sp_link_as_album_r(Handle), false);
        }

        public Artist ToArtist()
        {
            ThrowHelper.AssertLinkConverstion(LinkType, LinkType.Artist);
            return new Artist(LibSpotify.sp_link_as_artist_r(Handle), false);
        }

        public User ToUser()
        {
            // There doesn't appear to be a linktype user
            return new User(LibSpotify.sp_link_as_user_r(Handle), false);
        }

        public static Link FromLink(string link)
        {
            return new Link(LibSpotify.sp_link_create_from_string_r(link));
        }

        public static Link FromTrack(Track track, TimeSpan offset)
        {
            ThrowHelper.ThrowIfNull(track, "track");
            return new Link(LibSpotify.sp_link_create_from_track_r(track.Handle,
                Convert.ToInt32(offset.TotalMilliseconds)));
        }

        public static Link FromTrack(Track track)
        {
            ThrowHelper.ThrowIfNull(track, "track");
            return new Link(LibSpotify.sp_link_create_from_track_r(track.Handle, 0));
        }

        public static Link FromAlbum(Album album)
        {
            ThrowHelper.ThrowIfNull(album, "album");
            return new Link(LibSpotify.sp_link_create_from_album_r(album.Handle));
        }

        public static Link FromAlbumCover(Album album, ImageSize size)
        {
            ThrowHelper.ThrowIfNull(album, "album");
            return new Link(LibSpotify.sp_link_create_from_album_cover_r(album.Handle, size));
        }

        public static Link FromArtist(Artist artist)
        {
            ThrowHelper.ThrowIfNull(artist, "artist");
            return new Link(LibSpotify.sp_link_create_from_artist_r(artist.Handle));
        }

        public static Link FromArtistPortrait(Artist artist, ImageSize size)
        {
            ThrowHelper.ThrowIfNull(artist, "artist");
            return new Link(LibSpotify.sp_link_create_from_artist_portrait_r(artist.Handle, size));
        }

        public static Link FromArtistBrowsePortrait(ArtistBrowse artistBrowse, int index)
        {
            ThrowHelper.ThrowIfNull(artistBrowse, "artistBrowse");
            return new Link(LibSpotify.sp_link_create_from_artistbrowse_portrait_r(artistBrowse.Handle, index));
        }

        public static Link FromSearch(Search search)
        {
            ThrowHelper.ThrowIfNull(search, "search");
            return new Link(LibSpotify.sp_link_create_from_search_r(search.Handle));
        }

        public static Link FromPlaylist(Playlist playlist)
        {
            ThrowHelper.ThrowIfNull(playlist, "playlist");
            return new Link(LibSpotify.sp_link_create_from_playlist_r(playlist.Handle));
        }

        public static Link FromUser(User user)
        {
            ThrowHelper.ThrowIfNull(user, "user");
            return new Link(LibSpotify.sp_link_create_from_user_r(user.Handle));
        }

        /*
        public static Link FromImage(Image image)
        {
            ThrowHelper.ThrowIfNull(image, "image");
            return new Link(LibSpotify.sp_link_create_from_image_r(image.Handle));
        }
         */
    }
}
