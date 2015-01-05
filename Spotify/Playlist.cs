using System;
using System.Collections.Generic;
using System.Linq;

using Spotify.Internal;

namespace Spotify
{
    public class Playlist : DomainObject
    {
        #region Events
        public event EventHandler<PlaylistTracksAddedEventArgs> OnTracksAdded;
        public event EventHandler<PlaylistTracksRemovedEventArgs> OnTracksRemoved;
        public event EventHandler<PlaylistTracksMovedEventArgs> OnTracksMoved;
        public event EventHandler OnRenamed;
        public event EventHandler OnStateChanged;
        public event EventHandler<PlaylistUpdateInProgrssEventArgs> OnUpdateInProgress;
        public event EventHandler OnMetadataUpdated;
        public event EventHandler<PlaylistTrackCreatedChangedEventArgs> OnTrackCreatedChanged;
        public event EventHandler<PlaylistTrackSeenChangedEventArgs> OnTrackSeenChanged;
        public event EventHandler<PlaylistDescriptionChangedEventArgs> OnDescriptionChanged; 

        // TODO ImageChanged

        public event EventHandler OnSubscribersChanged;
        public event EventHandler<PlaylistTrackMessageChangedEventArgs> OnTrackMessageChanged;
        #endregion

        internal Playlist(IntPtr handle, bool preIncremented = true)
            : base(handle, LibSpotify.sp_playlist_add_ref_r, LibSpotify.sp_playlist_release_r, preIncremented)
        {
            _callbacks = new LibSpotify.sp_playlist_callbacks();
            _callbacks.description_changed = RaiseDescriptionChanged;
            _callbacks.playlist_metadata_updated = RaiseMetadataUpdated;
            _callbacks.playlist_renamed = RaiseRenamed;
            _callbacks.playlist_state_changed = RaiseStateChanged;
            _callbacks.playlist_update_in_progress = RaiseUpdateInProgress;
            _callbacks.subscribers_changed = RaiseSubscribersChanged;
            _callbacks.track_created_changed = RaiseCreatedChanged;
            _callbacks.track_message_changed = RaiseMessageChanged;
            _callbacks.track_seen_changed = RaiseSeenChanged;
            _callbacks.tracks_added = RaiseTracksAdded;
            _callbacks.tracks_moved = RaiseTracksMoved;
            _callbacks.tracks_removed = RaiseTracksRemoved;
            
            ThrowHelper.ThrowIfError(LibSpotify.sp_playlist_add_callbacks_r(Handle, ref _callbacks, IntPtr.Zero));
        }

        #region Properties
        public PlaylistType ListType
        {
            get;
            internal set;
        }

        public bool IsLoaded
        {
            get
            {
                return LibSpotify.sp_playlist_is_loaded_r(Handle);
            }
        }

        public IList<Track> Tracks
        {
            get
            {
                return MakeList<Track>(p => { return new Track(p, false); }, LibSpotify.sp_playlist_num_tracks_r,
                    LibSpotify.sp_playlist_track_r);
            }
        }

        public string Name
        {
            get
            {
                return LibSpotify.ReadUtf8(LibSpotify.sp_playlist_name_r(Handle));
            }
        }

        public int Index
        {
            get;
            internal set;
        }

        public User Owner
        {
            get
            {
                return new User(LibSpotify.sp_playlist_owner_r(Handle), false);
            }
        }

        public bool IsCollaborative
        {
            get
            {
                return LibSpotify.sp_playlist_is_collaborative_r(Handle);
            }
            set
            {
                ThrowHelper.ThrowIfError(LibSpotify.sp_playlist_set_collaborative_r(Handle, value));
            }
        }

        public bool AutolinkTracks
        {
            set
            {
                ThrowHelper.ThrowIfError(LibSpotify.sp_playlist_set_autolink_tracks_r(Handle, value));
            }
        }

        // TODO sp_playlist_get_image???

        public bool HasPendingChanges
        {
            get
            {
                return LibSpotify.sp_playlist_has_pending_changes_r(Handle);
            }
        }

