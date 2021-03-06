﻿using System;

namespace Spotify
{
    public class MusicDeliveryEventArgs : EventArgs
    {
        internal MusicDeliveryEventArgs(byte[] data, AudioFormat format)
        {
            PcmData = data;
            Format = format;
        }
        public readonly byte[] PcmData;
        public readonly AudioFormat Format;
    }
}
