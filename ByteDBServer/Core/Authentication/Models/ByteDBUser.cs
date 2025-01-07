﻿using ByteDBServer.Core.Server;
using System;

namespace ByteDBServer.Core.Authentication.Models
{
    internal class ByteDBUser : IDisposable
    {
        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBUser() { }
        public ByteDBUser(string username, string passwordHash, bool write = false) 
        { 
            Username = username;
            PasswordHash = passwordHash; 

            if (write)
                ByteBDAuthenticator.WriteUser(this);
        }

        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //
        public byte[] PasswordHashBytes => ByteDBServerInstance.ServerEncoding.GetBytes(PasswordHash);

        public string Username { get; private set; }
        public string PasswordHash { get; private set; }
   
        public bool Logged { get; }

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
                    ByteBDAuthenticator.RemoveUser(this);
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
