using ByteDBServer.Core.Server.Networking.Models;
using ByteDBServer.Core.Server.Networking.Pools;
using System.Threading.Tasks;

namespace ByteDBServer.Core.Server.Networking.Handlers
{
    internal class ByteDBReadingHandler
    {
        //
        // ----------------------------- METHODS ----------------------------- 
        //

        /// <summary>
        /// Executes a task and awaits for it`s result.
        /// </summary>
        /// <param name="task"><see cref="ByteDBReadingTask"/> which has to be executed.</param>
        /// <returns></returns>
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
