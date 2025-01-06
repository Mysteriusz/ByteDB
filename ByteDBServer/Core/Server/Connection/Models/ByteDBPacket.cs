using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ByteDBServer.Core.DataTypes;
using ByteDBServer.Core.Misc.Logs;
using ByteDBServer.Core.Misc;
using ByteDBServer.Core.Server.Connection.Handshake.Packets;

namespace ByteDBServer.Core.Server.Connection.Models
{
    internal abstract class ByteDBPacket : List<byte>, IDisposable, IByteDBValidator<ByteDBPacket>
    {
        //
        // ----------------------------- PARAMETERS ----------------------------- 
        //

        public static ByteDBCustomPacket Empty { get { return new ByteDBCustomPacket(); } } 
        
        public Int3 Size 
        {
            get 
            {
                return new Int3(Payload.Count); 
            }
        }
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
        public List<byte> Payload 
        { 
            get 
            { 
                return this; 
            }
        }
        public ByteDBPacketType PacketType { get; }

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

        public void Write(Stream stream)
        {
            List<byte> packet = GetPacket(this);

            stream.Write(packet.ToArray(), 0, packet.Count);
            stream.Flush();
        }
        public async Task WriteAsync(Stream stream)
        {
            List<byte> packet = GetPacket(this);

            await stream.WriteAsync(packet.ToArray(), 0, packet.Count);
            await stream.FlushAsync();
        }

        /// <summary>
        /// Validates received packet and writes error packets on given stream if its incorrect.
        /// </summary>
        /// <param name="stream">Stream on which error packets have to be sent.</param>
        /// <param name="packet">Packet to validate.</param>
        /// <returns>True if <paramref name="packet"/> is valid to <typeparamref name="TValidationPacket"/>; False if packet is not valid.</returns>
        public static bool ValidatePacket<TValidationPacket>(Stream stream, ByteDBCustomPacket packet) where TValidationPacket : IByteDBValidator<ByteDBPacket>, new()
        {
            try
            {
                if (packet.IsEmpty)
                    throw new HandshakeTimeoutException(stream, HandshakeTimeoutException.DefaultMessage);

                ByteDBResponsePacketV1 response = packet.AsPacket<ByteDBResponsePacketV1>();

                TValidationPacket validator = new TValidationPacket();
                bool valid = validator.Validate(response);

                if (valid)
                    ByteDBServerLogger.WriteToFile("PACKET IN ORDER");
                else
                    throw new HandshakePacketException(stream, HandshakePacketException.DefaultMessage);
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Method used to get a full packet (Header, Payload).
        /// </summary>
        /// <param name="packet">.</param>
        /// <returns><see cref="List{byte}"/> of bytes in packet.</returns>
        public static List<byte> GetPacket(ByteDBPacket packet)
        {
            List<byte> _packet = new List<byte>();

            _packet.AddRange(packet.Header);
            _packet.AddRange(packet.Payload);
            
            return _packet;
        }

        public abstract bool Validate(ByteDBPacket packet);

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
