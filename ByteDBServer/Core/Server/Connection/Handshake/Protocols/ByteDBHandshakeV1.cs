using System;
using System.IO;
using ByteDBServer.Core.Server.Connection.Models;

namespace ByteDBServer.Core.Server.Connection.Handshake
{
    internal class ByteDBHandshakeV1 : ByteDBProtocol
    {
        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBHandshakeV1(Stream stream) : base(1)
        {
            try
            {
                new ByteDBWelcomePacketV1(stream);
            }
            catch
            {
                new ByteDBErrorPacket(stream);
            }
        }

        //
        // ----------------------------- DISPOSING ----------------------------- 
        //

        private bool _disposed = false;
        public override void Dispose()
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
        ~ByteDBHandshakeV1()
        {
            Dispose(false);
        }
    }
}
