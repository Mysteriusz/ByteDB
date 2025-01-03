using ByteDBServer.Core.DataTypes;
using ByteDBServer.Core.Server.Connection.Models;
using System.IO;
using System;

namespace ByteDBServer.Core.Server.Connection.Handshake
{
    internal class ByteDBErrorPacket : ByteDBPacket
    {
        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBErrorPacket(Stream stream) : base(ByteDBPacketType.ErrorPacket)
        {
            //
            // ----------------------------- ERROR PACKET STRUCTURE ----------------------------- 
            //

            // Write Error Packet To Provided Stream
            Write(stream);
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
                    this.Header.Clear();
                    this.Payload.Clear();
                }

                _disposed = true;
            }
        }
        ~ByteDBErrorPacket()
        {
            Dispose(false);
        }
    }
}
