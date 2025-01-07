using System.Collections.Generic;

namespace ByteDBServer.Core.Server.Connection.Handshake.Custom
{
    internal class ByteDBUnknownPacket
    {
        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //

        public List<byte> Header { get; private set; }
        public List<byte> Payload { get; private set; }

        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBUnknownPacket() { }
        public ByteDBUnknownPacket(byte[] payload) { Payload.AddRange(payload); }
        public ByteDBUnknownPacket(byte[] header, byte[] payload) { Header.AddRange(header); Payload.AddRange(payload); }
    }
}
