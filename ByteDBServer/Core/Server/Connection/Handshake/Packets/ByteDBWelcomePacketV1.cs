using ByteDBServer.Core.DataTypes;
using ByteDBServer.Core.Server.Connection.Models;
using System;
using System.IO;

namespace ByteDBServer.Core.Server.Connection.Handshake
{
    internal class ByteDBWelcomePacketV1 : ByteDBPacket
    {
        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBWelcomePacketV1(Stream stream) : base(ByteDBPacketType.WelcomePacket)
        {
            //
            // ----------------------------- WELCOME PACKET STRUCTURE ----------------------------- 
            //

            // Server Greeting
            AddRange(new NullTerminatedString("Welcome to ByteDB").Bytes);

            // Server Version
            AddRange(new NullTerminatedString(ByteDBServer.Version).Bytes);

            // Packet Type
            Add((byte)PacketType);

            // Server Capabilities
            AddRange((byte[])ByteDBServer.ServerCapabilitiesInt);

            // Write Welcoming Packet To Provided Stream
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
        ~ByteDBWelcomePacketV1()
        {
            Dispose(false);
        }
    }
}
