using ByteDBServer.Core.Server.Connection.Models;
using System.Collections.Generic;
using System.Linq;

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
    }
}
