using System.Linq;
using System;

namespace ByteDBServer.Core.Authentication
{
    internal class ByteDBProtocolAuthenticator : IDisposable
    {
        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //

        public byte[] Salt { get; private set; }

        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBProtocolAuthenticator(int saltSize = 20) { Salt = ByteBDAuthenticator.GenerateSalt(saltSize); }

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        public bool ValidateAuthentication(byte[] bytes, string username)
        {
            byte[] userPassBytes = ByteBDAuthenticator.GetUser(username).PasswordHashBytes;
            byte[] expected = ByteBDAuthenticator.Hash(userPassBytes.Concat(Salt).ToArray());

            return bytes.SequenceEqual(expected);
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
                    Salt = [];
                }

                _disposed = true;
            }
        }
        ~ByteDBProtocolAuthenticator()
        {
            Dispose(false);
        }
    }
}
