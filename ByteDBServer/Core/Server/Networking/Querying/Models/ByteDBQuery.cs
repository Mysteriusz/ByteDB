namespace ByteDBServer.Core.Server.Networking.Querying.Models
{
    /// <summary>
    /// Basic query class representation.
    /// </summary>
    internal class ByteDBQuery
    {
        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //

        /// <summary>
        /// Query keywords, ex: INSERT INTO | IF CONTAINS | FROM VALUES.
        /// </summary>
        public string[] Keywords { get; }

        /// <summary>
        /// Query values, ex: "Workers".
        /// </summary>
        public string[] Values { get; }

        /// <summary>
        /// Query argument collections, ex: (arg1, arg2, arg3).
        /// </summary>
        public ByteDBArgumentCollection[] ArgumentCollections { get; }

        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBQuery() { }
        public ByteDBQuery(string[] keywords, string[] values, ByteDBArgumentCollection[] arguments)
        {
            Keywords = keywords;
            Values = values;
            ArgumentCollections = arguments;
        }
    }
}
