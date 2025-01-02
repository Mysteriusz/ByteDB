using ByteDBServer.Core.Server.Connection.Models;
using System.IO;

namespace ByteDBServer.Core.Server.Connection.Handshake
{
    internal class ByteDBHandshakeV1 : ByteDBProtocol
    {
        public ByteDBHandshakeV1(Stream stream, int bufferSize)
        {
            byte[] buffer = new byte[bufferSize];
            int bytesRead = stream.Read(buffer, 0, bufferSize);

            if (bytesRead > 0)
            {

            }
        }
    }
}
