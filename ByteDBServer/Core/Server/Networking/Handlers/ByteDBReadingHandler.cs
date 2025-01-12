﻿using ByteDBServer.Core.Server.Networking.Models;
using ByteDBServer.Core.Server.Networking.Pools;
using System.Threading.Tasks;

namespace ByteDBServer.Core.Server.Networking.Handlers
{
    internal class ByteDBReadingHandler
    {
        public async Task ExecuteTask(ByteDBReadingTask task)
        {
            try
            {
                await task();
            }
            finally
            {
                ByteDBReadingPool.Handlers.Remove(this);
            }
        }
    }
}
