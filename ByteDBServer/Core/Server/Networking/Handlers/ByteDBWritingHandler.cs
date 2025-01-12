using ByteDBServer.Core.Server.Networking.Models;
using ByteDBServer.Core.Server.Networking.Pools;
using System.Threading.Tasks;

namespace ByteDBServer.Core.Server.Networking.Handlers
{
    internal class ByteDBWritingHandler
    {
        public async Task ExecuteTask(ByteDBWritingTask task)
        {
            try
            {
                await task();
            }
            finally
            {
                ByteDBWritingPool.Handlers.Remove(this);
            }
        }
    }
}
