using System;
using System.Threading.Tasks;
using System.Security.Cryptography;
using ByteDBServer.Core.Misc.Logs;

namespace ByteDBServer.Core.Authentication.Models
{
    internal class ByteDBKey : IDisposable
    {
        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBKey(TimeSpan time, int saltSize, bool write = false)
        {
            ByteDBServerLogger.WriteToFile("AUTHENTICATION KEY CREATED, " + "EXPIRING IN: " + time);

            Salt = GenerateSalt(saltSize);
            DisposalTask = CreateDisposalTask(time);

            if (write)
                ByteBDAuthenticator.WriteKey(this);
        }
        public ByteDBKey(TimeSpan time, int saltSize, byte[] salt, bool write = false)
        {
            ByteDBServerLogger.WriteToFile("AUTHENTICATION KEY CREATED, " + "EXPIRING IN: " + time);

            Salt = salt;
            DisposalTask = CreateDisposalTask(time);

            if (write)
                ByteBDAuthenticator.WriteKey(this);
        }

        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //

        public byte[] Salt { get; }
        public Task DisposalTask { get; }
        public DateTime ExpireDate { get; }

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        private Task CreateDisposalTask(TimeSpan time)
        {
            return Task.Delay(time).ContinueWith(_ => Dispose());
        }
        private byte[] GenerateSalt(int size)
        {
            byte[] salt = new byte[size];

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                rng.GetBytes(salt);

            return salt;
        }

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
                    ByteBDAuthenticator.RemoveKey(this);
                    Array.Clear(Salt, 0, Salt.Length);
                    DisposalTask.Dispose();
                }

                _disposed = true;
            }
        }
        ~ByteDBKey()
        {
            Dispose(false);
        }
    }
}
