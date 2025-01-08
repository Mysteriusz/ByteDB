using ByteDBServer.Core.Server.Packets.Models;
using ByteDBServer.Core.DataTypes;
using ByteDBServer.Core.Misc;
using System.Linq;
using System;

namespace ByteDBServer.Core.Server.Packets
{
    internal class ByteDBWelcomePacketV1 : ByteDBPacket
    {
        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //

        public NullTerminatedString Message { get; private set; }
        public NullTerminatedString Version { get; private set; }
        public Int4 Capabilities { get; private set; }
        public Int2 SaltLength { get; private set; }
        public byte[] Salt { get; private set; }
        public byte AuthenticationType { get; private set; }

        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBWelcomePacketV1() : base(ByteDBPacketType.WelcomePacket) { }
        public ByteDBWelcomePacketV1(byte[] payload) : base(ByteDBPacketType.WelcomePacket, payload) { }
        public ByteDBWelcomePacketV1(NullTerminatedString message, NullTerminatedString version, Int4 capabilities, Int2 saltLength, byte[] salt, byte authType) : base(ByteDBPacketType.WelcomePacket)
        {
            AddRange(message.Bytes);
            Message = message;

            AddRange(version.Bytes);
            Version = version;

            AddRange(capabilities.Bytes);
            Capabilities = capabilities;

            AddRange(saltLength.Bytes);
            SaltLength = saltLength;

            AddRange(salt);
            Salt = salt;

            Add(authType);
            AuthenticationType = authType;
        }

        //
        // ----------------------------- OVERRIDES ----------------------------- 
        //

        public override void ReInitialize(byte[] bytes, int index = 0)
        {
            Payload.Clear();

            try
            {
                // ----------------------------- HEADER ----------------------------- 

                if (bytes[index] != (byte)PacketType)
                    throw new ByteDBPacketException("Packet type is incorrect.");

                Int3 packetSize = new Int3(bytes, index + 1);

                // ----------------------------- PAYLOAD ----------------------------- 

                byte[] packetPayload = bytes.Skip(index + 4).Take(packetSize).ToArray();

                // ----------------------------- TRY TO ASSIGN VALUES ----------------------------- 

                Message = new NullTerminatedString(packetPayload, 0, out int usernameIndex);
                AddRange(Message.Bytes);

                Version = new NullTerminatedString(packetPayload, usernameIndex + 1, out int versionIndex);
                AddRange(Version.Bytes);

                Capabilities = new Int4(packetPayload, versionIndex + 1);
                AddRange(Capabilities.Bytes);

                SaltLength = new Int2(packetPayload, versionIndex + 5);
                AddRange(SaltLength.Bytes);

                Salt = packetPayload.Skip(versionIndex + 7).Take(SaltLength).ToArray();
                AddRange(Salt);

                AuthenticationType = packetPayload[versionIndex + 7 + SaltLength];
                Add(AuthenticationType);
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
