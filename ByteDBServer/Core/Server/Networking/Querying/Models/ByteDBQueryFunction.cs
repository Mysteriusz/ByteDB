using System;

namespace ByteDBServer.Core.Server.Networking.Querying.Models
{
    internal class ByteDBQueryFunction
    {
        public ByteDBQueryFunction() { Arg1 = ""; Arg2 = "" ;}
        public ByteDBQueryFunction(string arg1, string arg2, char opr) { Arg1 = arg1; Arg2 = arg2; Operator = opr; }

        public string Arg1 { get; set; }
        public string Arg2 { get; set; }
        public char Operator { get; set; }

        public override string ToString()
        {
            return $"{Arg1} {Operator} {Arg2}";
        }
    }
}
