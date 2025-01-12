using ByteDBServer.Core.Server.Networking.Models;
using ByteDBServer.Core.Server.Networking.Pools;
using ByteDBServer.Core.Misc.Logs;
using System.Collections.Generic;
using ByteDBServer.Core.Config;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Linq;
using System.Net;
using System;

namespace ByteDBServer.Core.Server
{
    internal static class ByteDBServerListener
    {
        //
        // ----------------------------------- PROPERTIES -----------------------------------
        //

        public static CancellationTokenSource CancellationToken { get; } = new CancellationTokenSource();
        
        public static Socket Listener { get; } = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public static HashSet<ByteDBClient> ConnectedClients { get; set; } = new HashSet<ByteDBClient>();

        //
        // -------------------------------------- METHODS --------------------------------------
        //

        public static void StartListening()
        {
            Listener.Bind(new IPEndPoint(IPAddress.Parse(ByteDBServerInstance.IpAddress), ByteDBServerInstance.ListeningPort));
            Listener.Listen(0);

            ByteDBReadingPool.StartProcessing();
            ByteDBWritingPool.StartProcessing();

            _ = NewListener();
            _ = ConnectedListener();

            ByteDBServerLogger.WriteToFile("SERVER LISTENING STARTED!");
        }
        public static void StopListening()
        {
            ByteDBWritingPool.StopProcessing();
            ByteDBReadingPool.StopProcessing();

            Listener.Close();    
            CancellationToken.Cancel();

            ByteDBServerLogger.WriteToFile("SERVER LISTENING STOPPED!");
        }

        private async static Task NewListener()
        {
            try
            {
                while (!CancellationToken.IsCancellationRequested)
                {
                    Socket socket = await Listener.AcceptAsync();
                    ByteDBWritingPool.EnqueueTask(ByteDBTasks.ConnectTask(new ByteDBClient(socket)));
                }
            }
            catch (Exception ex)
            {
                await ByteDBServerLogger.WriteExceptionToFileAsync(ex);
            }
        }
        private async static Task ConnectedListener()
        {
            try
            {
                List<Socket> readSockets = new List<Socket>();
                List<Socket> writeSockets = new List<Socket>();

                while (!CancellationToken.IsCancellationRequested)
                {
                    GC.Collect();
                    
                    if (ConnectedClients.Count == 0)
                    {
                        await ByteDBServerConfig.LongDelayTask;
                        continue;
                    }

                    readSockets.Clear();
                    writeSockets.Clear();
                    foreach (ByteDBClient client in ConnectedClients)
                    {
                        writeSockets.Add(client);
                        readSockets.Add(client);
                    }

                    if (readSockets.Count > 0)
                    {
                        Socket.Select(readSockets, writeSockets, null, 1000);
                        if (readSockets.Count > 0)
                            _ = ProcessReading(readSockets);

                        //if (writeSockets.Count > 0)
                        //    _ = ProcessWriting(writeSockets);
                    }

                    await ByteDBServerConfig.ModerateDelayTask;
                }
            }
            catch (Exception ex)
            {
                await ByteDBServerLogger.WriteExceptionToFileAsync(ex);
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

                    if (!IsConnected(client))
                    {
                        ByteDBReadingPool.EnqueueTask(ByteDBTasks.DisconnectTask(client));
                        continue;
                    }

                    int received = socket.Receive(buffer);
                    ByteDBServerLogger.WriteToFile("RECEIVED PACKET");
                }
            });
        }
        private static Task ProcessWriting(List<Socket> sockets)
        {
            return Task.Run(() =>
            {
            });
        }

        public static bool IsAuthenticated(ByteDBClient client)
        {
            return ConnectedClients.Contains(client);
        }
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
