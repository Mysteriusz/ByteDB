using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;
using ByteDBServer.Core.Server.Connection.Handshake;
using ByteDBServer.Core.Misc;

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
            return new Thread(async () => 
            {
                while (!CancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        TcpClient client = await Listener.AcceptTcpClientAsync();

                        if (ConnectedClients.Count == ByteDBServer.MaxConnections)
                            throw new ConnectionsOverflowException("Server has reached maximum allowed connection count provided in config file");

                        NetworkStream stream = client.GetStream();

                        // Start the handshake protocol with client
                        new ByteDBHandshakeV1(stream);
                    }
                    catch
                    {
                        // TODO: LOG EXCEPTION TO FILE
                    }

                    Thread.Sleep(ListeningDelay);
                }
            });
        }
    }
}
