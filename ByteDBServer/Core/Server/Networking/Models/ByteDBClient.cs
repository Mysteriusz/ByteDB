using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace ByteDBServer.Core.Server.Networking.Models
{
    internal class ByteDBClient : IDisposable
    {
        public Socket Socket { get; }
        public NetworkStream Stream { get; }

        private bool _disposed = false;

        public ByteDBClient(Socket socket)
        {
            Socket = socket ?? throw new ArgumentNullException(nameof(socket));
            Stream = new NetworkStream(Socket, false);
        }

        public static implicit operator Socket(ByteDBClient client) => client.Socket;
        public static implicit operator NetworkStream(ByteDBClient client) => client.Stream;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                Socket?.Dispose();
                Stream?.Dispose();
            }

            _disposed = true;
        }
        ~ByteDBClient()
        {
            Dispose(false);
        }
    }

}
