using ByteDBServer.Core.Server.Connection.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ByteDBServer.Core.Server.Connection.Handshake.Packets
{
    internal class ByteDBCustomPacket
    {
        //
        // ----------------------------- PARAMETERS ----------------------------- 
        //

        public List<byte> Header = new List<byte>();
        public List<byte> Payload = new List<byte>();

        public bool IsEmpty => Header.Count == 0 || Payload.Count == 0;

        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBCustomPacket() { }
        public ByteDBCustomPacket(byte[] packet) { Header = packet.Take(4).ToList(); Payload = packet.Skip(4).ToList(); }
        public ByteDBCustomPacket(byte[] header, byte[] payload) { Header = header.ToList(); Payload = payload.ToList(); }

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        public TPacket AsPacket<TPacket>() where TPacket : ByteDBPacket, new()
        {
            var constructor = typeof(TPacket).GetConstructor([typeof(byte[]), typeof(byte[])]);
            return (TPacket)constructor.Invoke([Header.ToArray(), Payload.ToArray()]);
        }

        //
        // ----------------------------- OVERRIDES ----------------------------- 
        //

        public override bool Equals(object obj)
        {
            if (obj is ByteDBCustomPacket packet)
                return packet.Header.SequenceEqual(Header) && packet.Payload.SequenceEqual(Payload);

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
