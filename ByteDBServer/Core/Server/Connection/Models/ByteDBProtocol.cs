using System;

namespace ByteDBServer.Core.Server.Connection.Models
{
    internal abstract class ByteDBProtocol : IDisposable
    {
        //
        // ----------------------------- PARAMETERS ----------------------------- 
        //

        public byte Version { get; set; }

        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBProtocol(byte version)
        {
            Version = version;
        }

        //
        // ----------------------------- DISPOSING ----------------------------- 
        //

        private bool _disposed = false;
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    this.Version = 0;
                }

                _disposed = true;
            }
        }
        ~ByteDBProtocol()
        {
            Dispose(false);
        }
    }
}
