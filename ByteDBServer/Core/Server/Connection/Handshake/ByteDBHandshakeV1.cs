using ByteDBServer.Core.Server.Connection.Models;
using System.IO;

namespace ByteDBServer.Core.Server.Connection.Handshake
{
    internal class ByteDBHandshakeV1 : ByteDBProtocol
    {
        public ByteDBHandshakeV1(Stream stream) : base(1)
        {
            new ByteDBWelcomePacketV1(stream);
        }

        public override void Dispose()
        {

        }
    }
}
