using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using Spotify.Internal;

namespace Spotify
{
    public class Playlist : DomainObject //, INotifyPropertyChanged
    {
        #region Events
        public event EventHandler<PlaylistTracksAddedEventArgs> TracksAdded;
        public event EventHandler<PlaylistTracksRemovedEventArgs> TracksRemoved;
        public event EventHandler<PlaylistTracksMovedEventArgs> TracksMoved;
        public event EventHandler Renamed;
        public event EventHandler StateChanged;
        public event EventHandler<PlaylistUpdateInProgrssEventArgs> UpdateInProgress;
        public event EventHandler MetadataUpdated;
        public event EventHandler<PlaylistTrackCreatedChangedEventArgs> TrackCreatedChanged;
        public event EventHandler<PlaylistTrackSeenChangedEventArgs> TrackSeenChanged;
        public event EventHandler<PlaylistDescriptionChangedEventArgs> DescriptionChanged;
        public event EventHandler<PlaylistImageChangedEventArgs> ImageChanged;
        public event EventHandler SubscribersChanged;
        public event EventHandler<PlaylistTrackMessageChangedEventArgs> TrackMessageChanged;
        
        // There are so many "Changed" Events. I wonder if using PropertyChange is more appropriate.
        // public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        internal Playlist(IntPtr handle, bool preIncremented = true)
            : base(handle, LibSpotify.sp_playlist_add_ref_r, LibSpotify.sp_playlist_release_r, preIncremented)
        {
            _callbacks = new LibSpotify.sp_playlist_callbacks();
            _callbacks.description_changed = OnDescriptionChanged;
            _callbacks.image_changed = OnImageChanged;
            _callbacks.playlist_metadata_updated = OnMetadataUpdated;
            _callbacks.playlist_renamed = OnRenamed;
            _callbacks.playlist_state_changed = OnStateChanged;
            _callbacks.playlist_update_in_progress = OnUpdateInProgress;
            _callbacks.subscribers_changed = OnSubscribersChanged;
            _callbacks.track_created_changed = OnCreatedChanged;
            _callbacks.track_message_changed = OnMessageChanged;
            _callbacks.track_seen_changed = OnSeenChanged;
            _callbacks.tracks_added = OnTracksAdded;
            _callbacks.tracks_moved = OnTracksMoved;
            _callbacks.tracks_removed = OnTracksRemoved;
            
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

        // I'm still unsure whether exposing NumTracks/TrackAt(int) is better than a List of Tracks. The obvious
        // advantage is not creating a list of Disposable objects everytime someone touches the Tracks. Maybe caching
        // the results would be better and just invalidating on changes
        // The List also allows Linq to be used.
        private int NumTracks
        {
            get
            {
                return LibSpotify.sp_playlist_num_tracks_r(Handle);
            }
        }

        private Track TrackAt(int index)
        {

            return ListItem(index, p => { return new Track(p, false); },
                LibSpotify.sp_playlist_num_tracks_r, LibSpotify.sp_playlist_track_r);
        }
        
        public IList<Track> Tracks
        {
            get
            {
                if (_tracks == null)
                    _tracks = MakeList(p => { return new Track(p, false); }, LibSpotify.sp_playlist_num_tracks_r, LibSpotify.sp_playlist_track_r);
                return _tracks;
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

        private int NumSubscribers
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
                if (_subscribers == null)
                    _subscribers = MarshalPlaylistSubscribers(Handle);
                return _subscribers;
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

        #region Async Methods
        public Task<Image> LoadImageAsync(Session session, AsyncCallback userCallback, object state)
        {
            return Task.Factory.FromAsync<Session, Image>(BeginLoadImage, EndLoadImage, session, state);
        }

        public IAsyncResult BeginLoadImage(Session session, AsyncCallback userCallback, object state)
        {
            byte[] imageId = new byte[20];

            bool hasImage = LibSpotify.sp_playlist_get_image_r(Handle, imageId);
            if (!hasImage)
            {
                AsyncLoadImageResult result = new AsyncLoadImageResult(userCallback, state);
                result.CompletedSynchronously = true;
                result.SetCallbackComplete();
                result.SetCompleted(Error.Ok);
                return result;
            }

            IntPtr p = System.Runtime.InteropServices.Marshal.AllocHGlobal(imageId.Length);
            try
            {
                System.Runtime.InteropServices.Marshal.Copy(imageId, 0, p, imageId.Length);
                return ImageLoader.Begin((ptr, size) => { return ptr; }, p, session, ImageSize.Normal, userCallback, state);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FreeHGlobal(p);
            }
        }

        public Image EndLoadImage(IAsyncResult result)
        {
            return ImageLoader.End(result);
        }
        #endregion

        #region Private Methods   
        private static IList<string> MarshalPlaylistSubscribers(IntPtr playlist)
        {
            List<string> subscribers = new List<string>();

            IntPtr p = LibSpotify.sp_playlist_subscribers_r(playlist);
            if (p != IntPtr.Zero)
            {
                int n = Marshal.ReadInt32(p);

                int offset = Marshal.SizeOf(typeof(Int32));
                for (int i = 0; i < n; ++i)
                {
                    subscribers.Add(LibSpotify.ReadUtf8(Marshal.ReadIntPtr(p, offset)));
                    offset += Marshal.SizeOf(typeof(IntPtr));
                }

                LibSpotify.sp_playlist_subscribers_free_r(p);
            }

            return subscribers;
        }

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

        private void OnDescriptionChanged(IntPtr playlist, IntPtr description, IntPtr state)
        {
            Internal.EventDispatcher.Dispatch(this, playlist, DescriptionChanged,
                new PlaylistDescriptionChangedEventArgs(LibSpotify.ReadUtf8(description)));
        }

        private void OnMetadataUpdated(IntPtr playlist, IntPtr state)
        {
            Internal.EventDispatcher.Dispatch(this, playlist, MetadataUpdated, EventArgs.Empty);
        }

        private void OnRenamed(IntPtr playlist, IntPtr state)
        {
            Internal.EventDispatcher.Dispatch(this, playlist, Renamed, EventArgs.Empty);
        }

        private void OnStateChanged(IntPtr playlist, IntPtr state)
        {
            Internal.EventDispatcher.Dispatch(this, playlist, StateChanged, EventArgs.Empty);
        }

        private void OnUpdateInProgress(IntPtr playlist, bool done, IntPtr state)
        {
            Internal.EventDispatcher.Dispatch(this, playlist, UpdateInProgress,
                new PlaylistUpdateInProgrssEventArgs(done));
        }

        private static IList<int> ToList(int[] tracks, int n)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < n; ++i)
                list.Add(tracks[i]);
            return list;
        }

        private void OnTracksAdded(IntPtr playlist, int[] tracks, int numTracks, int position, IntPtr state)
        {
            _tracks = null;
            Internal.EventDispatcher.Dispatch(this, playlist, TracksAdded,
                new PlaylistTracksAddedEventArgs(ToList(tracks, numTracks), position));
        }

        private void OnTracksRemoved(IntPtr playlist, int[] tracks, int numTracks, IntPtr state)
        {
            _tracks = null;
            Internal.EventDispatcher.Dispatch(this, playlist, TracksRemoved,
                new PlaylistTracksRemovedEventArgs(ToList(tracks, numTracks)));
        }

        private void OnTracksMoved(IntPtr playlist, int[] tracks, int numTracks, int newPosition, IntPtr state)
        {
            _tracks = null;
            Internal.EventDispatcher.Dispatch(this, playlist, TracksMoved,
                new PlaylistTracksMovedEventArgs(ToList(tracks, numTracks), newPosition));
        }

        private void OnCreatedChanged(IntPtr playlist, int position, IntPtr user, int when, IntPtr state)
        {
            Internal.EventDispatcher.Dispatch(this, playlist, TrackCreatedChanged,
                new PlaylistTrackCreatedChangedEventArgs(position, new User(user, false), TimeUtil.FromUnixTimeSeconds(when)));
        }

        private void OnSeenChanged(IntPtr playlist, int position, bool seen, IntPtr state)
        {
            Internal.EventDispatcher.Dispatch(this, playlist, TrackSeenChanged,
                  new PlaylistTrackSeenChangedEventArgs(position, seen));
        }

        private void OnImageChanged(IntPtr playlist, IntPtr image, IntPtr state)
        {
            Internal.EventDispatcher.Dispatch(this, playlist, ImageChanged,
                new PlaylistImageChangedEventArgs(image));
        }

        private void OnMessageChanged(IntPtr playlist, int position, IntPtr message, IntPtr state)
        {
            Internal.EventDispatcher.Dispatch(this, playlist, TrackMessageChanged,
                new PlaylistTrackMessageChangedEventArgs(position, LibSpotify.ReadUtf8(message)));
        }

        private void OnSubscribersChanged(IntPtr playlist, IntPtr state)
        {
            _subscribers = null;
            Internal.EventDispatcher.Dispatch(this, playlist, SubscribersChanged, EventArgs.Empty);
        }
        #endregion

        #region Fields
        private IList<Track> _tracks = null;
        private IList<string> _subscribers = null;
        private LibSpotify.sp_playlist_callbacks _callbacks;
        #endregion
    }
}
