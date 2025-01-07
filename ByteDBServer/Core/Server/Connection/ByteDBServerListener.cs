using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;
using ByteDBServer.Core.Server.Connection.Handshake;
using ByteDBServer.Core.Misc;
using System;
using ByteDBServer.Core.Misc.Logs;
using ByteDBServer.Core.Server.Connection.Handshake.Packets;

namespace ByteDBServer.Core.Server
{
    internal class ByteDBServerListener
    {
        //
        // ----------------------------------- PROPERTIES -----------------------------------
        //

        public TcpListener Listener { get; }
        public Thread ListeningThread { get; }

        public List<TcpClient> ConnectedClients { get; } = new List<TcpClient>();
        public CancellationTokenSource CancellationToken { get; } = new CancellationTokenSource();

        //
        // ----------------------------------- CONSTRUCTORS -----------------------------------
        //

        public ByteDBServerListener(string address, int port)
        {
            ByteDBServerLogger.WriteToFile(address);
            Listener = new TcpListener(IPAddress.Parse(address), port);
            ListeningThread = AwaitClients();
        }

        //
        // -------------------------------------- METHODS --------------------------------------
        //

        public void StartListening()
        {
            Listener.Start();
            ListeningThread.Start();
        }
        public void StopListening()
        {
            CancellationToken.Cancel();
            Listener.Stop();
            ListeningThread.Abort();
        }

        public Thread AwaitClients()
        {
            return new Thread(async () =>
            {
                try
                {
                    while (!CancellationToken.IsCancellationRequested)
                    {
                        TcpClient client = await Listener.AcceptTcpClientAsync();

                        if (ConnectedClients.Count == ByteDBServerInstance.MaxConnections)
                            throw new ConnectionsOverflowException("Server has reached maximum allowed connection count provided in config file");

                        if (ConnectedClients.Contains(client))
                            throw new InternalConnectionException("Connected client tried to connect again");

                        if (Listener == null) throw new InternalConnectionException("Listener is null");
                        if (CancellationToken == null) throw new InternalConnectionException("CancellationToken is null");
                        if (ConnectedClients == null) throw new InternalConnectionException("ConnectedClients is null");

                        NetworkStream stream = client.GetStream();

                        // Start the handshake protocol with client
                        var handshake = new ByteDBHandshakeV1();
                        bool success = handshake.ExecuteProtocol(stream);

                        // Check if protocol was a success
                        if (success)
                        {
                            // Add client to connected
                            ConnectedClients.Add(client);

                            // Send okay packet to clinet
                            new ByteDBOkayPacket("Connection successfull!");
                        }
                        else
                            client.Dispose();

                        Thread.Sleep(ByteDBServerInstance.ListeningDelay);
                    }
                }
                catch (Exception ex)
                {
                    ByteDBServerLogger.WriteExceptionToFile(ex);
                }
            });
        }
    }
}
