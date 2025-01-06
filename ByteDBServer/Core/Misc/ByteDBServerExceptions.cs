using ByteDBServer.Core.Misc.Logs;
using ByteDBServer.Core.Server;
using ByteDBServer.Core.Server.Connection.Handshake;
using System;
using System.IO;

namespace ByteDBServer.Core.Misc
{
    // Base exception class to handle logging and default message
    public abstract class ByteDBServerException : Exception
    {
        protected ByteDBServerException(string message) : base(message)
        {
            ByteDBServerLogger.WriteExceptionToFile(this);
        }
        protected ByteDBServerException(Stream stream, string message) : base(message)
        {
            ByteDBServerLogger.WriteExceptionToFile(this);

            var packet = new ByteDBErrorPacket();
            packet.AddRange(ByteDBServerInstance.ServerEncoding.GetBytes(message));

            packet.Write(stream);
        }
    }

    //
    // ----------------------------- INTERNAL RELATED ----------------------------- 
    //

    public class Int2OverflowException : ByteDBServerException
    {
        public Int2OverflowException() : base("Value exceeds the valid range for Int2.") { }
        public Int2OverflowException(string message) : base(message) { }
    }

    public class Int3OverflowException : ByteDBServerException
    {
        public Int3OverflowException() : base("Value exceeds the valid range for Int3.") { }
        public Int3OverflowException(string message) : base(message) { }
    }

    public class Int4OverflowException : ByteDBServerException
    {
        public Int4OverflowException() : base("Value exceeds the valid range for Int4.") { }
        public Int4OverflowException(string message) : base(message) { }
    }

    public class Int6OverflowException : ByteDBServerException
    {
        public Int6OverflowException() : base("Value exceeds the valid range for Int6.") { }
        public Int6OverflowException(string message) : base(message) { }
    }

    public class Int8OverflowException : ByteDBServerException
    {
        public Int8OverflowException() : base("Value exceeds the valid range for Int8.") { }
        public Int8OverflowException(string message) : base(message) { }
    }

    //
    // ----------------------------- CONNECTION RELATED ----------------------------- 
    //

    public class ConnectionsOverflowException : ByteDBServerException
    {
        public const string DefaultMessage = "Server connection overflow";

        public ConnectionsOverflowException() : base(DefaultMessage) { }
        public ConnectionsOverflowException(string message) : base(message) { }

        public ConnectionsOverflowException(Stream stream) : base(stream, DefaultMessage) { }
        public ConnectionsOverflowException(Stream stream, string message) : base(stream, message) { }
    }

    public class HandshakeTimeoutException : ByteDBServerException
    {
        public const string DefaultMessage = "Handshake Timeout";

        public HandshakeTimeoutException() : base(DefaultMessage) { }
        public HandshakeTimeoutException(string message) : base(message) { }

        public HandshakeTimeoutException(Stream stream) : base(stream, DefaultMessage) { }
        public HandshakeTimeoutException(Stream stream, string message) : base(stream, message) { }
    }

    public class HandshakePacketException : ByteDBServerException
    {
        public const string DefaultMessage = "Packet out of order";

        public HandshakePacketException() : base(DefaultMessage) { }
        public HandshakePacketException(string message) : base(message) { }

        public HandshakePacketException(Stream stream) : base(stream, DefaultMessage) { }
        public HandshakePacketException(Stream stream, string message) : base(stream, message) { }
    }

    public class InternalConnectionException : ByteDBServerException
    {
        public const string DefaultMessage = "Exception was thrown";

        public InternalConnectionException() : base(DefaultMessage) { }
        public InternalConnectionException(string message) : base(message) { }

        public InternalConnectionException(Stream stream) : base(stream, DefaultMessage) { }
        public InternalConnectionException(Stream stream, string message) : base(stream, message) { }
    }
}
