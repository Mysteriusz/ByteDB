using System.Net.Sockets;
using System.Linq;

namespace ByteDBServer.Core.Server.Networking.Models
{
    internal class ByteDBClient
    {
        public Socket Socket { get; }
        public NetworkStream Stream { get; }

        public ByteDBClient(Socket socket) { Socket = socket; Stream = new NetworkStream(Socket, false); }
    }
}
