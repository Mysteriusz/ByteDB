using ByteDBServer.Core.Authentication.Models;
using ByteDBServer.Core.DataTypes;
using System.Collections.Generic;
using System.Net.Sockets;
using System;

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
        /// Client`s available <see cref="ServerCapabilities"/>.
        /// </summary>
        public HashSet<ServerCapabilities> RequestedCapabilities { get; set; }

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
            }

            _disposed = true;
        }
        ~ByteDBClient()
        {
            Dispose(false);
        }
    }

}
