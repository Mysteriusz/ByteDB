using ByteDBServer.Core.Server.Networking.Handlers;
using ByteDBServer.Core.Server.Connection;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace ByteDBServer.Core.Server
{
    internal static class ByteDBServerListener
    {
        //
        // ----------------------------------- PROPERTIES -----------------------------------
        //

        public static List<ByteDBClient> ConnectedClients { get; } = new List<ByteDBClient>();
        
        public static TcpListener Listener { get; } = new TcpListener(IPAddress.Parse(ByteDBServerInstance.IpAddress), ByteDBServerInstance.ListeningPort);
        public static CancellationTokenSource CancellationToken { get; } = new CancellationTokenSource();
       
        public static Task ClientsTask { get; } = AwaitClients();

        public static ByteDBConnectedClientHandler ConnectedClientsHandler { get; }
        public static ByteDBNewClientHandler NewClientsHandler { get; }

        //
        // -------------------------------------- METHODS --------------------------------------
        //

        public static void StartListening()
        {
            ThreadPool.SetMinThreads(ByteDBServerInstance.MinThreadPoolSize, ByteDBServerInstance.MinThreadPoolSize);
            ThreadPool.SetMaxThreads(ByteDBServerInstance.MaxThreadPoolSize, ByteDBServerInstance.MaxThreadPoolSize);

            Listener.Start();
            ClientsTask.Start();
        }
        public static void StopListening()
        {
            Listener.Stop();
            ClientsTask.Dispose();
        }

        private async static Task AwaitClients()
        {
            while (!CancellationToken.IsCancellationRequested)
            {
                ByteDBClient client = new ByteDBClient(await Listener.AcceptTcpClientAsync());

                if (IsConnected(client))
                    ConnectedClientsHandler.AddToPool(client);
                else
                    NewClientsHandler.AddToPool(client);
            }
        }

        private static bool IsConnected(ByteDBClient client) => ConnectedClients.Contains(client);
    }
}
