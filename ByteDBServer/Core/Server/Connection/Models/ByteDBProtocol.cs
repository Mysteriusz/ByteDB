using ByteDBServer.Core.DataTypes;

namespace ByteDBServer.Core.Server.Connection.Models
{
    internal abstract class ByteDBProtocol
    {
        public Int1 Version { get; set; }

        public ByteDBProtocol(int version)
        {
            Version = new Int1(version);
        }
    }
}
