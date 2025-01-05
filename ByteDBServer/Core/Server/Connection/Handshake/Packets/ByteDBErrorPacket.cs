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

        public ByteDBErrorPacket(Stream stream, string message) : base(ByteDBPacketType.ErrorPacket)
        {
            //
            // ----------------------------- ERROR PACKET STRUCTURE ----------------------------- 
            //

            // Server Message
            AddRange(new NullTerminatedString(message).Bytes);
        }
    }
}
