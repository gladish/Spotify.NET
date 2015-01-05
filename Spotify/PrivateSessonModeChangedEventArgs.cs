using System;

namespace Spotify
{
    public class PrivateSessonModeChangedEventArgs : EventArgs
    {
        public PrivateSessonModeChangedEventArgs(bool b)
        {
            Changed = b;
        }
        public readonly bool Changed;
    }
}
