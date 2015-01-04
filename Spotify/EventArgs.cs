using System;

namespace Spotify
{
    public class ErrorEventArgs : EventArgs
    {
        public ErrorEventArgs(Error code) { ErrorCode = code; }
        public readonly Error ErrorCode;
    }

    public class LoggedInEventArgs : ErrorEventArgs
    {
        public LoggedInEventArgs(Error code) : base(code) { }
    }

    public class ConnectionErrorEventArgs : ErrorEventArgs
    {
        public ConnectionErrorEventArgs(Error code) : base(code) { }
    }

    public class MessageToUserEventArgs : EventArgs
    {
        public MessageToUserEventArgs(string s) { Message = s; }
        public readonly string Message;
    }

    public class LogMessageEventArgs : EventArgs
    {
        public LogMessageEventArgs(string s) { Message = s; }
        public readonly string Message;
    }

    public class StreamingErrorEventArgs : ErrorEventArgs
    {
        public StreamingErrorEventArgs(Error code) : base(code) { }
    }

    public class OfflineErrorEventArgs : ErrorEventArgs
    {
        public OfflineErrorEventArgs(Error code) : base(code) { }
    }

    public class CredentialsBlobUpdatedEventArgs
    {
        public CredentialsBlobUpdatedEventArgs(string s)
        {
            Blob = s;
        }
        public readonly string Blob;
    }

    public class ScrobbleErrorEventArgs : ErrorEventArgs
    {
        public ScrobbleErrorEventArgs(Error code) : base(code) { }
    }

    public class PrivateSessonModeChangedEventArgs
    {
        public PrivateSessonModeChangedEventArgs(bool b)
        {
            Changed = b;
        }
        public readonly bool Changed;
    }

    public class MusicDeliveryEventArgs : EventArgs
    {
        public MusicDeliveryEventArgs(byte[] data, AudioFormat format)
        {
            PcmData = data;
            Format = format;
        }
        public readonly byte[] PcmData;
        public readonly AudioFormat Format;
    }
}
