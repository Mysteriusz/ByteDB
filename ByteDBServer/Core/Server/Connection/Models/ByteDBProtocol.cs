using DataTypesTesting.DataTypes;
using System;

namespace ByteDBServer.Core.Server.Connection.Models
{
    internal abstract class ByteDBProtocol : IDisposable
    {
        //
        // ----------------------------- PARAMETERS ----------------------------- 
        //

        public Int2 Version { get; set; }

        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBProtocol(int version)
        {
            Version = new Int2((ushort)version);
        }

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        public abstract void Dispose();
    }
}
