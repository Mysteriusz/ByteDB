﻿using ByteDBServer.Core.Server.Networking.Querying.Models;
using ByteDBServer.Core.Server.Networking.Querying;
using ByteDBServer.Core.Server.Networking.Models;
using ByteDBServer.Core.Server.Protocols;
using ByteDBServer.Core.Server.Packets;
using ByteDBServer.Core.Misc.Logs;
using System.Threading.Tasks;
using System;

namespace ByteDBServer.Core.Server.Networking
{
    //
    // ----------------------------- CLASSES ----------------------------- 
    //

    public delegate Task ByteDBReadingTask();
    public delegate Task ByteDBWritingTask();
    public delegate Task ByteDBQueryTask();

    //
    // ----------------------------- TASKS ----------------------------- 
    //

    /// <summary>
    /// Basic server sided tasks that can be executed for client.
    /// </summary>
    internal static class ByteDBTasks
    {
        /// <summary>
        /// Creates a task to disconnect a client and removes their data from active connections.
        /// </summary>
        /// <param name="client">The client to be disconnected.</param>
        /// <returns>A task that disconnects the client and cleans up resources.</returns>
        public static ByteDBReadingTask DisconnectTask(ByteDBClient client)
        {
            return async () =>
            {
                try
                {
                    using (client)
                    {
                        // Remove the client and their data from the active client list
                        ByteDBServerListener.ConnectedClients.Remove(client);

                        if (client.UserData.LoggedCount > 0)
                            client.UserData.LoggedCount--;

                        ByteDBServerLogger.WriteToFile(client.UserData.Logged.ToString());
                        ByteDBServerLogger.WriteToFile(client.UserData.LoggedCount.ToString());
                        ByteDBServerLogger.WriteToFile(client.UserData.Username.ToString());
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
        /// <param name="client">The client to be connected and authenticated.</param>
        /// <returns>A task that connects the client and handles authentication.</returns>
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
                        }
                    }
                }
                catch (Exception ex)
                {
                    ByteDBServerLogger.WriteExceptionToFile(ex);
                }
            };
        }

        /// <summary>
        /// Tokenizes and executes query from client.
        /// </summary>
        /// <param name="query">Query bytes to be executed.</param>
        /// <returns>A task that connects the client and handles authentication.</returns>
        public static ByteDBQueryTask ExecuteQuery(ByteDBClient client, byte[] query, bool sendPacket = true)
        {
            return async () =>
            {
                try
                {
                    using (ByteDBQueryReader qr = new ByteDBQueryReader())
                    {
                        ByteDBQuery parsedQuery = await qr.Read(query);
                        bool success = await ByteDBQueryTasks.Execute(client, parsedQuery);

                        if (!success && sendPacket)
                            using (ByteDBErrorPacket err = new ByteDBErrorPacket("Query failed"))
                                await err.WriteAsync(client);
                        else if (success && sendPacket)
                            using (ByteDBOkayPacket okay = new ByteDBOkayPacket("Query success"))
                                await okay.WriteAsync(client);
                    }
                }
                catch (Exception ex)
                {
                    if (sendPacket)
                        using (ByteDBErrorPacket err = new ByteDBErrorPacket("Query failed"))
                            await err.WriteAsync(client);

                    ByteDBServerLogger.WriteExceptionToFile(ex);
                }
            };
        }
        public static ByteDBQueryTask ExecuteQuery(ByteDBClient client, ByteDBQuery query, bool sendPacket = true)
        {
            return async () =>
            {
                try
                {
                    bool success = await ByteDBQueryTasks.Execute(client, query);
                
                    if (!success && sendPacket)
                        using (ByteDBErrorPacket err = new ByteDBErrorPacket("Query failed"))
                            await err.WriteAsync(client);
                    else if (success && sendPacket)
                        using (ByteDBOkayPacket okay = new ByteDBOkayPacket("Query success"))
                            await okay.WriteAsync(client);
                }
                catch (Exception ex)
                {
                    if (sendPacket)
                        using (ByteDBErrorPacket err = new ByteDBErrorPacket("Query failed"))
                            await err.WriteAsync(client);

                    ByteDBServerLogger.WriteExceptionToFile(ex);
                }
            };
        }
    }
}