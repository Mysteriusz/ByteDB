namespace ByteDBServer.Core.Server.Networking.Querying.Models
{
    internal class ByteDBQuery
    {
        public string[] Keywords { get; }
        public string[] Values { get; }
        public ByteDBArgumentCollection[] ArgumentCollections { get; }

        public ByteDBQuery() { }
        public ByteDBQuery(string[] keywords, string[] values, ByteDBArgumentCollection[] arguments)
        {
            Keywords = keywords;
            Values = values;
            ArgumentCollections = arguments;
        }
    }
}
