using System;
using Spotify.Internal;

namespace Spotify
{
    public class PlaylistContainer : DomainObject
    {
        public event EventHandler<PlaylistAddedEventArgs> OnPlaylistAdded;
        public event EventHandler OnPlaylistRemoved;
        public event EventHandler OnPlaylistMoved;
        public event EventHandler OnLoaded;

        internal PlaylistContainer(IntPtr handle, bool preIncremented = true)
            : base(handle, LibSpotify.sp_album_add_ref_r, LibSpotify.sp_album_release_r, preIncremented)
        {
            LibSpotify.sp_playlistcontainer_callbacks callbacks = new LibSpotify.sp_playlistcontainer_callbacks();
            callbacks.container_loaded = RaiseContainerLoaded;
            callbacks.playlist_added = RaisePlaylistAdded;
            callbacks.playlist_moved = RaisePlaylistMoved;
            callbacks.playlist_removed = RaisePlaylistRemoved;

            ThrowHelper.ThrowIfError(LibSpotify.sp_playlistcontainer_add_callbacks_r(Handle,
                ref callbacks, IntPtr.Zero));
        }

        private void RaiseContainerLoaded(IntPtr pc, IntPtr userdata)
        {
            EventDispatcher.Dispatch(this, pc, OnLoaded, EventArgs.Empty);
        }

        private void RaisePlaylistAdded(IntPtr pc, IntPtr playlist, int position, IntPtr userdata)
        {
            EventDispatcher.Dispatch(this, pc, OnPlaylistAdded,
                new PlaylistAddedEventArgs(playlist, position));
        }

        private void RaisePlaylistMoved(IntPtr pc, IntPtr playlist, int position, int newPosition, IntPtr userdata)
        {
            EventDispatcher.Dispatch(this, pc, OnPlaylistMoved,
                new PlaylistMovedEventArgs(playlist, position, newPosition));
        }

        private void RaisePlaylistRemoved(IntPtr pc, IntPtr playlist, int position, IntPtr userdata)
        {
            EventDispatcher.Dispatch(this, pc, OnPlaylistRemoved,
                new PlaylistRemovedEventArgs(playlist, position));
        }
    }
}
