using ByteDBServer.Core.DataTypes;
using ByteDBServer.Core.Misc;
using ByteDBServer.Core.Server.Connection.Models;
using System.Linq;
using System;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

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
