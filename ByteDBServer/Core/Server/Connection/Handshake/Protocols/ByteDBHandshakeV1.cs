using System.IO;
using System.Threading.Tasks;
using ByteDBServer.Core.Misc.Logs;
using ByteDBServer.Core.Server.Connection.Models;

namespace ByteDBServer.Core.Server.Connection.Handshake
{
    internal class ByteDBHandshakeV1 : ByteDBProtocol
    {
        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBHandshakeV1() : base(1, "HandshakeV1") { }

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        public override void StartProtocol(Stream stream, int timeout = 5)
        {
            ByteDBServerLogger.WriteToFile(StartProcotolMessage);

            var welcomePacket = new ByteDBWelcomePacketV1(ByteDBServer.ServerWelcomePacketMessage);
            welcomePacket.Write(stream);

            WaitForResponseInTime(stream, timeout);
        }
        public override async Task StartProtocolAsync(Stream stream, int timeout = 5)
        {
            ByteDBServerLogger.WriteToFile(StartProcotolMessage);

            var welcomePacket = new ByteDBWelcomePacketV1(ByteDBServer.ServerWelcomePacketMessage);
            welcomePacket.Write(stream);

            await WaitForResponseInTimeAsync(stream, timeout);
        }
    }
}
