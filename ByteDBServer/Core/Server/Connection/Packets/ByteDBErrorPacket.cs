using ByteDBServer.Core.DataTypes;
using ByteDBServer.Core.Misc;
using ByteDBServer.Core.Server.Connection.Models;
using System;
using System.Threading.Tasks;

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

                Message = new NullTerminatedString(bytes, index + 4);
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
