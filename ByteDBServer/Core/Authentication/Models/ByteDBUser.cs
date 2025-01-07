using ByteDBServer.Core.Server;

namespace ByteDBServer.Core.Authentication.Models
{
    internal class ByteDBUser
    {
        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBUser() { }
        public ByteDBUser(string username, string passwordHash) { Username = username; PasswordHash = passwordHash; }

        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //
        public byte[] PasswordHashBytes => ByteDBServerInstance.ServerEncoding.GetBytes(PasswordHash);

        public string Username { get; }
        public string PasswordHash { get; }
   
        public bool Logged { get; }
    }
}
