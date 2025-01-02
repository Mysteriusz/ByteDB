using ByteDBServer.Core.Server.Connection.Models;
using System.IO;

namespace ByteDBServer.Core.Server.Connection.Handshake
{
    internal class ByteDBWelcomePacketV1 : ByteDBPacket
    {
        public ByteDBWelcomePacketV1(Stream stream, int bufferSize)
        {
            this.Write(stream);
        }

        public override void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}
