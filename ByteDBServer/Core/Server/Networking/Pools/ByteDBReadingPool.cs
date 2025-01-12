using ByteDBServer.Core.Server.Networking.Handlers;
using ByteDBServer.Core.Server.Networking.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace ByteDBServer.Core.Server.Networking.Pools
{
    internal static class ByteDBReadingPool
    {
        public static int MaxHandlerCount { get; } = ByteDBServerInstance.HandlerPoolSize;

        public static ConcurrentQueue<ByteDBReadingTask> TaskQueue = new ConcurrentQueue<ByteDBReadingTask>();

        public static List<ByteDBReadingHandler> Handlers = new List<ByteDBReadingHandler>();

        public static CancellationTokenSource CancellationToken = new CancellationTokenSource();

        public static void StartProcessing()
        {
            _ = ProcessingTask();
        }
        public static void StopProcessing()
        {
            CancellationToken.Cancel();
        }
        public static void EnqueueTask(ByteDBReadingTask task) => TaskQueue.Enqueue(task);

        public static Task ProcessingTask()
        {
            return Task.Run(async () =>
            {
                while (true)
                {
                    if (TaskQueue.Count == 0 || Handlers.Count == MaxHandlerCount)
                    {
                        await ByteDBServerInstance.LongDelayTask;
                        continue;
                    }

                    if (TaskQueue.TryDequeue(out ByteDBReadingTask task))
                    {
                        var handler = new ByteDBReadingHandler();
                        Handlers.Add(handler);
                        _ = handler.ExecuteTask(task);
                    }
                }
            }, CancellationToken.Token);
        }
    }
}
