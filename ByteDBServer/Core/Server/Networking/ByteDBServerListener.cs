using ByteDBServer.Core.Misc.Logs;
using ByteDBServer.Core.Server.Networking;
using ByteDBServer.Core.Server.Networking.Handlers;
using ByteDBServer.Core.Server.Networking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ByteDBServer.Core.Server
{
    internal static class ByteDBServerListener
    {
        //
        // ----------------------------------- PROPERTIES -----------------------------------
        //

        public static CancellationTokenSource CancellationToken { get; } = new CancellationTokenSource();
        
        public static Socket Listener { get; } = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        
        public static List<ByteDBClient> ConnectedClients { get; set; } = new List<ByteDBClient>();

        //
        // -------------------------------------- METHODS --------------------------------------
        //

        public static void StartListening()
        {
            ByteDBReadingHandlerPool.InitializePool();
            ByteDBReadingHandlerPool.StartPoolProcessing();

            Listener.Bind(new IPEndPoint(IPAddress.Parse(ByteDBServerInstance.IpAddress), ByteDBServerInstance.ListeningPort));

            Listener.Listen(10);
            _ = ListeningTask();

            ByteDBServerLogger.WriteToFile("SERVER LISTENING STARTED!");
        }
        public static void StopListening()
        {
            ByteDBReadingHandlerPool.StopPoolProcessing();

            Listener.Close();    
            CancellationToken.Cancel();

            ByteDBServerLogger.WriteToFile("SERVER LISTENING STOPPED!");
        }

        private static Task ListeningTask()
        {
            return Task.Run(async () =>
            {
                try
                {
                    while (!CancellationToken.IsCancellationRequested)
                    {
                        // Get the list of sockets from connected clients
                        var connectedSockets = ConnectedClients.Select(c => c.Socket).ToList();

                        // Add listener socket to both reading and writing requests
                        var readingRequests = new List<Socket>(connectedSockets) { Listener };
                        var writingRequests = new List<Socket>(connectedSockets) { Listener };

                        // Perform select with a 1-second timeout
                        Socket.Select(readingRequests, writingRequests, null, 1000);

                        // Handle readable sockets (those that have data to read)
                        _ = ReadingHandle(readingRequests);
                    }
                }
                catch (Exception ex)
                {
                    ByteDBServerLogger.WriteExceptionToFile(ex);
                }
            });
        }

        private static Task ReadingHandle(List<Socket> sockets)
        {
            return Task.Run(async () =>
            {
                foreach (var socket in sockets)
                {
                    if (!IsConnected(socket))
                    {
                        var client = await Listener.AcceptAsync();
                        ByteDBReadingHandlerPool.EnqueueTask(ByteDBTasks.NewConnectionTask(client));
                    }
                }
                await Task.CompletedTask;
            });
        }

        private static bool IsConnected(Socket socket) => ConnectedClients.Select(c => c.Socket).Contains(socket);
    }
}
