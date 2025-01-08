using ByteDBServer.Core.Server.Packets.Models;
using ByteDBServer.Core.DataTypes;
using ByteDBServer.Core.Misc;
using System.Linq;
using System;

namespace ByteDBServer.Core.Server.Packets
{
    internal class ByteDBOkayPacket : ByteDBPacket
    {
        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //

        public NullTerminatedString Message { get; private set; }

        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBOkayPacket() : base(ByteDBPacketType.OkayPacket) { }
        public ByteDBOkayPacket(byte[] payload) : base(ByteDBPacketType.OkayPacket, payload) { }
        public ByteDBOkayPacket(NullTerminatedString message) : base(ByteDBPacketType.OkayPacket)
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

                Int3 packetSize = new Int3(bytes, index + 1);

                // ----------------------------- PAYLOAD ----------------------------- 

                byte[] packetPayload = bytes.Skip(index + 4).Take(packetSize).ToArray();

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
