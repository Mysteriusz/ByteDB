using System.Collections.Generic;

namespace ByteDBServer.Core.Server.Networking.Querying.Models
{
    internal class ByteDBValueCollection : List<string>
    {
        public ByteDBValueCollection() { }
        public ByteDBValueCollection(IEnumerable<string> values) : base(values) { }
    }
}
