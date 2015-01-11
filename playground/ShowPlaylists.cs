using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace playground
{
    public partial class Program
    {
        private static async Task ShowPlaylists(Spotify.Session session, string username, string password)
        {
            await session.LoginAsync(new Spotify.LoginParameters() { UserName = username, Password = password }, null);
            var container = await session.LoadPlaylistContainerAsync(null);
        }
    }
}
