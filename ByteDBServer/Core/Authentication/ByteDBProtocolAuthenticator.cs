using ByteDBServer.Core.Authentication.Models;
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

        public ByteDBProtocolAuthenticator(int saltSize = 20) { Salt = ByteDBAuthenticator.GenerateSalt(saltSize); }

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        #nullable enable
        public bool ValidateAuthentication(byte[] scramble, string username, out ByteDBUser? outputUser)
        {
            outputUser = ByteDBAuthenticator.GetUser(username);

            if (outputUser == null)
                return false;

            byte[] userPassBytes = outputUser.PasswordHashBytes;
            byte[] expected = ByteDBAuthenticator.Hash(userPassBytes.Concat(Salt).ToArray());

            return scramble.SequenceEqual(expected);
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
                    Salt = null;
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
