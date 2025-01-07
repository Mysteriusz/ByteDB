using ByteDBServer.Core.DataTypes;
using ByteDBServer.Core.Misc;
using ByteDBServer.Core.Server.Connection.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ByteDBServer.Core.Server.Connection.Handshake.Packets
{
    internal class ByteDBResponsePacketV1 : ByteDBPacket
    {
        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //

        public NullTerminatedString Username { set { Payload.AddRange(value.Bytes); } }
        public Int4 Capabilities { set { Payload.AddRange(value.Bytes); } }
        public Int2 AuthScrambleSize { set { Payload.AddRange(value.Bytes); } }
        public byte[] AuthScramble { set { Payload.AddRange(value); } }

        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBResponsePacketV1() : base(ByteDBPacketType.ResponsePacket) { }
        public ByteDBResponsePacketV1(byte[] payload) : base(payload, ByteDBPacketType.ResponsePacket) { }
        public ByteDBResponsePacketV1(byte[] header, byte[] payload) : base(header, payload) { }
        public ByteDBResponsePacketV1(NullTerminatedString username, Int4 capabilities, Int2 authScrambleSize, byte[] authScramble) : base(ByteDBPacketType.ResponsePacket)
        {
            Username = username;
            Capabilities = capabilities;
            AuthScrambleSize = authScrambleSize;
            AuthScramble = authScramble;
        }

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        public override void Read(byte[] bytes, int index = 0)
        {
            Payload.Clear();

            try
            {
                // ----------------------------- HEADER ----------------------------- 

                if (bytes[index] != (byte)PacketType)
                    throw new ByteDBPacketException("Packet type is incorrect.");

                Int3 packetSize = new Int3(bytes, index + 1);

                // ----------------------------- PAYLOAD ----------------------------- 

                Username = new NullTerminatedString(bytes, index + 4, out int usernameIndex);

                Capabilities = new Int4(bytes, usernameIndex + 1);

                Int2 authScrambleSize = new Int2(bytes, usernameIndex + 5);
                AuthScrambleSize = authScrambleSize;

                AuthScramble = bytes.Skip(usernameIndex + 7).Take(authScrambleSize).ToArray();
            }
            catch (IndexOutOfRangeException)
            {
                throw new ByteDBPacketException("Byte array is too short for this packet.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
