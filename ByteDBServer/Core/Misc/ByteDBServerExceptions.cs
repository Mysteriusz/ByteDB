using System;

namespace ByteDBServer.Core.Misc
{
    //
    // ----------------------------- INTERNAL RELATED ----------------------------- 
    //

    public class Int2OverflowException : Exception
    {
        public Int2OverflowException() : base("Value exceeds the valid range for Int2.") { }
        public Int2OverflowException(string message) : base(message) { }
    }

    public class Int3OverflowException : Exception
    {
        public Int3OverflowException() : base("Value exceeds the valid range for Int3.") { }
        public Int3OverflowException(string message) : base(message) { }
    }

    public class Int4OverflowException : Exception
    {
        public Int4OverflowException() : base("Value exceeds the valid range for Int4.") { }
        public Int4OverflowException(string message) : base(message) { }
    }

    public class Int6OverflowException : Exception
    {
        public Int6OverflowException() : base("Value exceeds the valid range for Int6.") { }
        public Int6OverflowException(string message) : base(message) { }
    }

    public class Int8OverflowException : Exception
    {
        public Int8OverflowException() : base("Value exceeds the valid range for Int8.") { }
        public Int8OverflowException(string message) : base(message) { }
    }

    public class ByteDBInternalException : Exception
    {
        public ByteDBInternalException() : base("Server threw an exception.") { }
        public ByteDBInternalException(string message) : base(message) { }
    }

    //
    // ----------------------------- CONNECTION RELATED ----------------------------- 
    //

    public class ByteDBConnectionOverflowException : Exception
    {
        public const string DefaultMessage = "Server connection overflow";

        public ByteDBConnectionOverflowException() : base(DefaultMessage) { }
        public ByteDBConnectionOverflowException(string message) : base(message) { }
    }

    public class ByteDBTimeoutException : Exception
    {
        public const string DefaultMessage = "Handshake Timeout";

        public ByteDBTimeoutException() : base(DefaultMessage) { }
        public ByteDBTimeoutException(string message) : base(message) { }
    }

    public class ByteDBPacketException : Exception
    {
        public const string DefaultMessage = "Packet out of order";

        public ByteDBPacketException() : base(DefaultMessage) { }
        public ByteDBPacketException(string message) : base(message) { }
    }

    public class ByteDBPacketDataException : Exception
    {
        public const string DefaultMessage = "Packet data incorrect";

        public ByteDBPacketDataException() : base(DefaultMessage) { }
        public ByteDBPacketDataException(string message) : base(message) { }
    }

    public class ByteDBConnectionException : Exception
    {
        public const string DefaultMessage = "Exception was thrown";

        public ByteDBConnectionException() : base(DefaultMessage) { }
        public ByteDBConnectionException(string message) : base(message) { }
    }
}
