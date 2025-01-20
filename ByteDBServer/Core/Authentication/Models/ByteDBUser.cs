using ByteDBServer.Core.Server;
using System;

namespace ByteDBServer.Core.Authentication.Models
{
    internal class ByteDBUser : IDisposable
    {
        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBUser() { }
        public ByteDBUser(string username, string passwordHashBase64, bool write = false)
        {
            Username = username;
            PasswordHashBase64 = passwordHashBase64;

            if (write)
                ByteDBAuthenticator.WriteUser(this);
        }
        public ByteDBUser(string username, byte[] passwordHash, bool write = false) 
        { 
            Username = username;
            PasswordHash = passwordHash; 

            if (write)
                ByteDBAuthenticator.WriteUser(this);
        }

        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //
        public string PasswordHashBase64
        {
            get => Convert.ToBase64String(PasswordHash);
            set => PasswordHash = Convert.FromBase64String(value);
        }

        public string Username { get; private set; }
        public byte[] PasswordHash { get; private set; }

        public int LoggedCount { get; set; }
        public bool Logged => LoggedCount > 0;

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
                    Username = null;
                    PasswordHash = null;
                }

                _disposed = true;
            }
        }
        ~ByteDBUser()
        {
            Dispose(false);
        }
    }
}
