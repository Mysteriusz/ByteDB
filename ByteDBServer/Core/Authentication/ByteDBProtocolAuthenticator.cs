using ByteDBServer.Core.Authentication.Models;
using System;
using System.Security.Cryptography;

#nullable enable
namespace ByteDBServer.Core.Authentication
{
    internal class ByteDBProtocolAuthenticator : IDisposable
    {
        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //

        public byte[]? Salt { get; }
        public ByteDBKey? AuthenticationKey { get; }

        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBProtocolAuthenticator() { }
        public ByteDBProtocolAuthenticator(TimeSpan expire, int saltSize = 64)
        { Salt = GenerateSalt(saltSize); AuthenticationKey = GenerateAuthenticationKey(expire); }

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        public ByteDBKey GenerateAuthenticationKey(TimeSpan expire)
        {
            return new ByteDBKey(Salt, expire);
        }
        public byte[] GenerateSalt(int size)
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
                    if (Salt != null)
                        Array.Clear(Salt, 0, Salt.Length);

                    AuthenticationKey?.Dispose();
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
