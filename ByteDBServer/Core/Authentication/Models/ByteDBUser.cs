namespace ByteDBServer.Core.Authentication.Models
{
    internal class ByteDBUser
    {
        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBUser() { }
        public ByteDBUser(string username, string passwordHash) { Username = username; PassowrdHash = passwordHash; }

        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //

        public string PassowrdHash { get; }
        public string Username { get; }
   
        public bool Logged { get; }
    }
}
