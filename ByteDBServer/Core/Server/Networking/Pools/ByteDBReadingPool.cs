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
        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //

        public static int MaxHandlerCount { get; } = ByteDBServerInstance.HandlerPoolSize;

        public static ConcurrentQueue<ByteDBReadingTask> TaskQueue = new ConcurrentQueue<ByteDBReadingTask>();

        public static List<ByteDBReadingHandler> Handlers = new List<ByteDBReadingHandler>();

        public static CancellationTokenSource CancellationToken = new CancellationTokenSource();

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        /// <summary>
        /// Starts processing tasks in the pool by running the processing task asynchronously.
        /// </summary>
        public static void StartProcessing()
        {
            _ = ProcessingTask();
        }

        /// <summary>
        /// Stops the task processing by canceling the cancellation token.
        /// </summary>
        public static void StopProcessing()
        {
            CancellationToken.Cancel();
        }

        /// <summary>
        /// Enqueues a writing task to the task queue for processing.
        /// </summary>
        /// <param name="task">The writing task to be enqueued</param>
        public static void EnqueueTask(ByteDBReadingTask task) => TaskQueue.Enqueue(task);

        /// <summary>
        /// The main processing task that dequeues and handles tasks from the task queue.
        /// </summary>
        /// <returns>A Task that represents the ongoing processing of tasks</returns>
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
