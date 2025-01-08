using ByteDBServer.Core.Server.Connection.Models;
using ByteDBServer.Core.DataTypes;
using ByteDBServer.Core.Misc;
using System.Linq;
using System;

namespace ByteDBServer.Core.Server.Connection
{
    internal class ByteDBErrorPacket : ByteDBPacket
    {
        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //

        public NullTerminatedString Message { get; private set; }

        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBErrorPacket() : base(ByteDBPacketType.ErrorPacket) { }
        public ByteDBErrorPacket(byte[] payload) : base(ByteDBPacketType.ErrorPacket, payload) { }
        public ByteDBErrorPacket(NullTerminatedString message) : base(ByteDBPacketType.ErrorPacket)
        { 
            AddRange(message.Bytes);
            Message = message;
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

                Int3 payloadSize = new Int3(bytes, index + 1);

                // ----------------------------- PAYLOAD ----------------------------- 

                byte[] packetPayload = bytes.Skip(index + 4).Take(payloadSize).ToArray();

                // ----------------------------- TRY TO ASSIGN VALUES ----------------------------- 

                Message = new NullTerminatedString(packetPayload, 0);
                AddRange(Message.Bytes);
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
