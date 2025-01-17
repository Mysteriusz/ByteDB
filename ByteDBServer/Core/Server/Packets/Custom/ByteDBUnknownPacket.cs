﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace ByteDBServer.Core.Server.Packets.Custom
{
    /// <summary>
    /// Custom, non-templated Packet.
    /// </summary>
    internal class ByteDBUnknownPacket : IDisposable
    {
        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //

        /// <summary>
        /// Packet Header as <see cref="List{byte}"/>.
        /// </summary>
        public List<byte> Header { get; set; } = new List<byte>();

        /// <summary>
        /// Packet Payload as <see cref="List{byte}"/>.
        /// </summary>
        public List<byte> Payload { get; set; } = new List<byte>();

        /// <summary>
        /// Complete Packet as <see cref="List{byte}"/>.
        /// </summary>
        public List<byte> Packet 
        {
            get 
            {
                List<byte> _packet = new List<byte>();
                _packet.AddRange(Header);
                _packet.AddRange(Payload);

                return _packet;
            } 
        }

        /// <summary>
        /// Specifies if packet is a FINALIZER packet.
        /// </summary>
        public bool FIN { get; set; } = false;

        /// <summary>
        /// Specifies if packet is a TIMEOUT packet.
        /// </summary>
        public bool TIMEOUT { get; set; } = false;

        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBUnknownPacket() { }
        public ByteDBUnknownPacket(byte[] packet) { Header.AddRange(packet.Take(4)); Payload.AddRange(packet.Skip(4)); }
        public ByteDBUnknownPacket(byte[] header, byte[] payload) { Header.AddRange(header); Payload.AddRange(payload); }
        
        //
        // ----------------------------- DISPOSING ----------------------------- 
        //

        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                Header.Clear();
                Payload.Clear();
            }

            disposed = true;
        }
    }
}
