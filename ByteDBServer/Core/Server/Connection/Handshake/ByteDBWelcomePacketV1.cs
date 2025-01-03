using ByteDBServer.Core.DataTypes;
using ByteDBServer.Core.Server.Connection.Models;
using System.IO;

namespace ByteDBServer.Core.Server.Connection.Handshake
{
    internal class ByteDBWelcomePacketV1 : ByteDBPacket
    {
        public ByteDBWelcomePacketV1() 
        {
            //
            // ----------------------------- WELCOME PACKET STRUCTURE ----------------------------- 
            //

            // Server Version
            AddRange(new NullTerminatedString(ByteDBServer.Version).Bytes);

            // 
        }

        public override void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}
