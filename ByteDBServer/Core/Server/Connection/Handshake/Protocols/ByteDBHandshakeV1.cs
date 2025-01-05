using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using ByteDBServer.Core.Misc;
using ByteDBServer.Core.Misc.Logs;
using ByteDBServer.Core.Server.Connection.Handshake.Packets;
using ByteDBServer.Core.Server.Connection.Models;

namespace ByteDBServer.Core.Server.Connection.Handshake
{
    internal class ByteDBHandshakeV1 : ByteDBProtocol
    {
        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBHandshakeV1() : base(1, "HandshakeV1") { }

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        public override void StartProtocol(Stream stream, int timeout = 5)
        {
            ByteDBServerLogger.WriteToFile(StartProcotolMessage);

            var welcomePacket = new ByteDBWelcomePacketV1(ByteDBServer.ServerWelcomePacketMessage);
            welcomePacket.Write(stream);

            Task waitForResponse = WaitForResponseInTime(stream, timeout);
        }

        private async Task WaitForResponseInTime(Stream stream, int time)
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
    }
}
