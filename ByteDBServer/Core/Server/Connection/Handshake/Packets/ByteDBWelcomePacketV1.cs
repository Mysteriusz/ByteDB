using ByteDBServer.Core.DataTypes;
using ByteDBServer.Core.Misc;
using ByteDBServer.Core.Server.Connection.Models;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Linq;
using System;
using System.Threading.Tasks;

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

                Message = new NullTerminatedString(bytes, index + 4, out int usernameIndex);

                Version = new NullTerminatedString(bytes, usernameIndex + 1, out int versionIndex);

                Capabilities = new Int4(bytes, versionIndex + 1);

                var saltLength = new Int2(bytes, versionIndex + 5);
                SaltLength = saltLength;

                Salt = bytes.Skip(versionIndex + 7).Take(saltLength).ToArray();

                AuthenticationType = bytes[versionIndex + 7 + saltLength];
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
