using ByteDBServer.Core.Server.Packets.Models;
using ByteDBServer.Core.DataTypes;
using ByteDBServer.Core.Misc;
using System.Linq;
using System;

namespace ByteDBServer.Core.Server.Packets
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

        public ByteDBErrorPacket() : base(ByteDBPacketType.ERROR_PACKET) { }
        public ByteDBErrorPacket(byte[] payload) : base(ByteDBPacketType.ERROR_PACKET, payload) { }
        public ByteDBErrorPacket(NullTerminatedString message) : base(ByteDBPacketType.ERROR_PACKET)
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

                payloadSize.Dispose();
                packetPayload = null;
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

        public override void Dispose()
        {
            Message?.Dispose();

            base.Dispose();
        }
    }
}
