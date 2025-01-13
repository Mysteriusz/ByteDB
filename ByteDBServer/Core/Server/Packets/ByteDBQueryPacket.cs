using ByteDBServer.Core.DataTypes;
using ByteDBServer.Core.Misc;
using ByteDBServer.Core.Server.Packets.Models;
using System.Linq;
using System.Windows.Forms;
using System;

namespace ByteDBServer.Core.Server.Packets
{
    internal class ByteDBQueryPacket : ByteDBPacket
    {
        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //

        public NullTerminatedString Query { get; private set; }

        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBQueryPacket() : base(ByteDBPacketType.QueryPacket) { }
        public ByteDBQueryPacket(byte[] payload) : base(ByteDBPacketType.QueryPacket, payload) { }
        public ByteDBQueryPacket(NullTerminatedString query) : base(ByteDBPacketType.QueryPacket)
        {
            AddRange(query.Bytes);
            Query = query;
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

                Query = new NullTerminatedString(packetPayload, 0);
                AddRange(Query.Bytes);

                packetSize.Dispose();
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
            Query?.Dispose();

            base.Dispose();
        }
    }
}
