using System;
using System.Runtime.InteropServices;

using Spotify.Internal;

namespace Spotify
{
    public sealed class Image : DomainObject
    {
        public event EventHandler Loaded;

        internal Image(IntPtr handle, bool preIncremented = true)
            : base(handle, LibSpotify.sp_image_add_ref_r, LibSpotify.sp_image_release_r, preIncremented)
        {
            ThrowHelper.ThrowIfError(LibSpotify.sp_image_add_load_callback_r(Handle, OnImageLoaded, IntPtr.Zero));
        }

        public System.Drawing.Image ToImage()
        {
            // don't Dispose() the MemoryStream, the Image will own it.
            return System.Drawing.Image.FromStream(new System.IO.MemoryStream(Data));
        }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            if (disposing)
            {
                LibSpotify.sp_image_remove_load_callback_r(Handle, OnImageLoaded, IntPtr.Zero);
            }

            base.Dispose(disposing);
        }

        private void OnImageLoaded(IntPtr image, IntPtr user)
        {
            EventDispatcher.Dispatch(this, image, Loaded, EventArgs.Empty);
        }

        public bool IsLoaded
        {
            get
            {
                return LibSpotify.sp_image_is_loaded_r(Handle);
            }
        }

        public ImageFormat Format
        {
            get
            {
                return LibSpotify.sp_image_format_r(Handle);
            }
        }

        public byte[] Data
        {
            get
            {
                IntPtr n = IntPtr.Zero;
                IntPtr p = LibSpotify.sp_image_data_r(Handle, ref n);

                byte[] bytes = new byte[n.ToInt32()];
                
                for (int i = 0; i < n.ToInt32(); ++i)
                    bytes[i] = Marshal.ReadByte(p, i);

                return bytes;
            }
        }

        public Error Error
        {
            get
            {
                return LibSpotify.sp_image_error_r(Handle);
            }
        }
    }
}
