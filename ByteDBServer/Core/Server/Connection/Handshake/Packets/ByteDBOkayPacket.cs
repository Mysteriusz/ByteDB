using ByteDBServer.Core.DataTypes;
using ByteDBServer.Core.Server.Connection.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByteDBServer.Core.Server.Connection.Handshake.Packets
{
    internal class ByteDBOkayPacket : ByteDBPacket
    {
        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBOkayPacket(Stream stream, string message) : base(ByteDBPacketType.OkayPacket)
        {
            //
            // ----------------------------- OKAY PACKET STRUCTURE ----------------------------- 
            //

            // Server Message
            AddRange(new NullTerminatedString(message).Bytes);

            // Write Error Packet To Provided Stream
            Write(stream);
        }
    }
}
