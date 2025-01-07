using ByteDBServer.Core.Authentication.Models;
using ByteDBServer.Core.Authentication;
using System;
using System.Linq;
using System.Security.Cryptography;
using ByteDBServer.Core.Misc.Logs;

#nullable enable
namespace ByteDBServer.Core.Authentication
{
    internal class ByteDBProtocolAuthenticator : IDisposable
    {
        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //

        public ByteDBKey AuthenticationKey { get; }

        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBProtocolAuthenticator(TimeSpan expire, int saltSize = 20) { AuthenticationKey = new ByteDBKey(expire, saltSize, true); }

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        public bool ValidateAuthentication(byte[] bytes, string username)
        {
            byte[] userPassBytes = ByteBDAuthenticator.GetUser(username).PasswordHashBytes;
            byte[] expected = ByteBDAuthenticator.Hash(userPassBytes.Concat(AuthenticationKey.Salt).ToArray());

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
                    AuthenticationKey.Dispose();
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
