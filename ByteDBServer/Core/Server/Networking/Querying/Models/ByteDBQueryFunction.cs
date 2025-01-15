namespace ByteDBServer.Core.Server.Networking.Querying.Models
{
    /// <summary>
    /// Basic function class representation.
    /// </summary>
    internal class ByteDBQueryFunction
    {
        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //

        /// <summary>
        /// Function first argument.
        /// </summary>
        public string Arg1 { get; set; }

        /// <summary>
        /// Function second argument.
        /// </summary>
        public string Arg2 { get; set; }

        /// <summary>
        /// Function operator.
        /// </summary>
        public char Operator { get; set; }

        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBQueryFunction() { Arg1 = ""; Arg2 = ""; }
        public ByteDBQueryFunction(string arg1, string arg2, char opr) { Arg1 = arg1; Arg2 = arg2; Operator = opr; }

        //
        // ----------------------------- OVERRIDES ----------------------------- 
        //

        public override string ToString()
        {
            return $"{Arg1} {Operator} {Arg2}";
        }
    }
}
