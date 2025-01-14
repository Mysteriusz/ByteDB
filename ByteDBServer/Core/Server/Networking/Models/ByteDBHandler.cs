using ByteDBServer.Core.Misc.Logs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ByteDBServer.Core.Server.Networking.Models
{
    internal class ByteDBReadingHandler : ByteDBHandler<ByteDBReadingTask, ByteDBReadingHandler> { }
    internal class ByteDBWritingHandler : ByteDBHandler<ByteDBWritingTask, ByteDBWritingHandler> { }
    internal class ByteDBQueryHandler : ByteDBHandler<ByteDBQueryTask, ByteDBQueryHandler> { }

    internal abstract class ByteDBHandler<TTask, THandler>
        where TTask : Delegate
        where THandler : ByteDBHandler<TTask, THandler>
    {
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
