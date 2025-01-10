using ByteDBServer.Core.Server.Networking.Handlers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using ByteDBServer.Core.Misc.Logs;
using System;

namespace ByteDBServer.Core.Server.Networking
{
    internal static class ByteDBReadingHandlerPool
    {
        public static ConcurrentQueue<ByteDBReadingTask> TaskQueue { get; } = new ConcurrentQueue<ByteDBReadingTask>();
        public static List<ByteDBReadingHandler> Handlers { get; } = new List<ByteDBReadingHandler>();
        
        public static CancellationTokenSource CancellationToken { get; } = new CancellationTokenSource();
        public static Task ProcessingTask { get; private set; }

        public static int HandlerCount => ByteDBServerInstance.HandlerPoolSize;

        public static void InitializePool()
        {
            for (int i = 0; i < HandlerCount; i++)
                Handlers.Add(new ByteDBReadingHandler());
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
                            GetLeastProcessed().ExecuteReadingTask(task);
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
        public static ByteDBReadingHandler GetLeastProcessed()
        {
            foreach (var handler in Handlers)
                if (!handler.IsBusy)
                    return handler;

            return Handlers[0];
        }

        public static void EnqueueTask(ByteDBReadingTask task) => TaskQueue.Enqueue(task);
    }
}
