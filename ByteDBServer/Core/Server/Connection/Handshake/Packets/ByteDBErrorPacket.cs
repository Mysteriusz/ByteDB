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

        public ByteDBErrorPacket() : base(ByteDBPacketType.ErrorPacket) { }
        public ByteDBErrorPacket(byte[] payload) : base(ByteDBPacketType.ErrorPacket) { Payload.AddRange(payload); }
        //
        // ----------------------------- OVERRIDES ----------------------------- 
        //

        public override bool Validate(ByteDBPacket packet)
        {
            //
            // ----------------------------- ERROR PACKET STRUCTURE ----------------------------- 
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
