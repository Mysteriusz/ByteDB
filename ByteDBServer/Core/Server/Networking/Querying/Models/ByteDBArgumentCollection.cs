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
        /// Previous word string used for identifying argument call keyword.
        /// </summary>
        public string PreviousWord { get; set; }

        /// <summary>
        /// Collection functions.
        /// </summary>
        public List<ByteDBQueryFunction> Functions = new List<ByteDBQueryFunction>();

        public List<ByteDBArgumentCollection> SubArguments = new List<ByteDBArgumentCollection>();

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