        public string Description
        {
            get
            {
                return LibSpotify.ReadUtf8(LibSpotify.sp_playlist_get_description_r(Handle));
            }
        }

        public int NumSubscribers
        {
            get
            {
                return Convert.ToInt32(LibSpotify.sp_playlist_num_subscribers_r(Handle));
            }
        }

        public IList<string> Subscribers
        {
            get
            {
                return LibSpotify.GetPlaylistSubscribers(Handle);
            }
        }
        #endregion

        #region Public Methods
        public bool IsInRam(Session session)
        {
            return LibSpotify.sp_playlist_is_in_ram_r(session.Handle, Handle);
        }

        public PlaylistOfflineStatus GetOfflineStatus(Session session)
        {
            return LibSpotify.sp_playlist_get_offline_status_r(session.Handle, Handle);
        }

        public int GetOfflineDownloadCompleted(Session session)
        {
            return LibSpotify.sp_playlist_get_offline_download_completed_r(session.Handle, Handle);
        }

        public void SetIsInRam(Session session, bool inRam)
        {
            ThrowHelper.ThrowIfError(LibSpotify.sp_playlist_set_in_ram_r(session.Handle, Handle, inRam));
        }

        public void SetOfflineMode(Session session, bool offline)
        {
            ThrowHelper.ThrowIfError(LibSpotify.sp_playlist_set_offline_mode_r(session.Handle, Handle, offline));
        }

        public void UpdateSubscribers(Session session)
        {
            ThrowHelper.ThrowIfError(LibSpotify.sp_playlist_update_subscribers_r(session.Handle, Handle));
        }

        public void AddTracks(Session session, int position, IEnumerable<Track> tracks)
        {
            IntPtr [] pointers = tracks.Select(t => t.Handle).ToArray();
            ThrowHelper.ThrowIfError(
                LibSpotify.sp_playlist_add_tracks_r(Handle, pointers, pointers.Length, position, session.Handle));
        }

        public void RemoveTracks(IEnumerable<Track> tracks)
        {
            int [] indices = tracks.Select(t => t.Index).ToArray();
            ThrowIfDuplicates(indices);

            ThrowHelper.ThrowIfError(
                LibSpotify.sp_playlist_remove_tracks_r(Handle, indices, indices.Length));
        }

        public void ReorderTracks(IEnumerable<Track> tracks, int position)
        {
            int[] indices = tracks.Select(t => t.Index).ToArray();
            ThrowIfDuplicates(indices);

            ThrowHelper.ThrowIfError(
                LibSpotify.sp_playlist_reorder_tracks_r(Handle, indices, indices.Length, position)); 
        }

        public void Rename(string name)
        {
            ThrowHelper.ThrowIfError(LibSpotify.sp_playlist_rename_r(Handle, name));
        }

        public DateTime GetTrackCreateTime(Track t)
        {
            return TimeUtil.FromUnixTimeSeconds(LibSpotify.sp_playlist_track_create_time_r(Handle, t.Index));
        }

        public User GetTrackCreator(Track t)
        {
            return new User(LibSpotify.sp_playlist_track_creator_r(Handle, t.Index), false);
        }

        public bool HasTrackBeenSeen(Track t)
        {
            return LibSpotify.sp_playlist_track_seen_r(Handle, t.Index);
        }

        public void SetTrackSeen(Track t, bool seen)
        {
            ThrowHelper.ThrowIfError(LibSpotify.sp_playlist_track_set_seen_r(Handle, t.Index, seen));
        }

        public string GetTrackMessage(Track t)
        {
            return LibSpotify.ReadUtf8(LibSpotify.sp_playlist_track_message_r(Handle, t.Index));
        }

        #endregion

        #region Private Methods
        private static void ThrowIfDuplicates(IEnumerable<int> e)
        {
            // check for duplicates
            var query = e.GroupBy(index => index)
                .Where(g => g.Count() > 1)
                .Select(k => k.Key)
                .ToList();

            if (query.Count > 0)
                throw new ArgumentException("tracks enumerable contains duplicate values[{0}]",
                    string.Join(",", query.ToList()));
        }

