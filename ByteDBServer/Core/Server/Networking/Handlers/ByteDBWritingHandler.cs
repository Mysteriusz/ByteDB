using ByteDBServer.Core.Server.Networking.Models;
using ByteDBServer.Core.Server.Networking.Pools;
using System.Threading.Tasks;

namespace ByteDBServer.Core.Server.Networking.Handlers
{
    internal class ByteDBWritingHandler
    {
        //
        // ----------------------------- METHODS ----------------------------- 
        //

        /// <summary>
        /// Executes a task and awaits for it`s result.
        /// </summary>
        /// <param name="task"><see cref="ByteDBWritingTask"/> which has to be executed.</param>
        /// <returns></returns>
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
