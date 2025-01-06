﻿using ByteDBServer.Core.DataTypes;
using ByteDBServer.Core.Server.Connection.Models;

namespace ByteDBServer.Core.Server.Connection.Handshake.Packets
{
    internal class ByteDBOkayPacket : ByteDBPacket, IByteDBValidator<ByteDBPacket>
    {
        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBOkayPacket() : base(ByteDBPacketType.OkayPacket) { }
        public ByteDBOkayPacket(byte[] payload) : base(payload, ByteDBPacketType.OkayPacket) { }
        public ByteDBOkayPacket(byte[] header, byte[] payload) : base(header, payload) { }

        public ByteDBOkayPacket(NullTerminatedString message) 
        { Payload.AddRange(message.Bytes); }

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
                if (fullPacket[0] != (byte)ByteDBPacketType.OkayPacket)
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
