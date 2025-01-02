using System.Collections.Generic;
using System.IO;

namespace ByteDBServer.Core.Server.Connection.Models
{
    internal class ByteDBPacket : List<byte>
    {
        public int Size { get; set; }

        public List<byte> Header { get; }
        public List<byte> Payload { get; set; }

        public ByteDBPacket() { }
        public ByteDBPacket(IEnumerable<byte> payload)
        {
            payload = Payload;
        }

        public void Write(Stream stream)
        {
            
        }
        public void Read()
        {

        }
    }
}
