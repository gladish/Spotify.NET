using System;
using System.IO;

namespace Spotify.Diagnostics
{
    public static class Debug
    {
        public static void Print(TextWriter writer, Spotify.Track track, Spotify.Session session = null)
        {
            TimeSpan duration = track.Duration;

            if (session != null)
                writer.WriteLine(" " + session.IsTrackStarred(track));

            writer.WriteLine("Track {0} [{1}] has {2} artist(s), {3}% popularity",
                track.Name,
                track.Duration,
                track.Artists.Count,
                track.Popularity);

            if (track.Disc != 0)
                writer.WriteLine("\t {0} on dics {1}", track.Index, track.Disc);

            foreach (Spotify.Artist artist in track.Artists)
                writer.WriteLine("\t Artist: {0}", artist.Name);
        }

        public static void Print(TextWriter writer, Spotify.Search search, Spotify.Session session = null)
        {
            writer.WriteLine("Query:" + search.Query);
            writer.WriteLine("DidYouMean: " + search.DidYouMean);

            writer.WriteLine("Albums");
            foreach (Spotify.Album album in search.Albums)
                Print(writer, album, session);

            writer.WriteLine("Artists");
            foreach (Spotify.Artist artist in search.Artists)
                Print(writer, artist, session);

            writer.WriteLine("Playlist");
            foreach (Spotify.Playlist playlist in search.Playlists)
                Print(writer, playlist, session);

            writer.WriteLine("Tracks");
            foreach (Spotify.Track track in search.Tracks)
                Print(writer, track, session);
        }

        public static void Print(TextWriter writer, Spotify.Album album, Spotify.Session session = null)
        {
            writer.WriteLine("Name: {0}", album.Name);
            writer.WriteLine("IsAvailable: {0}", album.IsAvailable);
            writer.WriteLine("IsLoaded: {0}", album.IsLoaded);
            writer.WriteLine("Type: {0}", album.Type);
            writer.WriteLine("Year: {0}", album.Year);
        }

        public static void Print(TextWriter writer, Spotify.AudioFormat format)
        {
            writer.WriteLine("AudioFormat[ channels:{0} sampleRate:{1} sampleType:{2}]",
                format.Channels, format.SampleRate, format.SampleType);
        }

        public static void Print(TextWriter writer, Spotify.Artist artist, Spotify.Session session = null)
        {
            // TODO
        }

        public static void Print(TextWriter writer, Spotify.Playlist playlist, Spotify.Session session = null)
        {
            // TODO
        }
    }
}
