using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using ByteDBServer.Core.Misc.Logs;

namespace ByteDBServer.Core.Server.Networking.Models
{
    internal class ByteDBReadingPool : ByteDBPool<ByteDBReadingHandler, ByteDBReadingTask> { }
    internal class ByteDBWritingPool : ByteDBPool<ByteDBWritingHandler, ByteDBWritingTask> { }
    internal class ByteDBQueryPool : ByteDBPool<ByteDBQueryHandler, ByteDBQueryTask> { }

    internal abstract class ByteDBPool <THandler, TTask>
        where TTask : Delegate
        where THandler : ByteDBHandler<TTask, THandler>, new()
    {
        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //

        public int MaxHandlerCount { get; } = ByteDBServerInstance.HandlerPoolSize;

        public ConcurrentQueue<TTask> TaskQueue = new ConcurrentQueue<TTask>();

        public static List<ByteDBHandler<TTask, THandler>> Handlers = new List<ByteDBHandler<TTask, THandler>>();

        public CancellationTokenSource CancellationToken = new CancellationTokenSource();

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        /// <summary>
        /// Starts processing tasks in the pool by running the processing task asynchronously.
        /// </summary>
        public void StartProcessing()
        {
            _ = ProcessingTask();
        }

        /// <summary>
        /// Stops the task processing by canceling the cancellation token.
        /// </summary>
        public void StopProcessing()
        {
            CancellationToken.Cancel();
        }

        /// <summary>
        /// Enqueues a writing task to the task queue for processing.
        /// </summary>
        /// <param name="task">The writing task to be enqueued</param>
        public void EnqueueTask(TTask task) => TaskQueue.Enqueue(task);

        /// <summary>
        /// The main processing task that dequeues and handles tasks from the task queue.
        /// </summary>
        /// <returns>A Task that represents the ongoing processing of tasks</returns>
        public Task ProcessingTask()
        {
            return Task.Run(async () =>
            {
                try
                {
                    while (true)
                    {
                        if (TaskQueue.Count == 0 || Handlers.Count == MaxHandlerCount)
                        {
                            await ByteDBServerInstance.LongDelayTask;
                            continue;
                        }

                        if (TaskQueue.TryDequeue(out TTask task))
                        {
                            THandler handler = new THandler();
                            Handlers.Add(handler);
                            _ = handler.ExecuteTask(task, Handlers);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ByteDBServerLogger.WriteExceptionToFile(ex);
                }
            }, CancellationToken.Token);
        }
    }
}
