using ByteDBServer.Core.DataTypes;
using ByteDBServer.Core.Server.Connection.Models;

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
    }
}
