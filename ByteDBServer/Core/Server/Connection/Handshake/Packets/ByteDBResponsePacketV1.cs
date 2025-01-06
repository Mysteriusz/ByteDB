﻿using ByteDBServer.Core.DataTypes;
using ByteDBServer.Core.Server.Connection.Models;
using System;

namespace ByteDBServer.Core.Server.Connection.Handshake.Packets
{
    internal class ByteDBResponsePacketV1 : ByteDBPacket
    {
        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBResponsePacketV1() : base(ByteDBPacketType.ResponsePacket)  { }
        public ByteDBResponsePacketV1(byte[] payload) : base(payload, ByteDBPacketType.ResponsePacket) { }
        public ByteDBResponsePacketV1(byte[] header, byte[] payload) : base(header, payload) { }

        public ByteDBResponsePacketV1(Int4 capabilities, NullTerminatedString name, byte[] digest64) 
        { Payload.AddRange(capabilities.Bytes); Payload.AddRange(name.Bytes); Payload.AddRange(digest64); }

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
                if (fullPacket[0] != (byte)ByteDBPacketType.ResponsePacket)
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
