using ByteDBServer.Core.DataTypes;
using ByteDBServer.Core.Server.Connection.Models;

namespace ByteDBServer.Core.Server.Connection.Handshake.Packets
{
    internal class ByteDBOkayPacket : ByteDBPacket
    {
        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //

        public NullTerminatedString Message { set { Payload.AddRange(value.Bytes); } }

        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBOkayPacket() : base(ByteDBPacketType.OkayPacket) { }
        public ByteDBOkayPacket(byte[] payload) : base(payload, ByteDBPacketType.OkayPacket) { }
        public ByteDBOkayPacket(byte[] header, byte[] payload) : base(header, payload) { }
        public ByteDBOkayPacket(NullTerminatedString message) : base(ByteDBPacketType.OkayPacket)
        {
            Message = message;
        }
    }
}
