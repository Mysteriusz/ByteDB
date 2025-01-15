using System.Collections.Generic;

namespace ByteDBServer.Core.Server.Networking.Querying.Models
{
    internal class ByteDBArgumentCollection : List<string>
    {
        public List<ByteDBQueryFunction> Functions = new List<ByteDBQueryFunction>();

        public ByteDBArgumentCollection() { }
        public ByteDBArgumentCollection(IEnumerable<string> values) : base(values) { }
    }
}
