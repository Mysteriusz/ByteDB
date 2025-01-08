using ByteDBServer.Core.Server.Connection.Models;
using ByteDBServer.Core.DataTypes;
using ByteDBServer.Core.Misc;
using System.Linq;
using System;

namespace ByteDBServer.Core.Server.Connection.Packets
{
    internal class ByteDBResponsePacketV1 : ByteDBPacket
    {
        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //

        public NullTerminatedString Username { get; private set; }
        public Int4 Capabilities { get; private set; }
        public Int2 AuthScrambleSize { get; private set; }
        public byte[] AuthScramble { get; private set; }

        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBResponsePacketV1() : base(ByteDBPacketType.ResponsePacket) { }
        public ByteDBResponsePacketV1(byte[] payload) : base(ByteDBPacketType.ResponsePacket, payload) { }
        public ByteDBResponsePacketV1(NullTerminatedString username, Int4 capabilities, Int2 authScrambleSize, byte[] authScramble) : base(ByteDBPacketType.ResponsePacket)
        {
            AddRange(username.Bytes);
            Username = username;

            AddRange(capabilities.Bytes);
            Capabilities = capabilities;
            
            AddRange(authScrambleSize.Bytes);
            AuthScrambleSize = authScrambleSize;
            
            AddRange(authScramble);
            AuthScramble = authScramble;
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

                Username = new NullTerminatedString(packetPayload, 0, out int usernameIndex);
                AddRange(Username.Bytes);

                Capabilities = new Int4(packetPayload, usernameIndex + 1);
                AddRange(Capabilities.Bytes);

                AuthScrambleSize = new Int2(packetPayload, usernameIndex + 5);
                AddRange(AuthScrambleSize.Bytes);

                AuthScramble = packetPayload.Skip(usernameIndex + 7).Take(AuthScrambleSize).ToArray();
                AddRange(AuthScramble);
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
