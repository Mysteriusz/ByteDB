using System.Net.Sockets;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace ByteDBServer.Core.Server.Connection
{
    public class ByteDBClient
    {
        public TcpClient Client { get; }
        public Stream ClientStream { get; }

        public int ClientTimeout { get; set; }

        public ByteDBClient(TcpClient client, int timeout = 10)
        {
            Client = client;
            ClientStream = client.GetStream();

            ClientTimeout = timeout;
        }

        public byte[] AwaitData()
        {
            byte[] buffer = new byte[ByteDBServerInstance.BufferSize];
            int bytesRead = ClientStream.Read(buffer, 0, buffer.Length);
            
            return buffer.Take(bytesRead).ToArray();
        }
        public async Task<byte[]> AwaitDataAsync()
        {
            byte[] buffer = new byte[ByteDBServerInstance.BufferSize];
            int bytesRead = await ClientStream.ReadAsync(buffer, 0, buffer.Length);
            
            return buffer.Take(bytesRead).ToArray();
        }
    }
}
