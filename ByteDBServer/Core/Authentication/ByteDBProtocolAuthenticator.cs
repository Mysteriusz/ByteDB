using ByteDBServer.Core.Authentication.Models;
using System.Linq;
using System;
using ByteDBServer.Core.Server;
using ByteDBServer.Core.Misc.Logs;

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

            byte[] expected = ByteDBAuthenticator.Hash(outputUser.PasswordHash.Concat(Salt).ToArray());

            bool isAuthenticated = scramble.SequenceEqual(expected);


            if (isAuthenticated)
                outputUser.LoggedCount++;

            return isAuthenticated;
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