        private void RaiseDescriptionChanged(IntPtr playlist, IntPtr description, IntPtr state)
        {
            Internal.EventDispatcher.Dispatch(this, playlist, OnDescriptionChanged,
                new PlaylistDescriptionChangedEventArgs(LibSpotify.ReadUtf8(description)));
        }

        private void RaiseMetadataUpdated(IntPtr playlist, IntPtr state)
        {
            Internal.EventDispatcher.Dispatch(this, playlist, OnMetadataUpdated, EventArgs.Empty);
        }

        private void RaiseRenamed(IntPtr playlist, IntPtr state)
        {
            Internal.EventDispatcher.Dispatch(this, playlist, OnRenamed, EventArgs.Empty);
        }

        private void RaiseStateChanged(IntPtr playlist, IntPtr state)
        {
            Internal.EventDispatcher.Dispatch(this, playlist, OnStateChanged, EventArgs.Empty);
        }

        private void RaiseUpdateInProgress(IntPtr playlist, bool done, IntPtr state)
        {
            Internal.EventDispatcher.Dispatch(this, playlist, OnUpdateInProgress,
                new PlaylistUpdateInProgrssEventArgs(done));
        }

        private void RaiseTracksAdded(IntPtr playlist, IntPtr[] tracks, int numTracks, int position, IntPtr state)
        {
            List<Track> list = new List<Track>();
            for (int i = 0; i < numTracks; ++i)
                list.Add(new Track(tracks[i], false));

            Internal.EventDispatcher.Dispatch(this, playlist, OnTracksAdded,
                new PlaylistTracksAddedEventArgs(list, position));
        }

        private void RaiseTracksRemoved(IntPtr playlist, IntPtr[] tracks, int numTracks, IntPtr state)
        {
            List<Track> list = new List<Track>();
            for (int i = 0; i < numTracks; ++i)
                list.Add(new Track(tracks[i], false));

            Internal.EventDispatcher.Dispatch(this, playlist, OnTracksRemoved,
                new PlaylistTracksRemovedEventArgs(list));
        }

        private void RaiseTracksMoved(IntPtr playlist, IntPtr[] tracks, int numTracks, int newPosition, IntPtr state)
        {
            List<Track> list = new List<Track>();
            for (int i = 0; i < numTracks; ++i)
                list.Add(new Track(tracks[i], false));

            Internal.EventDispatcher.Dispatch(this, playlist, OnTracksMoved,
                new PlaylistTracksMovedEventArgs(list, newPosition));
        }

        private void RaiseCreatedChanged(IntPtr playlist, int position, IntPtr user, int when, IntPtr state)
        {
            Internal.EventDispatcher.Dispatch(this, playlist, OnTrackCreatedChanged,
                new PlaylistTrackCreatedChangedEventArgs(position, new User(user, false), TimeUtil.FromUnixTimeSeconds(when)));
        }

        private void RaiseSeenChanged(IntPtr playlist, int position, bool seen, IntPtr state)
        {
            Internal.EventDispatcher.Dispatch(this, playlist, OnTrackSeenChanged,
                  new PlaylistTrackSeenChangedEventArgs(position, seen));
        }

        private void RaiseImageChanged(IntPtr playlist, IntPtr image, IntPtr state)
        {
            // TODO
        }

        private void RaiseMessageChanged(IntPtr playlist, int position, IntPtr message, IntPtr state)
        {
            Internal.EventDispatcher.Dispatch(this, playlist, OnTrackMessageChanged,
                new PlaylistTrackMessageChangedEventArgs(position, LibSpotify.ReadUtf8(message)));
        }

        private void RaiseSubscribersChanged(IntPtr playlist, IntPtr state)
        {
            Internal.EventDispatcher.Dispatch(this, playlist, OnSubscribersChanged, EventArgs.Empty);
        }
        #endregion

        #region Fields
        private LibSpotify.sp_playlist_callbacks _callbacks;
        #endregion
    }
}
