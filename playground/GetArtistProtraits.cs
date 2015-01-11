using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace playground
{
    public partial class Program
    {
        private static async Task GetArtistProtraits(Spotify.Session session, string username, string password)
        {
            await session.LoginAsync(new Spotify.LoginParameters() { UserName = username, Password = password }, null);

            // TODO: understand what we must provide in searh
            var query = new Spotify.SearchParameters();
            query.ArtistCount = 10;
            query.Query = "leonard cohen";
            query.SearchType = Spotify.SearchType.Standard;
            query.AlbumCount = 1;

            var search = await session.SearchAsync(query, null);
            var artistBrowse = await session.BrowseAristAsync(search.Artists[0], Spotify.ArtistBrowseType.NoTracks, null);
            var portraits = await artistBrowse.LoadPortraitsAsync(session, null);

            int id = 0;
            foreach (var p in portraits)
            {
                string file = string.Format("c:/Temp/Image{0}.jpg", id++);
                p.ToImage().Save(file);
            }
        }
    }
}
