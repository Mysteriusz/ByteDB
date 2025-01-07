using ByteDBServer.Core.Authentication.Models;
using System.Collections.Generic;

namespace ByteDBServer.Core.Authentication
{
    internal static class ByteBDServerAuthenticator
    {
        public static List<ByteDBKey> ActiveKeys { get; set; }
    }
}
