using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;
using ByteDBServer.Core.Server.Connection.Handshake;
using ByteDBServer.Core.Misc;
using System.Runtime.InteropServices;
using ByteDBServer.Core.Misc.Logs;
using System;

namespace ByteDBServer.Core.Server
{
    internal class ByteDBServerListener
    {
        //
        // ----------------------------------- PROPERTIES -----------------------------------
        //

        public TcpListener Listener { get; private set; }
        public List<TcpClient> ConnectedClients { get; private set; }

        public CancellationTokenSource CancellationToken { get; } = new CancellationTokenSource();
        
        //
        // ----------------------------------- CONSTRUCTORS -----------------------------------
        //

        public ByteDBServerListener(string address, int port)
        {
            Listener = new TcpListener(IPAddress.Parse(address), port);
        }

        //
        // -------------------------------------- METHODS --------------------------------------
        //

        public void StartListening()
        {
            Listener.Start();
        }
        public void StopListening()
        {
            CancellationToken.Cancel();
            Listener.Stop();
        }

        public Thread AwaitClients()
        {
            return new Thread(async () => 
            {
                while (!CancellationToken.IsCancellationRequested)
                {
                    bool success = false;

                    TcpClient client = await Listener.AcceptTcpClientAsync();

                    try
                    {
                        if (ConnectedClients.Count == ByteDBServer.MaxConnections)
                            throw new ConnectionsOverflowException("Server has reached maximum allowed connection count provided in config file");

                        NetworkStream stream = client.GetStream();

                        // Start the handshake protocol with client
                        new ByteDBHandshakeV1(stream);

                        success = true;
                    }
                    catch (Exception ex)
                    {
                        ByteDBServerLogger.WriteExceptionToFile(ex);
                    }

                    if (success)
                        ConnectedClients.Add(client);

                    Thread.Sleep(ByteDBServer.ListeningDelay);
                }
            });
        }
    }
}
