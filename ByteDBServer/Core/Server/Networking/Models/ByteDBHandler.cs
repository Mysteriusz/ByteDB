using ByteDBServer.Core.Misc.Logs;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace ByteDBServer.Core.Server.Networking.Models
{
    //
    // ----------------------------- CLASSES ----------------------------- 
    //

    internal class ByteDBReadingHandler : ByteDBHandler<ByteDBReadingTask, ByteDBReadingHandler> { }
    internal class ByteDBWritingHandler : ByteDBHandler<ByteDBWritingTask, ByteDBWritingHandler> { }
    internal class ByteDBQueryHandler : ByteDBHandler<ByteDBQueryTask, ByteDBQueryHandler> { }
    
    /// <summary>
    /// Template task handler class.
    /// </summary>
    /// <typeparam name="TTask">Task delegate that can execute.</typeparam>
    /// <typeparam name="THandler">Task handler (class that inherits this <see cref="ByteDBHandler{TTask, THandler}"/>)</typeparam>
    internal abstract class ByteDBHandler<TTask, THandler>
        where TTask : Delegate
        where THandler : ByteDBHandler<TTask, THandler>
    {
        //
        // ----------------------------- METHODS ----------------------------- 
        //

        /// <summary>
        /// Executes a task and removes itself from handler list.
        /// </summary>
        /// <param name="task"><see cref="Delegate"/> task to execute.</param>
        /// <param name="handlers"><see cref="List{ByteDBHandler{TTask, THandler}}"/> handler list containing this handler.</param>
        /// <returns></returns>
        public async Task ExecuteTask(TTask task, List<ByteDBHandler<TTask, THandler>> handlers)
        {
            try
            {
                var method = task.Method;
                var target = task.Target;

                await (Task)method.Invoke(target, null);
            }
            catch (Exception ex)
            {
                ByteDBServerLogger.WriteExceptionToFile(ex);
            }
            finally
            {
                handlers.Remove(this);
            }
        }
    }
}
