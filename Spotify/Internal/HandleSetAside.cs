using System;
using System.Collections.Generic;

namespace Spotify.Internal
{
    internal static class HandleSetAside
    {
        private static object _lock = new object();
        private static long _nextId = 1;
        private static Dictionary<IntPtr, object> _cache = new Dictionary<IntPtr, object>();

        public static IntPtr Put(object obj)
        {
            ThrowHelper.ThrowIfNull(obj, "obj");

            IntPtr p = IntPtr.Zero;
            lock (_lock)
            {
                p = new IntPtr(_nextId++);
                _cache.Add(p, obj);
            }
            return p;
        }

        public static object Get(IntPtr p)
        {
            object obj = null;
            lock (_lock)
            {
                if (!_cache.ContainsKey(p))
                    throw new ArgumentException("cache doesn't contain " + p);

                obj = _cache[p];
                _cache.Remove(p);
            }
            return obj;
        }
    }
}
