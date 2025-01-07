using ByteDBServer.Core.DataTypes;
using ByteDBServer.Core.Server.Connection.Models;

namespace ByteDBServer.Core.Server.Connection.Handshake
{
    internal class ByteDBWelcomePacketV1 : ByteDBPacket
    {
        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //

        public NullTerminatedString Message { set { Payload.AddRange(value.Bytes); } }
        public NullTerminatedString Version { set { Payload.AddRange(value.Bytes); } }
        public Int4 Capabilities { set { Payload.AddRange(value.Bytes); } }
        public Int2 SaltLength { set { Payload.AddRange(value.Bytes); } }
        public byte[] Salt { set { Payload.AddRange(value); } }
        public byte AuthenticationType { set { Payload.Add(value); } }

        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBWelcomePacketV1() : base(ByteDBPacketType.WelcomePacket) { }
        public ByteDBWelcomePacketV1(byte[] payload) : base(payload, ByteDBPacketType.WelcomePacket) { }
        public ByteDBWelcomePacketV1(byte[] header, byte[] payload) : base(header, payload) { }
        public ByteDBWelcomePacketV1(NullTerminatedString message, NullTerminatedString version, Int4 capabilities, Int2 saltLength, byte[] salt, byte authType) : base(ByteDBPacketType.WelcomePacket)
        { 
            Message = message;
            Version = version;
            Capabilities = capabilities;
            SaltLength = saltLength;
            Salt = salt;
            AuthenticationType = authType;
        }
    }
}
