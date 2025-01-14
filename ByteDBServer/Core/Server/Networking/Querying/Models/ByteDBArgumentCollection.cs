using System.Collections.Generic;

namespace ByteDBServer.Core.Server.Querying.Models
{
    internal class ByteDBArgumentCollection : List<string>
    {
        public ByteDBArgumentCollection() { }
        public ByteDBArgumentCollection(IEnumerable<string> values) : base(values) { }
    }
}
