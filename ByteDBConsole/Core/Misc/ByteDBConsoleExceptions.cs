namespace ByteDBConsole.Core.Misc
{
    public class ByteDBCommandException : Exception
    {
        public ByteDBCommandException() { }
        public ByteDBCommandException(string message) : base(message) { }
    }
    public class ByteDBServerException : Exception
    {
        public ByteDBServerException() { }
        public ByteDBServerException(string message) : base(message) { }
    }
    public class ByteDBProcessException : Exception
    {
        public ByteDBProcessException() { }
        public ByteDBProcessException(string message) : base(message) { }
    }
    public class ByteDBConfigException : Exception
    {
        public ByteDBConfigException() { }
        public ByteDBConfigException(string message) : base(message) { }
    }
}
