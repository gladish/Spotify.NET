using System;
using System.Runtime.InteropServices;

namespace Spotify
{
    [StructLayout(LayoutKind.Sequential)]
    public struct AudioFormat
    {
        public readonly SampleType SampleType;
        public readonly int SampleRate;
        public readonly int Channels;
    }
}
