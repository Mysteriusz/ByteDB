﻿using System;

namespace ByteDBServer.Core.Misc
{
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
    public class ConnectionsOverflowException : Exception
    {
        public ConnectionsOverflowException() : base("Server connection overflow") { }
        public ConnectionsOverflowException(string message) : base(message) { }
    }
}
