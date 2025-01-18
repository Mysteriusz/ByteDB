using ByteDBServer.Core.Authentication.Models;
using ByteDBServer.Core.DataTypes;
using System.Net.Sockets;
using System;
using System.Collections.Generic;
using ByteDBServer.Core.Server.Networking.Querying.Models;

namespace ByteDBServer.Core.Server.Networking.Models
{
    internal class ByteDBClient : IDisposable
    {
        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //

        /// <summary>
        /// Client`s socket.
        /// </summary>
        public Socket Socket { get; }

        /// <summary>
        /// Client`s stream.
        /// </summary>
        public NetworkStream Stream { get; }

        /// <summary>
        /// Client`s <see cref="ByteDBUser"/> instance.
        /// </summary>
        public ByteDBUser UserData { get; set; }

        /// <summary>
        /// Client`s available <see cref="ServerCapabilities"/> as <see cref="Int4"/>.
        /// </summary>
        public Int4 RequestedCapabilitiesInt { get; set; }

        /// <summary>
        /// Is client currently performing a transaction.
        /// </summary>
        public bool InTransaction { get; set; }

        public List<ByteDBQuery> TransactionQueries { get; } = new List<ByteDBQuery>();

        private bool _disposed = false;

        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBClient(Socket socket)
        {
            Socket = socket ?? throw new ArgumentNullException(nameof(socket));
            Stream = new NetworkStream(Socket, false);
        }

        //
        // ----------------------------- IMPLICIT ----------------------------- 
        //

        public static implicit operator Socket(ByteDBClient client) => client.Socket;
        public static implicit operator NetworkStream(ByteDBClient client) => client.Stream;

        //
        // ----------------------------- DISPOSAL ----------------------------- 
        //

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
                UserData?.Dispose();
                RequestedCapabilitiesInt?.Dispose();
            }

            _disposed = true;
        }
        ~ByteDBClient()
        {
            Dispose(false);
        }
    }

}
