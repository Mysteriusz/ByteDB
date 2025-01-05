using System.IO;
using System.Threading.Tasks;
using ByteDBServer.Core.Misc;
using ByteDBServer.Core.Misc.Logs;
using ByteDBServer.Core.Server.Connection.Handshake.Packets;
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

            var welcomePacket = new ByteDBWelcomePacketV1(ByteDBServer.ServerEncoding.GetBytes(ByteDBServer.ServerWelcomePacketMessage));
            welcomePacket.Write(stream);

            ByteDBCustomPacket responsePacket = WaitForResponseInTime(stream, timeout);
            ByteDBResponsePacketV1 response = responsePacket.AsPacket<ByteDBResponsePacketV1>();

            bool valid = ByteDBResponsePacketV1.Validate(response);

            if (valid)
                ByteDBServerLogger.WriteToFile("PACKET IN ORDER");
            else
                throw new HandshakePacketException();
        }
        public override async Task StartProtocolAsync(Stream stream, int timeout = 5)
        {
            ByteDBServerLogger.WriteToFile(StartProcotolMessage);

            var welcomePacket = new ByteDBWelcomePacketV1(ByteDBServer.ServerEncoding.GetBytes(ByteDBServer.ServerWelcomePacketMessage));
            welcomePacket.Write(stream);

            ByteDBCustomPacket responsePacket = await WaitForResponseInTimeAsync(stream, timeout);
            ByteDBResponsePacketV1 response = responsePacket.AsPacket<ByteDBResponsePacketV1>();

            bool valid = ByteDBResponsePacketV1.Validate(response);

            if (valid)
                ByteDBServerLogger.WriteToFile("PACKET IN ORDER");
            else
                throw new HandshakePacketException();
        }
    }
}
