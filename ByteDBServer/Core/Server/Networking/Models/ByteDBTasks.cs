using ByteDBServer.Core.Server.Protocols;
using ByteDBServer.Core.Misc.Logs;
using System.Threading.Tasks;
using System.Net.Sockets;
using System;
using ByteDBServer.Core.Authentication;

namespace ByteDBServer.Core.Server.Networking.Models
{
    public delegate Task ByteDBReadingTask();
    public delegate Task ByteDBWritingTask();

    //
    // ----------------------------- TASKS ----------------------------- 
    //

    internal static class ByteDBTasks
    {
        /// <summary>
        /// Creates a task to disconnect a client and removes their data from active connections.
        /// </summary>
        /// <param name="client">The client to be disconnected</param>
        /// <returns>A task that disconnects the client and cleans up resources</returns>
        public static ByteDBReadingTask DisconnectTask(ByteDBClient client)
        {
            return async () =>
            {
                try
                {
                    using (client)
                    {
                        // Remove the client and their data from the active client list
                        ByteDBServerListener.ConnectedClients.Add(client);
                        ByteDBAuthenticator.ActiveUsers.Add(client.UserData);
                    }
                }
                catch (Exception ex)
                {
                    ByteDBServerLogger.WriteExceptionToFile(ex);
                }

                await Task.CompletedTask;
            };
        }

        /// <summary>
        /// Creates a task to connect a client, authenticate them, and add them to active connections.
        /// </summary>
        /// <param name="client">The client to be connected and authenticated</param>
        /// <returns>A task that connects the client and handles authentication</returns>
        public static ByteDBWritingTask ConnectTask(ByteDBClient client)
        {
            return async () =>
            {
                try
                {
                    using (ByteDBHandshakeV1 protocol = new ByteDBHandshakeV1())
                    {
                        // await for authentication result
                        bool success = await protocol.ExecuteProtocolAsync(client);

                        // if authentication was successful
                        if (success)
                        {
                            // Add the client and their data to the active connections list
                            ByteDBServerListener.ConnectedClients.Add(client);
                            ByteDBAuthenticator.ActiveUsers.Add(client.UserData);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ByteDBServerLogger.WriteExceptionToFile(ex);
                }
            };
        }
    }
}
