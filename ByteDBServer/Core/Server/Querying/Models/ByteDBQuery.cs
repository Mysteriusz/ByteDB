using System.Collections.Generic;

#nullable enable
namespace ByteDBServer.Core.Server.Networking
{
    internal class ByteDBQuery
    {
        public List<string>? Keywords { get; }
        public List<string>? Values { get; }
        public ByteDBValueCollection[]? Arguments { get; }

        public ByteDBQuery(List<string>? keywords, List<string>? values, ByteDBValueCollection[]? arguments)
        {
            Values = values;
            Keywords = keywords;
            Arguments = arguments; 
        }
    }
}
