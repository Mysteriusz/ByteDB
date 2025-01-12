using ByteDBServer.Core.Server.Networking.Handlers;
using ByteDBServer.Core.Server.Networking.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace ByteDBServer.Core.Server.Networking.Pools
{
    internal static class ByteDBWritingPool
    {
        public static int MaxHandlerCount { get; } = ByteDBServerInstance.HandlerPoolSize;

        public static ConcurrentQueue<ByteDBWritingTask> TaskQueue = new ConcurrentQueue<ByteDBWritingTask>();

        public static List<ByteDBWritingHandler> Handlers = new List<ByteDBWritingHandler>();

        public static CancellationTokenSource CancellationToken = new CancellationTokenSource();

        public static void StartProcessing()
        {
            _ = ProcessingTask();
        }
        public static void StopProcessing()
        {
            CancellationToken.Cancel();
        }
        public static void EnqueueTask(ByteDBWritingTask task) => TaskQueue.Enqueue(task);

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

                    if (TaskQueue.TryDequeue(out ByteDBWritingTask task))
                    {
                        var handler = new ByteDBWritingHandler();
                        Handlers.Add(handler);
                        _ = handler.ExecuteTask(task);
                    }
                }
            }, CancellationToken.Token);
        }
    }
}
