using ByteDBServer.Core.Server.Networking.Handlers;
using ByteDBServer.Core.Server.Protocols;
using ByteDBServer.Core.Misc.Logs;
using System.Net.Sockets;
using System;

namespace ByteDBServer.Core.Server.Networking.Models
{
    internal static class ByteDBTasks
    {
        public static ByteDBReadingTask NewConnectionTask(Socket socket)
        {
            return async () =>
            {
                try
                {
                    ByteDBClient client = new ByteDBClient(socket);
                    bool success = await new ByteDBHandshakeV1().ExecuteProtocolAsync(client.Stream);

                    if (success)
                        ByteDBServerListener.ConnectedClients.Add(client);
                }
                catch (Exception ex)
                {
                    ByteDBServerLogger.WriteExceptionToFile(ex);
                }
            };
        }
    }
}
