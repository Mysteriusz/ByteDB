using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using ByteDBServer.Core.Misc.Logs;
using ByteDBServer.Core.Server.Connection.Handshake.Packets;

namespace ByteDBServer.Core.Server.Connection.Models
{
    /// <summary>
    /// Base protocol model with methods for handling custom protocols.
    /// </summary>
    internal abstract class ByteDBProtocol : IDisposable
    {
        //
        // ----------------------------- PARAMETERS ----------------------------- 
        //

        /// <summary>
        /// Protocol name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Protocol version.
        /// </summary>
        public byte Version { get; }

        /// <summary>
        /// Protocol create log message.
        /// </summary>
        public string CreateProcotolMessage => $"CREATED PROTOCOL: {Name}, VERSION: {Version}";

        /// <summary>
        /// Protocol start log message.
        /// </summary>
        public string StartProcotolMessage => $"STARTING PROTOCOL: {Name}, VERSION: {Version}";

        /// <summary>
        /// Protocol end log message.
        /// </summary>
        public string EndingProcotolMessage => $"ENDING PROTOCOL: {Name}, VERSION: {Version}";

        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        /// <summary>
        /// Creates a protocol with given version and name.
        /// </summary>
        /// <param name="version">Protocol Version.</param>
        /// <param name="name">Protocol Name.</param>
        public ByteDBProtocol(byte version, string name)
        {
            Version = version;
            Name = name;

            ByteDBServerLogger.WriteToFile(CreateProcotolMessage);
        }

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        /// <summary>
        /// Synchronously initiates protocol execution on given stream with timeout.
        /// </summary>
        /// <param name="stream">Stream on which protocol should be initiated.</param>
        /// <param name="responseTimeout">Time in seconds in which endpoint has to respond.</param>
        /// <returns>True if the protocol completes successfully; False if an error occurs during execution.</returns>
        public abstract bool StartProtocol(Stream stream, int responseTimeout = 5);

        /// <summary>
        /// Asynchronously initiates protocol execution on given stream with timeout.
        /// </summary>
        /// <param name="stream">Stream on which protocol should be initiated.</param>
        /// <param name="responseTimeout">Time in seconds in which endpoint has to respond.</param>
        /// <returns>True if the protocol completes successfully; False if an error occurs during execution.</returns>
        public abstract Task<bool> StartProtocolAsync(Stream stream, int responseTimeout = 5);

        /// <summary>
        /// Synchronously waits for a response packet on the specified stream within the given timeout period.
        /// </summary>
        /// <param name="stream">The stream to listen for a response.</param>
        /// <param name="seconds">The maximum time, in seconds, to wait for a response.</param>
        /// <returns>A <see cref="ByteDBCustomPacket"/> containing the response data if received in time; otherwise, returns an empty packet.</returns>
        public virtual ByteDBCustomPacket WaitForResponseInTime(Stream stream, int seconds)
        {
            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                // Creates a buffer
                byte[] buffer = new byte[ByteDBServerInstance.BufferSize];

                // Creates a task that completes after the specified time duration or is canceled using the provided CancellationToken.
                Task timeoutTask = Task.Delay(TimeSpan.FromSeconds(seconds), cts.Token);

                // Creates a task that reads data from the stream into the buffer or is canceled using the provided CancellationToken.
                Task responseTask = Task.Run(() => stream.Read(buffer, 0, buffer.Length), cts.Token);

                // Returns an int of the first completed task.
                int completedTask = Task.WaitAny(responseTask, timeoutTask);

                // If first completed task was responseTask then log and return response.
                if (completedTask == 0)
                {
                    cts.Cancel();

                    ByteDBServerLogger.WriteToFile("RESPONDED IN TIME");
                    
                    return new ByteDBCustomPacket(buffer);
                }
                // Else if first completed task was timeoutTask then log and return an empty packet.
                else
                {
                    ByteDBServerLogger.WriteToFile("NEVER RESPONDED");

                    return ByteDBPacket.Empty;
                }
            }
        }

        /// <summary>
        /// Asynchronously waits for a response packet on the specified stream within the given timeout period.
        /// </summary>
        /// <param name="stream">The stream to listen for a response.</param>
        /// <param name="seconds">The maximum time, in seconds, to wait for a response.</param>
        /// <returns>A <see cref="ByteDBCustomPacket"/> containing the response data if received in time; otherwise, returns an empty packet.</returns>
        public virtual async Task<ByteDBCustomPacket> WaitForResponseInTimeAsync(Stream stream, int seconds)
        {
            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                // Creates a buffer
                byte[] buffer = new byte[ByteDBServerInstance.BufferSize];

                // Creates a task that completes after the specified time duration or is canceled using the provided CancellationToken.
                Task timeoutTask = Task.Delay(TimeSpan.FromSeconds(seconds), cts.Token);

                // Creates a task that reads data from the stream into the buffer or is canceled using the provided CancellationToken.
                Task responseTask = stream.ReadAsync(buffer, 0, buffer.Length);

                // Returns an int of the first completed task.
                Task completedTask = await Task.WhenAny(responseTask, timeoutTask);

                // If first completed task was responseTask then log and return response.
                if (completedTask == responseTask)
                {
                    cts.Cancel();

                    ByteDBServerLogger.WriteToFile("RESPONDED IN TIME");

                    return new ByteDBCustomPacket(buffer);
                }
                // Else if first completed task was timeoutTask then log and return an empty packet.
                else
                {
                    ByteDBServerLogger.WriteToFile("NEVER RESPONDED");
                    
                    return ByteDBPacket.Empty;
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
