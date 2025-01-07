using ByteDBServer.Core.DataTypes;
using ByteDBServer.Core.Server.Connection.Models;

namespace ByteDBServer.Core.Server.Connection.Handshake
{
    internal class ByteDBErrorPacket : ByteDBPacket
    {
        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //

        public NullTerminatedString Message { set { Payload.AddRange(value.Bytes); } }

        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBErrorPacket() : base(ByteDBPacketType.ErrorPacket) { }
        public ByteDBErrorPacket(byte[] payload) : base(payload, ByteDBPacketType.ErrorPacket) { }
        public ByteDBErrorPacket(byte[] header, byte[] payload) : base(header, payload) { }
        public ByteDBErrorPacket(NullTerminatedString message) : base(ByteDBPacketType.ErrorPacket)
        { 
            Message = message;
        }
    }
}
