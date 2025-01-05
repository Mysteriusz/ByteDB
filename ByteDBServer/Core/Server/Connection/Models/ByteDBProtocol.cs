using ByteDBServer.Core.Misc.Logs;
using System;
using System.IO;
using System.Xml.Linq;

namespace ByteDBServer.Core.Server.Connection.Models
{
    internal abstract class ByteDBProtocol : IDisposable
    {
        //
        // ----------------------------- PARAMETERS ----------------------------- 
        //

        public string Name { get; }
        public byte Version { get; }

        public string CreateProcotolMessage { get { return $"CREATED PROTOCOL: {Name}, VERSION: {Version}"; } }
        public string StartProcotolMessage { get { return $"STARTING PROTOCOL: {Name}, VERSION: {Version}"; } }
        public string EndingProcotolMessage { get { return $"ENDING PROTOCOL: {Name}, VERSION: {Version}"; } }

        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBProtocol(byte version, string name)
        {
            Version = version;
            Name = name;

            ByteDBServerLogger.WriteToFile(CreateProcotolMessage);
        }

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        public abstract void StartProtocol(Stream stream, int responseTimeout = 5);

        //
        // ----------------------------- DISPOSING ----------------------------- 
        //

        private bool _disposed = false;
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {

                }

                _disposed = true;
            }
        }
        ~ByteDBProtocol()
        {
            Dispose(false);
        }
    }
}
