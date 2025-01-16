using System.Collections.Generic;

namespace ByteDBServer.Core.Server.Networking.Querying.Models
{
    /// <summary>
    /// Basic argument collection class representation.
    /// </summary>
    internal class ByteDBArgumentCollection : List<string>
    {
        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //

        /// <summary>
        /// Collection functions.
        /// </summary>
        public List<ByteDBQueryFunction> Functions = new List<ByteDBQueryFunction>();

        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBArgumentCollection() { }
        public ByteDBArgumentCollection(IEnumerable<string> values) : base(values) { }

        //
        // ----------------------------- OVERRIDES ----------------------------- 
        //

        public override string ToString()
        {
            return string.Join(", ", this);
        }
    }
}
