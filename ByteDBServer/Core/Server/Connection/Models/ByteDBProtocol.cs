using ByteDBServer.Core.DataTypes;
using System;

namespace ByteDBServer.Core.Server.Connection.Models
{
    internal abstract class ByteDBProtocol : IDisposable
    {
        //
        // ----------------------------- PARAMETERS ----------------------------- 
        //

        public Int1 Version { get; set; }

        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBProtocol(int version)
        {
            Version = new Int1(version);
        }

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        public abstract void Dispose();
    }
}
