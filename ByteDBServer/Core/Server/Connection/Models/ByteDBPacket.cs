using DataTypesTesting.DataTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ByteDBServer.Core.Server.Connection.Models
{
    internal abstract class ByteDBPacket : List<byte>, IDisposable
    {
        //
        // ----------------------------- CONSTANTS ----------------------------- 
        //

        public const ByteDBPacketType PacketType = ByteDBPacketType.WelcomePacket;

        //
        // ----------------------------- PARAMETERS ----------------------------- 
        //

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

        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBPacket() { }
        public ByteDBPacket(byte[] payload)
        {
            AddRange(payload);
        }

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        public void Write(Stream stream)
        {
            List<byte> packet = GetPacket(this);
            stream.Write(packet.ToArray(), 0, packet.Count);
        }
        public static List<byte> GetPacket(ByteDBPacket packet)
        {
            List<byte> _packet = new List<byte>();

            _packet.AddRange(packet.Header);
            _packet.AddRange(packet.Payload);
            
            return _packet;
        }

        public abstract void Dispose();

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
            builder.Append(string.Join(", ", Header));

            builder.AppendLine();

            builder.Append("Payload: ");
            builder.Append(string.Join(", ", Payload));


            return builder.ToString();
        }
    }
}
