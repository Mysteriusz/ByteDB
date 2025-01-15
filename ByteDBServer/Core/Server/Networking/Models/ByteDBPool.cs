using System.Collections.Concurrent;
using ByteDBServer.Core.Misc.Logs;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace ByteDBServer.Core.Server.Networking.Models
{
    //
    // ----------------------------- CLASSES ----------------------------- 
    //

    internal class ByteDBReadingPool : ByteDBPool<ByteDBReadingHandler, ByteDBReadingTask> { }
    internal class ByteDBWritingPool : ByteDBPool<ByteDBWritingHandler, ByteDBWritingTask> { }
    internal class ByteDBQueryPool : ByteDBPool<ByteDBQueryHandler, ByteDBQueryTask> { }

    /// <summary>
    /// Pool of handlers that execute tasks.
    /// </summary>
    /// <typeparam name="THandler"><see cref="ByteDBHandler{TTask, THandler}"/> inheriting class.</typeparam>
    /// <typeparam name="TTask"><see cref="Delegate"/> class that has to be executed.</typeparam>
    internal abstract class ByteDBPool <THandler, TTask>
        where TTask : Delegate
        where THandler : ByteDBHandler<TTask, THandler>, new()
    {
        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //

        /// <summary>
        /// Returns <see cref="ByteDBServerInstance.HandlerPoolSize"/>.
        /// </summary>
        public int MaxHandlerCount { get; } = ByteDBServerInstance.HandlerPoolSize;

        /// <summary>
        /// Currently queued tasks.
        /// </summary>
        public ConcurrentQueue<TTask> TaskQueue = new ConcurrentQueue<TTask>();

        /// <summary>
        /// List of currently working handlers.
        /// </summary>
        public static List<ByteDBHandler<TTask, THandler>> Handlers = new List<ByteDBHandler<TTask, THandler>>();

        /// <summary>
        /// Processing cancellation token.
        /// </summary>
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
