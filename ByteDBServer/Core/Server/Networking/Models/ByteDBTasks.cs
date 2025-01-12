using ByteDBServer.Core.Server.Protocols;
using ByteDBServer.Core.Misc.Logs;
using System.Threading.Tasks;
using System.Net.Sockets;
using System;

namespace ByteDBServer.Core.Server.Networking.Models
{
    public delegate Task ByteDBReadingTask();
    public delegate Task ByteDBWritingTask();

    internal static class ByteDBTasks
    {
        public static ByteDBReadingTask DisconnectTask(ByteDBClient client)
        {
            return async () =>
            {
                try
                {
                    using (client)
                    {
                        ByteDBServerListener.ConnectedClients.Remove(client);
                        //ByteDBServerLogger.WriteToFile("DISCONNECTED");
                    }
                }
                catch (Exception ex)
                {
                    await ByteDBServerLogger.WriteExceptionToFileAsync(ex);
                }

                await Task.CompletedTask;
            };
        }
        public static ByteDBWritingTask ConnectTask(ByteDBClient client)
        {
            return async () =>
            {
                try
                {
                    using (ByteDBHandshakeV1 protocol = new ByteDBHandshakeV1())
                    {
                        bool success = await protocol.ExecuteProtocolAsync(client.Stream);

                        if (success)
                            ByteDBServerListener.ConnectedClients.Add(client);
                    }
                }
                catch (Exception ex)
                {
                    await ByteDBServerLogger.WriteExceptionToFileAsync(ex);
                }
            };
        }
    }
}
