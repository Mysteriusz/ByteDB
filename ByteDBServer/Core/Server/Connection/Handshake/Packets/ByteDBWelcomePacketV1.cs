using ByteDBServer.Core.DataTypes;
using ByteDBServer.Core.Server.Connection.Models;
using System;
using System.IO;
using System.Linq;

namespace ByteDBServer.Core.Server.Connection.Handshake
{
    internal class ByteDBWelcomePacketV1 : ByteDBPacket
    {
        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBWelcomePacketV1() : base(ByteDBPacketType.WelcomePacket) { }
        public ByteDBWelcomePacketV1(byte[] payload) : base(ByteDBPacketType.WelcomePacket) { Payload.AddRange(payload); }

        //
        // ----------------------------- OVERRIDES ----------------------------- 
        //

        public override bool Validate(ByteDBPacket packet)
        {
            //
            // ----------------------------- WELCOME PACKET STRUCTURE ----------------------------- 
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
                int messageEndingIndex;
                NullTerminatedString message = new NullTerminatedString(fullPacket, 4, out messageEndingIndex);

                // Version
                int versionEndingIndex;
                NullTerminatedString version = new NullTerminatedString(fullPacket, messageEndingIndex, out versionEndingIndex);

                // Capabilities 
                Int4 capabilities = new Int4(fullPacket, versionEndingIndex);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
