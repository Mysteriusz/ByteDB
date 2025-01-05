using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using ByteDBServer.Core.Misc.Logs;
using ByteDBServer.Core.Server.Connection.Handshake.Packets;
using System.Linq;

namespace ByteDBServer.Core.Server.Connection.Models
{
    internal abstract class ByteDBProtocol : IDisposable
    {
        //
        // ----------------------------- PARAMETERS ----------------------------- 
        //

        public string Name { get; }
        public byte Version { get; }

        public string CreateProcotolMessage { get { return $"CREATED PROTOCOL: {Name}, VERSION: {Version}"; } }
        public string StartProcotolMessage { get { return $"STARTING PROTOCOL: {Name}, VERSION: {Version}"; } }
        public string EndingProcotolMessage { get { return $"ENDING PROTOCOL: {Name}, VERSION: {Version}"; } }

        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBProtocol(byte version, string name)
        {
            Version = version;
            Name = name;

            ByteDBServerLogger.WriteToFile(CreateProcotolMessage);
        }

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        public abstract void StartProtocol(Stream stream, int responseTimeout = 5);
        public abstract Task StartProtocolAsync(Stream stream, int responseTimeout = 5);

        public virtual void WaitForResponseInTime(Stream stream, int time)
        {
            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                byte[] buffer = new byte[ByteDBServer.BufferSize];

                Task timeoutTask = Task.Delay(TimeSpan.FromSeconds(time), cts.Token);
                Task responseTask = Task.Run(() => stream.Read(buffer, 0, buffer.Length), cts.Token);

                int completedTask = Task.WaitAny(responseTask, timeoutTask);

                if (completedTask == 0)
                {
                    cts.Cancel();

                    ByteDBServerLogger.WriteToFile("RESPONDED IN TIME");
                }
                else
                {
                    ByteDBServerLogger.WriteToFile("NEVER RESPONDED");
                }
            }
        }
        public virtual async Task WaitForResponseInTimeAsync(Stream stream, int time)
        {
            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                byte[] buffer = new byte[ByteDBServer.BufferSize];

                Task timeoutTask = Task.Delay(TimeSpan.FromSeconds(time), cts.Token);
                Task responseTask = stream.ReadAsync(buffer, 0, buffer.Length);

                Task completedTask = await Task.WhenAny(responseTask, timeoutTask);

                if (completedTask == responseTask)
                {
                    cts.Cancel();
                    ByteDBServerLogger.WriteToFile("RESPONDED IN TIME");
                }
                else
                {
                    ByteDBServerLogger.WriteToFile("NEVER RESPONDED");
                }
            }
        }

        //
        // ----------------------------- DISPOSING ----------------------------- 
        //

        private bool _disposed = false;
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {

                }

                _disposed = true;
            }
        }
        ~ByteDBProtocol()
        {
            Dispose(false);
        }
    }
}
