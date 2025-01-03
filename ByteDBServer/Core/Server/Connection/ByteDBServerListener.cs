using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;
using ByteDBServer.Core.Server.Connection.Handshake;

namespace ByteDBServer.Core.Server
{
    internal class ByteDBServerListener
    {
        //
        // ----------------------------------- PROPERTIES -----------------------------------
        //
        
        public static TcpListener Listener { get; private set; }
        public static List<TcpClient> ConnectedClients { get; private set; }

        public static CancellationTokenSource CancellationToken { get; } = new CancellationTokenSource();
        public static int ListeningDelay { get; private set; } = 1000;
        public static int BufferSize { get; private set; } = 2048;

        //
        // ----------------------------------- CONSTRUCTORS -----------------------------------
        //

        public ByteDBServerListener(string address, int port)
        {
            Listener = new TcpListener(IPAddress.Parse(address), port);
        }
        public ByteDBServerListener(string address, int port, int listeningDelay, int bufferSize)
        {
            Listener = new TcpListener(IPAddress.Parse(address), port);
            ListeningDelay = listeningDelay;
            BufferSize = bufferSize;
        }

        //
        // -------------------------------------- METHODS --------------------------------------
        //

        public static void StartListening()
        {
            Listener.Start();
        }
        public static void StopListening()
        {
            CancellationToken.Cancel();
            Listener.Stop();
        }

        public static Thread AwaitClients()
        {
            return new Thread(() => 
            {
                while (!CancellationToken.IsCancellationRequested)
                {
                    TcpClient client = Listener.AcceptTcpClient();
                    NetworkStream stream = client.GetStream();


                    Thread.Sleep(ListeningDelay);
                }
            });
        }
        public static async Task AwaitClientsAsync()
        {
            await new Task(() =>
            {
                while (!CancellationToken.IsCancellationRequested)
                {
                    Thread.Sleep(ListeningDelay);
                }
            });
        }
    }
}
