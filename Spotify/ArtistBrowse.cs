using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Spotify.Internal;

namespace Spotify
{
    public class ArtistBrowse : DomainObject
    {
        private class AsyncLoadPortraitsResult : AsyncCallbackResult<IList<Image>>
        {
            private int _count = 0;
            private bool _raised = false;

            public AsyncLoadPortraitsResult(int numProtraits, AsyncCallback userCallback, object state)
                : base(userCallback, state)
            {
                _count = numProtraits;
            }

            public void HandleImageLoaded(object sender, EventArgs e)
            {
                CheckComplete();      
            }

            public bool CheckComplete()
            {                
                // I don't really like this. It seems a bit over complicated
                bool allLoaded = true;
                lock (Closure)
                {                    
                    if (_raised) return false; // only fire once
                    if (Closure.Count < _count)  return false; // only fire if list is full

                    // only fire if all images are loaded
                    foreach (Image image in Closure)
                    {
                        if (!image.IsLoaded)
                            allLoaded = false;
                    }
                    if (allLoaded)
                    {
                        _raised = true;
                        SetCallbackComplete();
                    }
                }
                return allLoaded;
            }
        }

        internal ArtistBrowse(IntPtr handle, bool preIncremented = true)
            : base(handle, LibSpotify.sp_artistbrowse_add_ref_r, LibSpotify.sp_artistbrowse_release_r, preIncremented)
        {
        }

        #region Properties
        public bool IsLoaded
        {
            get
            {
                return LibSpotify.sp_artistbrowse_is_loaded_r(Handle);
            }
        }

        public Error Error
        {
            get
            {
                return LibSpotify.sp_artistbrowse_error_r(Handle);
            }
        }

        public Artist Artist
        {
            get
            {
                return new Artist(LibSpotify.sp_artistbrowse_artist_r(Handle));
            }
        }


        public IList<Track> Tracks
        {
            get
            {
                return MakeList(p => { return new Track(p, false); }, LibSpotify.sp_artistbrowse_num_tracks_r, 
                    LibSpotify.sp_artistbrowse_track_r);
            }
        }

        public IList<Track> TopHitTracks
        {
            get
            {
                return MakeList(p => { return new Track(p, false); }, LibSpotify.sp_artistbrowse_num_tophit_tracks_r,
                    LibSpotify.sp_artistbrowse_tophit_track_r);
            }
        }

        public IList<Album> Albums
        {
            get
            { 
                return MakeList(p => { return new Album(p, false); }, LibSpotify.sp_artistbrowse_num_albums_r,
                    LibSpotify.sp_artistbrowse_album_r);
            }
        }     

        public IList<Artist> SimilarArtists
        {
            get
            {
                return MakeList(p => { return new Artist(p, false); }, LibSpotify.sp_artistbrowse_num_similar_artists_r,
                    LibSpotify.sp_artistbrowse_similar_artist_r);
            }
        }

        public string Biography
        {
            get
            {
                return LibSpotify.ReadUtf8(LibSpotify.sp_artistbrowse_biography_r(Handle));
            }
        }

        public TimeSpan? BackendRequestDuration
        {
            get
            {
                TimeSpan? duration = null;
                int d = LibSpotify.sp_artistbrowse_backend_request_duration_r(Handle);
                if (d != -1)
                    duration = TimeSpan.FromMilliseconds(d);
                return duration;                
            }
        }        
        #endregion

        public Task<IList<Image>> LoadPortraitsAsync(Session session, object state)
        {
            return Task.Factory.FromAsync<Session, IList<Image>>(BeginLoadPortraits, EndLoadPortraits, session, state);
        }

        public IAsyncResult BeginLoadPortraits(Session session, AsyncCallback userCallback, object state)
        {
            ThrowHelper.ThrowIfNull(session, "session");
            int n = LibSpotify.sp_artistbrowse_num_portraits_r(Handle);

            AsyncLoadPortraitsResult result = new AsyncLoadPortraitsResult(n, userCallback, state);

            result.Closure = new List<Image>();
            for (int i = 0; i < n; ++i)
            {
                Image image = new Image(LibSpotify.sp_image_create_r(session.Handle, LibSpotify.sp_artistbrowse_portrait_r(Handle, i)));
                image.Loaded += result.HandleImageLoaded;
                result.Closure.Add(image);                
            }

            if (result.CheckComplete())
                result.CompletedSynchronously = true;

            return result;
        }

        public IList<Image> EndLoadPortraits(IAsyncResult result)
        {
            AsyncLoadPortraitsResult loadImageResult = ThrowHelper.DownCast<AsyncLoadPortraitsResult>(result, "result");
            loadImageResult.WaitForCallbackComplete();

            List<Exception> errors = null;
            foreach (Image img in loadImageResult.Closure)
            {
                if (img.Error != Error.Ok)
                {
                    if (errors == null)
                        errors = new List<Exception>();
                    errors.Add(new Spotify.Exception(img.Error));
                }
            }

            System.Exception ex = null;
            if (errors != null)
                ex = new AggregateException(errors);

            loadImageResult.SetCompleted(ex);
            loadImageResult.CheckPendingException();
            return loadImageResult.Closure;
        }
    }
}
