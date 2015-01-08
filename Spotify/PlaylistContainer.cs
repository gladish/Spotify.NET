using System;
using System.Collections.Generic;
using System.Text;

using Spotify.Internal;

namespace Spotify
{
    public class PlaylistContainer : DomainObject
    {
        #region Events
        public event EventHandler<PlaylistAddedEventArgs> PlaylistAdded;
        public event EventHandler<PlaylistRemovedEventArgs> PlaylistRemoved;
        public event EventHandler PlaylistMoved;
        public event EventHandler Loaded;
        #endregion

        internal PlaylistContainer(IntPtr handle, bool preIncremented = true)
            : base(handle, LibSpotify.sp_playlistcontainer_add_ref_r, LibSpotify.sp_playlistcontainer_release_r, preIncremented)
        {
            LibSpotify.sp_playlistcontainer_callbacks callbacks = new LibSpotify.sp_playlistcontainer_callbacks();
            callbacks.playlist_added = OnPlaylistAdded;
            callbacks.playlist_removed = OnPlaylistRemoved;
            callbacks.playlist_moved = OnPlaylistMoved;
            callbacks.container_loaded = OnContainerLoaded;

            ThrowHelper.ThrowIfError(LibSpotify.sp_playlistcontainer_add_callbacks_r(Handle,
                ref callbacks, IntPtr.Zero));
        }


        #region Properties
        public User Owner
        {
            get
            {
                IntPtr p = LibSpotify.sp_playlistcontainer_owner_r(Handle);
                return p != IntPtr.Zero ? new User(p, false) : null;
            }
        }
        
        public IList<Playlist> Playlists
        {
            get
            {
                IList<Playlist> list =  MakeList(p => { 
                    return new Playlist(p, false); }, 
                    LibSpotify.sp_playlistcontainer_num_playlists_r,
                    LibSpotify.sp_playlistcontainer_playlist_r);

                for (int i = 0; i < list.Count; ++i)
                {
                    list[i].ListType = LibSpotify.sp_playlistcontainer_playlist_type_r(Handle, i);
                    list[i].Index = i;
                }

                return list;
            }
        }

        public bool IsLoaded
        {
            get
            {
                return LibSpotify.sp_playlistcontainer_is_loaded_r(Handle);
            }
        }
        #endregion

        public IList<Track> GetUnseenTracks(Playlist playlist)
        {
            IntPtr[] unseen = new IntPtr[1024];

            int n = LibSpotify.sp_playlistcontainer_get_unseen_tracks_r(Handle, playlist.Handle, unseen, unseen.Length);
            System.Diagnostics.Debug.Assert(n < unseen.Length);

            List<Track> tracks = new List<Track>();

            for (int i = 0; i < n; ++i)
                tracks.Add(new Track(unseen[i], false));

            return tracks;
        }

        public void ClearUnseenTracks(Playlist playList)
        {
            int ret = LibSpotify.sp_playlistcontainer_clear_unseen_tracks_r(Handle, playList.Handle);
            if (ret == -1)
                throw new InvalidOperationException("failed to clear unseen tracks"); // why?
        }

        public string GetPlaylistFolderName(Playlist playList)
        {
            StringBuilder builder = new StringBuilder(512);

            ThrowHelper.ThrowIfError(LibSpotify.sp_playlistcontainer_playlist_folder_name_r(Handle, playList.Index,
                builder, builder.Capacity));

            return builder.ToString();
        }

        public long GetPlaylistFolderId(Playlist playList)
        {
            // TODO: Does this really need uint64? 
            ulong id = LibSpotify.sp_playlistcontainer_playlist_folder_id_r(Handle, playList.Index);
            return Convert.ToInt64(id);
        }

        public Playlist AddNewPlaylist(string name)
        {
            // TODO: See docs about ref-counting, does this need to be ref'd or is it already ref'd
            return new Playlist(LibSpotify.sp_playlistcontainer_add_new_playlist_r(Handle, name), false);
        }

        public Playlist AddNewPlaylist(Link link)
        {
            return new Playlist(LibSpotify.sp_playlistcontainer_add_playlist_r(Handle, link.Handle), false);
        }

        public void RemovePlaylist(int index)
        {
            ThrowHelper.ThrowIfError(LibSpotify.sp_playlistcontainer_remove_playlist_r(Handle, index));
        }

        public void MovePlaylist(int index, int newPosition, bool dryRun)
        {
            ThrowHelper.ThrowIfError(LibSpotify.sp_playlistcontainer_move_playlist_r(Handle, index, newPosition, dryRun));
        }

        public void AddFolder(int index, string name)
        {
            ThrowHelper.ThrowIfError(LibSpotify.sp_playlistcontainer_add_folder_r(Handle, index, name));
        }
        

        #region Private Methods
        private void OnContainerLoaded(IntPtr playlistContainer, IntPtr state)
        {
            EventDispatcher.Dispatch(this, playlistContainer, Loaded, EventArgs.Empty);
        }

        private void OnPlaylistAdded(IntPtr playlistContainer, IntPtr playlist, int position, IntPtr state)
        {
            EventDispatcher.Dispatch(this, playlistContainer, PlaylistAdded,
                new PlaylistAddedEventArgs(playlist, position));
        }

        private void OnPlaylistMoved(IntPtr playlistContainer, IntPtr playlist, int position, int newPosition, IntPtr state)
        {
            EventDispatcher.Dispatch(this, playlistContainer, PlaylistMoved,
                new PlaylistMovedEventArgs(playlist, position, newPosition));
        }

        private void OnPlaylistRemoved(IntPtr playlistContainer, IntPtr playlist, int position, IntPtr state)
        {
            EventDispatcher.Dispatch(this, playlistContainer, PlaylistRemoved,
                new PlaylistRemovedEventArgs(playlist, position));
        }
        #endregion
    }
}
