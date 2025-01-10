using ByteDBServer.Core.Server.Networking.Models;
using ByteDBServer.Core.Server.Protocols;
using ByteDBServer.Core.Misc.Logs;
using System.Threading.Tasks;
using System.Net.Sockets;
using System;

namespace ByteDBServer.Core.Server.Networking.Handlers
{
    public delegate Task ByteDBReadingTask();

    internal class ByteDBReadingHandler
    {
        private readonly object _lock = new object();

        public bool IsBusy { get; private set; }

        public void ExecuteReadingTask(ByteDBReadingTask task)
        {
            Task.Run(async () =>
            {
                if (IsBusy) await WaitForHandlerToBecomeFree();

                lock (_lock)
                    IsBusy = true;

                try
                {
                    await task();
                }
                catch (Exception ex)
                {
                    ByteDBServerLogger.WriteExceptionToFile(ex);
                }
                finally
                {
                    lock (_lock)
                        IsBusy = false;
                }
            });
        }

        private async Task WaitForHandlerToBecomeFree()
        {
            while (IsBusy)
                await Task.Delay(50);
        }
    }
}
