using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByteDBServer.Core.DataTypes
{
    public class ByteDBEncoding
    {
        public static readonly Dictionary<string, Encoding> EncodingType = new Dictionary<string, Encoding>()
        {
            { "UTF8", Encoding.UTF8 },
            { "ASCII", Encoding.ASCII },
        };
    }
}
