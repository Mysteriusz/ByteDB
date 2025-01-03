using ByteDBServer.Core.DataTypes;
using ByteDBServer.Core.Server.Connection.Models;
using System.IO;

namespace ByteDBServer.Core.Server.Connection.Handshake
{
    internal class ByteDBWelcomePacketV1 : ByteDBPacket
    {
        public ByteDBWelcomePacketV1(Stream stream) 
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

        public override void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}
