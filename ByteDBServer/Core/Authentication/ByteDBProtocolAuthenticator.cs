using ByteDBServer.Core.Authentication.Models;
using System.Linq;
using System;
using ByteDBServer.Core.DataTypes;

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
        public bool ValidateAuthentication(byte[] scramble, string username, bool multi_user_access, out ByteDBUser? outputUser)
        {
            outputUser = ByteDBAuthenticator.GetUser(username);

            if (outputUser == null || (!multi_user_access && outputUser.Logged))
                return false;

            byte[] userPassBytes = outputUser.PasswordHashBytes;
            byte[] expected = ByteDBAuthenticator.Hash(userPassBytes.Concat(Salt).ToArray());

            bool equals = scramble.SequenceEqual(expected);

            if (equals)
                outputUser.LoggedCount++;

            return equals;
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
