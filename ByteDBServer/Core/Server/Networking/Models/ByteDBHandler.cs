using ByteDBServer.Core.Server.Connection;
using System.Collections.Generic;

namespace ByteDBServer.Core.Server.Networking.Models
{
    public abstract class ByteDBHandler
    {
        Queue<ByteDBClient> ClientPool { get; } = new Queue<ByteDBClient>();

        public void AddToPool(ByteDBClient client)
        {
            ClientPool.Enqueue(client);
        }
    }
}
