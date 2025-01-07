using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ByteDBServer.Core.DataTypes;
using ByteDBServer.Core.Server.Connection.Handshake.Custom;

namespace ByteDBServer.Core.Server.Connection.Models
{
    internal abstract class ByteDBPacket : List<byte>, IDisposable
    {
        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //

        /// <summary>
        /// Packet type byte as <see cref="ByteDBPacketType"/>.
        /// </summary>
        public ByteDBPacketType PacketType { get; }

        /// <summary>
        /// Packet Header as <see cref="Int3"/>.
        /// </summary>
        public Int3 Size => new Int3(Payload.Count);

        /// <summary>
        /// Packet payload as <see cref="List{byte}"/>.
        /// </summary>
        public List<byte> Payload { get; } = new List<byte>();

        /// <summary>
        /// Packet Header as <see cref="List{byte}"/>.
        /// </summary>
        public List<byte> Header
        {
            get
            {
                List<byte> _header = new List<byte>();
                _header.Add((byte)PacketType);
                _header.AddRange((byte[])Size);

                return _header;
            }
        }

        /// <summary>
        /// Complete Packet as <see cref="List{byte}"/>.
        /// </summary>
        public List<byte> Packet
        {
            get
            {
                List<byte> _packet = new List<byte>();

                _packet.AddRange(Header);
                _packet.AddRange(Payload);

                return _packet;
            }
        }

        /// <summary>
        /// Static empty <see cref="ByteDBUnknownPacket"/>.
        /// </summary>
        public static ByteDBUnknownPacket Empty { get { return new ByteDBUnknownPacket(); } }

        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBPacket() { }
        public ByteDBPacket(ByteDBPacketType packetType) 
        {
            PacketType = packetType;
        }
        public ByteDBPacket(byte[] payload, ByteDBPacketType packetType)
        {
            AddRange(payload);
            PacketType = packetType;
        }
        public ByteDBPacket(byte[] header, byte[] payload)
        {
            PacketType = (ByteDBPacketType)header[0];
            AddRange(payload);
        }

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        /// <summary>
        /// Synchronously writes packet on provided stream.
        /// </summary>
        /// <param name="stream">Stream on which packet should be written.</param>
        public void Write(Stream stream)
        {
            List<byte> packet = Packet;

            stream.Write(packet.ToArray(), 0, packet.Count);
            stream.Flush();
        }

        /// <summary>
        /// Asynchronously writes packet on provided stream.
        /// </summary>
        /// <param name="stream">Stream on which packet should be written.</param>
        public async Task WriteAsync(Stream stream)
        {
            List<byte> packet = Packet;

            await stream.WriteAsync(packet.ToArray(), 0, packet.Count);
            await stream.FlushAsync();
        }

        //
        // ----------------------------- DISPOSING ----------------------------- 
        //

        private bool _disposed = false;
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    this.Header.Clear();
                    this.Payload.Clear();
                }

                _disposed = true;
            }
        }
        ~ByteDBPacket()
        {
            Dispose(false);
        }

        //
        // ----------------------------- OVERRIDES ----------------------------- 
        //

        public override bool Equals(object obj)
        {
            if (obj is ByteDBPacket packet)
                return packet.Payload == Payload;
            
            return false;
        }
        public override int GetHashCode()
        {
            return Payload.GetHashCode();
        }
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("Header: ");
            builder.Append(BitConverter.ToString(Header.ToArray()));

            builder.AppendLine();

            builder.Append("Payload: ");
            builder.Append(BitConverter.ToString(Payload.ToArray()));

            return builder.ToString();
        }
    }
}
