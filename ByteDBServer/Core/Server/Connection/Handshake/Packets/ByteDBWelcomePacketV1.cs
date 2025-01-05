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

        public ByteDBWelcomePacketV1(string message) : base(ByteDBPacketType.WelcomePacket)
        {
            //
            // ----------------------------- WELCOME PACKET STRUCTURE ----------------------------- 
            //
            
            // Server Greeting
            AddRange(new NullTerminatedString(message).Bytes);

            // Server Version
            AddRange(new NullTerminatedString(ByteDBServer.Version).Bytes);

            // Server Capabilities
            AddRange((byte[])ByteDBServer.ServerCapabilitiesInt);
        }
    }
}
