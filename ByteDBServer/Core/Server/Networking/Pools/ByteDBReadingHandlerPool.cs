using ByteDBServer.Core.Server.Networking.Handlers;
using ByteDBServer.Core.Server.Networking.Models;
using System.Collections.Concurrent;
using ByteDBServer.Core.Misc.Logs;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace ByteDBServer.Core.Server.Networking
{
    internal static class ByteDBReadingHandlerPool
    {
        public static ConcurrentQueue<ByteDBReadingTask> TaskQueue { get; } = new ConcurrentQueue<ByteDBReadingTask>();

        public static Queue<ByteDBReadingHandler> IdleHandlers { get; } = new Queue<ByteDBReadingHandler>();
        public static Queue<ByteDBReadingHandler> BusyHandlers { get; } = new Queue<ByteDBReadingHandler>();
        
        public static CancellationTokenSource CancellationToken { get; } = new CancellationTokenSource();
        public static Task ProcessingTask { get; private set; }

        public static int HandlerCount => ByteDBServerInstance.HandlerPoolSize;

        public static void InitializePool()
        {
            for (int i = 0; i < HandlerCount; i++)
                IdleHandlers.Enqueue(new ByteDBReadingHandler());
        }

        public static void StartPoolProcessing()
        {
            ProcessingTask = CreateProcessingTask();
        }
        public static void StopPoolProcessing()
        {
            CancellationToken.Cancel();
        }

        private static Task CreateProcessingTask()
        {
            return Task.Run(async () =>
            {
                try
                {
                    while (!CancellationToken.IsCancellationRequested)
                    {
                        if (TaskQueue.TryDequeue(out ByteDBReadingTask task))
                        {
                            var handler = await GetHandler();
                            handler.ExecuteReadingTask(task);

                            IdleHandlers.Enqueue(BusyHandlers.Dequeue());
                        }
                        else
                            await Task.Delay(50);
                    }
                }
                catch (Exception ex)
                {
                    ByteDBServerLogger.WriteExceptionToFile(ex);
                }
            });
        }

        public static async Task<ByteDBReadingHandler> GetHandler()
        {
            while (IdleHandlers.Count == 0)
                await Task.Delay(50);

            ByteDBReadingHandler handler = IdleHandlers.Dequeue();
            BusyHandlers.Enqueue(handler);
            return handler;
        }

        public static void EnqueueTask(ByteDBReadingTask task) => TaskQueue.Enqueue(task);
    }
}
