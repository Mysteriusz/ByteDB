using System.Net.Sockets;
using System.IO;

namespace ByteDBServer.Core.Server.Connection
{
    internal class ByteDBClient
    {
        public TcpClient Client { get; }
        public Stream ClientStream { get; }
    
        public ByteDBClient()
        {

        }
    }
}
