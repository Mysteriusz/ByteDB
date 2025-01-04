using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ByteDBServer.Core.Misc;
using ByteDBServer.Core.Server.Connection.Handshake.Packets;
using ByteDBServer.Core.Server.Connection.Models;

namespace ByteDBServer.Core.Server.Connection.Handshake
{
    internal class ByteDBHandshakeV1 : ByteDBProtocol
    {
        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBHandshakeV1(Stream stream, int timeout = 10) : base(1)
        {
            try
            {
                new ByteDBWelcomePacketV1(stream, "WelcomeToByteDB");

                Task response = AwaitResponseAsync(stream, timeout);
                response.Wait();

                new ByteDBOkayPacket(stream, "ConnectionSuccessfull");
            }
            catch (Exception ex)
            {
                new ByteDBErrorPacket(stream, ex.Message);
            }
        }

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        private async Task AwaitResponseAsync(Stream stream, int time)
        {
            using (CancellationTokenSource cts = new CancellationTokenSource()) 
            {
                cts.CancelAfter(TimeSpan.FromSeconds(time));

                byte[] buffer = new byte[ByteDBServer.BufferSize];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cts.Token);

                if (bytesRead > 0)
                {
                    try
                    {
                        new ByteDBResponsePacketV1(stream, buffer);
                    }
                    catch
                    {
                        throw new HandshakePacketException();
                    }
                }
                else
                    throw new HandshakeTimeoutException();
            }
        }
    }
}
