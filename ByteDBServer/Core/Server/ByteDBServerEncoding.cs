using System.Collections.Generic;
using System.Text;

namespace ByteDBServer.Core.Server
{
    public class ByteDBServerEncoding
    {
        public static readonly Dictionary<string, Encoding> EncodingType = new Dictionary<string, Encoding>()
        {
            { "UTF8", Encoding.UTF8 },
            { "ASCII", Encoding.ASCII },
        };
    }
}
