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

    public class ByteDBInternalException : ByteDBServerException
    {
        public ByteDBInternalException() : base("Server threw an exception.") { }
        public ByteDBInternalException(string message) : base(message) { }
    }

    //
    // ----------------------------- CONNECTION RELATED ----------------------------- 
    //

    public class ByteDBConnectionOverflowException : ByteDBServerException
    {
        public const string DefaultMessage = "Server connection overflow";

        public ByteDBConnectionOverflowException() : base(DefaultMessage) { }
        public ByteDBConnectionOverflowException(string message) : base(message) { }

        public ByteDBConnectionOverflowException(Stream stream) : base(stream, DefaultMessage) { }
        public ByteDBConnectionOverflowException(Stream stream, string message) : base(stream, message) { }
    }

    public class ByteDBTimeoutException : ByteDBServerException
    {
        public const string DefaultMessage = "Handshake Timeout";

        public ByteDBTimeoutException() : base(DefaultMessage) { }
        public ByteDBTimeoutException(string message) : base(message) { }

        public ByteDBTimeoutException(Stream stream) : base(stream, DefaultMessage) { }
        public ByteDBTimeoutException(Stream stream, string message) : base(stream, message) { }
    }

    public class ByteDBPacketException : ByteDBServerException
    {
        public const string DefaultMessage = "Packet out of order";

        public ByteDBPacketException() : base(DefaultMessage) { }
        public ByteDBPacketException(string message) : base(message) { }

        public ByteDBPacketException(Stream stream) : base(stream, DefaultMessage) { }
        public ByteDBPacketException(Stream stream, string message) : base(stream, message) { }
    }

    public class ByteDBConnectionException : ByteDBServerException
    {
        public const string DefaultMessage = "Exception was thrown";

        public ByteDBConnectionException() : base(DefaultMessage) { }
        public ByteDBConnectionException(string message) : base(message) { }

        public ByteDBConnectionException(Stream stream) : base(stream, DefaultMessage) { }
        public ByteDBConnectionException(Stream stream, string message) : base(stream, message) { }
    }
}
