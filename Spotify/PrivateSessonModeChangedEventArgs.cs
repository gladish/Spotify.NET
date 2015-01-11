using System;

namespace Spotify
{
    public class PrivateSessonModeChangedEventArgs : EventArgs
    {
        internal PrivateSessonModeChangedEventArgs(bool b)
        {
            Changed = b;
        }
        public readonly bool Changed;
    }
}
