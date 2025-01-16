using ByteDBServer.Core.Server.Networking.Models;
using ByteDBServer.Core.Misc.Logs;
using System.Collections.Generic;
using ByteDBServer.Core.Config;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Linq;
using System.Net;
using System;

namespace ByteDBServer.Core.Server.Networking
{
    /// <summary>
    /// Listens for incoming connections and for incoming connected client packets.
    /// </summary>
    internal static class ByteDBServerListener
    {
        //
        // ----------------------------------- PROPERTIES -----------------------------------
        //

        /// <summary>
        /// Server listening cancellation token.
        /// </summary>
        public static CancellationTokenSource CancellationToken { get; } = new CancellationTokenSource();

        /// <summary>
        /// Server socket listener.
        /// </summary>
        public static Socket Listener { get; } = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        /// <summary>
        /// List of currently connected clients.
        /// </summary>
        public static List<ByteDBClient> ConnectedClients { get; set; } = new List<ByteDBClient>();

        /// <summary>
        /// Pool of reading handlers.
        /// </summary>
        public static ByteDBReadingPool ReadingPool { get; } = new ByteDBReadingPool();

        /// <summary>
        /// Pool of writing handlers.
        /// </summary>
        public static ByteDBWritingPool WritingPool { get; } = new ByteDBWritingPool();

        /// <summary>
        /// Pool of query handlers.
        /// </summary>
        public static ByteDBQueryPool QueryPool { get; } = new ByteDBQueryPool();

        //
        // -------------------------------------- METHODS --------------------------------------
        //

        /// <summary>
        /// Starts listening on <see cref="ByteDBServerInstance.ListeningPort"/> port to connection attempts
        /// </summary>
        public static void StartListening()
        {
            Listener.Bind(new IPEndPoint(IPAddress.Parse(ByteDBServerInstance.IpAddress), ByteDBServerInstance.ListeningPort));
            Listener.Listen(0);

            ReadingPool.StartProcessing();
            WritingPool.StartProcessing();
            QueryPool.StartProcessing();

            _ = NewListener();
            _ = ConnectedListener();

            ByteDBServerLogger.WriteToFile("SERVER LISTENING STARTED!");
        }

        /// <summary>
        /// Stops the server from listening for connections and cleans up resources
        /// </summary>
        public static void StopListening()
        {
            ReadingPool.StopProcessing();
            WritingPool.StopProcessing();
            QueryPool.StopProcessing();

            Listener.Close();    
            CancellationToken.Cancel();

            ByteDBServerLogger.WriteToFile("SERVER LISTENING STOPPED!");
        }

        /// <summary>
        /// Listens for new connections and enqueues tasks to handle the connected clients.
        /// </summary>
        private async static Task NewListener()
        {
            try
            {
                // Continues listening for new connections until cancellation is requested
                while (!CancellationToken.IsCancellationRequested)
                {
                    // Asynchronously accepts a new incoming connection from the listener
                    Socket socket = await Listener.AcceptAsync();

                    // Enqueues a task to handle the new connection using the ByteDBWritingPool
                    WritingPool.EnqueueTask(ByteDBTasks.ConnectTask(new ByteDBClient(socket)));
                }
            }
            catch (Exception ex)
            {
                ByteDBServerLogger.WriteExceptionToFile(ex);
            }
        }

        /// <summary>
        /// Handles the connected clients, managing read and write operations for them.
        /// </summary>
        private async static Task ConnectedListener()
        {
            try
            {
                // Lists to hold the sockets that need to be read from or written to
                List<Socket> readSockets = new List<Socket>();
                List<Socket> writeSockets = new List<Socket>();

                while (!CancellationToken.IsCancellationRequested)
                {
                    // Forces garbage collection to free up memory resources 
                    GC.Collect();

                    // If there are no connected clients, wait before checking again
                    if (ConnectedClients.Count == 0)
                    {
                        await ByteDBServerConfig.LongDelayTask;
                        continue;
                    }

                    // Clears the lists for the next iteration
                    readSockets.Clear();
                    writeSockets.Clear();

                    // Adds all connected clients sockets to the read and write lists
                    foreach (ByteDBClient client in ConnectedClients)
                    {
                        writeSockets.Add(client);
                        readSockets.Add(client);
                    }

                    // If there are any sockets to read or write to, proceed with select operation
                    if (readSockets.Count > 0)
                    {
                        // Iterate to remove all dead sockets
                        for (int i = readSockets.Count - 1; i >= 0; i--)
                        {
                            if (!readSockets[i].Connected)
                            {
                                readSockets.RemoveAt(i);
                                ConnectedClients.RemoveAt(i);
                            }
                        }

                        // Continue if after cleanup there is no sockets
                        if (readSockets.Count == 0)
                            continue;

                        // Perform a non-blocking select on the sockets for reading and writing
                        Socket.Select(readSockets, null, null, 1000);

                        // If there are sockets ready to be read from, process them asynchronously
                        if (readSockets.Count > 0)
                            _ = ProcessReading(readSockets);
                    }

                    await ByteDBServerConfig.ModerateDelayTask;
                }
            }
            catch (Exception ex)
            {
                ByteDBServerLogger.WriteExceptionToFile(ex);
            }
        }

        private static Task ProcessReading(List<Socket> sockets)
        {
            return Task.Run(() =>
            {
                byte[] buffer = new byte[ByteDBServerInstance.BufferSize];
                foreach (var socket in sockets)
                {
                    ByteDBClient client = ConnectedClients.FirstOrDefault(c => c.Socket == socket);

                    // If clients socket is disconnected
                    if (!IsConnected(client))
                    {
                        ReadingPool.EnqueueTask(ByteDBTasks.DisconnectTask(client));
                        continue;
                    }

                    int received = socket.Receive(buffer);

                    ByteDBServerLogger.WriteToFile("PACKET RECEIVED");

                    // Execute clients query if it requests CLIENT_REQUESTS_QUERY_HANDLING  and packet is a QueryPacketType (0x04)
                    if (client.RequestedCapabilities.Contains(ServerCapabilities.CLIENT_REQUESTS_QUERY_HANDLING ) && buffer[0] == (byte)ByteDBPacketType.QUERY_PACKET)
                    {
                        QueryPool.EnqueueTask(ByteDBTasks.ExecuteQuery(client, buffer.Take(received).ToArray()));
                        continue;
                    }
                }
            });
        }
        private static Task ProcessWriting(List<Socket> sockets)
        {
            return Task.Run(() =>
            {
            });
        }

        /// <summary>
        /// Checks if client is contained in <see cref="ConnectedClients"/>.
        /// </summary>
        /// <param name="client">Client to check.</param>
        /// <returns>True if is authenticated; False if not.</returns>
        public static bool IsAuthenticated(ByteDBClient client)
        {
            return ConnectedClients.Contains(client);
        }

        /// <summary>
        /// Sends a check packet to socket.
        /// </summary>
        /// <param name="socket">Socket to check.</param>
        /// <returns>True if is connected; False if not.</returns>
        private static bool IsConnected(Socket socket)
        {
            try
            {
                byte[] temp = new byte[1];
                socket.Send(temp, 0, 0);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
