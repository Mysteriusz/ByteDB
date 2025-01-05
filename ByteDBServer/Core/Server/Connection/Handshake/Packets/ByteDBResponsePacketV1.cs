using ByteDBServer.Core.DataTypes;
using ByteDBServer.Core.Misc;
using ByteDBServer.Core.Server.Connection.Models;
using System;
using System.IO;
using System.Linq;

namespace ByteDBServer.Core.Server.Connection.Handshake.Packets
{
    internal class ByteDBResponsePacketV1 : ByteDBPacket
    {
        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBResponsePacketV1() : base(ByteDBPacketType.ResponsePacket)  { }
        public ByteDBResponsePacketV1(byte[] payload) : base(ByteDBPacketType.ResponsePacket) { Payload.AddRange(payload); }

        //
        // ----------------------------- OVERRIDES ----------------------------- 
        //

        public override bool Validate(ByteDBPacket packet)
        {
            //
            // ----------------------------- RESPONSE PACKET STRUCTURE ----------------------------- 
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

                // Requested Capabilities
                Int4 capabilities = new Int4(fullPacket[4], fullPacket[5], fullPacket[6], fullPacket[7]);

                // Username
                int usernameEndingIndex = 0;
                NullTerminatedString name = new NullTerminatedString(fullPacket, 8, out usernameEndingIndex);

                // Password 
                byte[] passwordHash = new byte[64];
                Array.Copy(fullPacket, usernameEndingIndex, passwordHash, 0, 64);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
