using System;

namespace ByteDBServer.Core.Server.Connection.Models
{
    internal abstract class ByteDBProtocol : IDisposable
    {
        //
        // ----------------------------- PARAMETERS ----------------------------- 
        //

        public byte Version { get; set; }

        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBProtocol(byte version)
        {
            Version = version;
        }

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        public abstract void Dispose();
    }
}
