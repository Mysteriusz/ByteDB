using ByteDBServer.Core.Server.Connection.Models;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ByteDBServer.Core.Server.Connection.Handshake.Packets
{
    internal class ByteDBCustomPacket
    {
        //
        // ----------------------------- PARAMETERS ----------------------------- 
        //

        public List<byte> Header = new List<byte>();
        public List<byte> Payload = new List<byte>();

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
            return (TPacket)constructor.Invoke([Header, Payload]);
        }
    }
}
