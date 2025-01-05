using ByteDBServer.Core.DataTypes;
using ByteDBServer.Core.Server.Connection.Models;
using System;
using System.IO;
using System.Linq;

namespace ByteDBServer.Core.Server.Connection.Handshake.Packets
{
    internal class ByteDBOkayPacket : ByteDBPacket
    {
        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBOkayPacket() : base(ByteDBPacketType.OkayPacket) { }
        public ByteDBOkayPacket(byte[] packet) : base(ByteDBPacketType.OkayPacket) { Payload.AddRange(packet); }

        //
        // ----------------------------- OVERRIDES ----------------------------- 
        //

        public override bool Validate(ByteDBPacket packet)
        {
            //
            // ----------------------------- OKAY PACKET STRUCTURE ----------------------------- 
            //

            try
            {
                byte[] fullPacket = GetPacket(packet).ToArray();

                // ----------------------------- HEADER ----------------------------- 

                // Check packet type
                if (fullPacket[0] != (byte)PacketType)
                    return false;

                // Payload Size
                Int3 size = new Int3(fullPacket[1], fullPacket[2], fullPacket[3]);

                // ----------------------------- PAYLOAD ----------------------------- 

                // Message
                NullTerminatedString message = new NullTerminatedString(fullPacket, 4);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
