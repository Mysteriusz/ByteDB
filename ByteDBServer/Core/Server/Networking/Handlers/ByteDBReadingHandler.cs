using ByteDBServer.Core.Server.Networking.Models;
using ByteDBServer.Core.Misc.Logs;
using System.Threading.Tasks;
using System;

namespace ByteDBServer.Core.Server.Networking.Handlers
{
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
