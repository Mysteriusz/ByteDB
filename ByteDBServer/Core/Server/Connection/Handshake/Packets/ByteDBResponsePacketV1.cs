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

        public ByteDBResponsePacketV1(Stream stream, byte[] bytes) : base(ByteDBPacketType.ResponsePacket) 
        {
            ValidatePacket(bytes);
        }

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        private void ValidatePacket(byte[] packet)
        {
            //
            // ----------------------------- RESPONSE PACKET STRUCTURE ----------------------------- 
            //

            try
            {
                // Check packet type
                if (packet[0] != (byte)PacketType)
                    throw new HandshakePacketException("Incorrect packet type");

                // Payload Size
                Int3 size = new Int3(packet[1], packet[2], packet[3]);

                // Requested Capabilities
                Int4 capabilities = new Int4(packet[4], packet[5], packet[6], packet[7]);

                // Username
                int usernameEndingIndex = 0;
                NullTerminatedString name = new NullTerminatedString(packet, 8, out usernameEndingIndex);

                // Password 
                byte[] passwordHash = new byte[64];
                Array.Copy(packet, usernameEndingIndex, passwordHash, 0, 64);
            }
            catch
            {
                throw new HandshakePacketException();
            }
        }
    }
}
