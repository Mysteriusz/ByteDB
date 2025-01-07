using System.IO;
using System.Threading.Tasks;
using ByteDBServer.Core.Misc.Logs;
using ByteDBServer.Core.Server.Connection.Handshake.Custom;
using ByteDBServer.Core.Server.Connection.Models;

namespace ByteDBServer.Core.Server.Connection.Handshake
{
    internal class ByteDBHandshakeV1 : ByteDBProtocol
    {
        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //

        public ByteDBWelcomePacketV1 WelcomePacket
        {
            get
            {
                return new ByteDBWelcomePacketV1
                (
                    ByteDBServerInstance.ServerWelcomePacketMessage,
                    ByteDBServerInstance.Version,
                    ByteDBServerInstance.ServerCapabilitiesInt,
                    SaltSize,
                    Authenticator.Salt,
                    (byte)ByteDBServerInstance.ServerAuthenticationType
                );
            }
        }

        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBHandshakeV1() : base(1, "HandshakeV1", 5, 20) { }

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        public override bool ExecuteProtocol(Stream stream)
        {
            // Log protocol execution
            ByteDBServerLogger.WriteToFile(StartProcotolMessage);

            // Send Welcome packet on stream synchronously
            WelcomePacket.Write(stream);

            // Wait for response synchronously
            ByteDBUnknownPacket responsePacket = WaitForResponseInTime(stream, ProtocolTimeout);

            return false;
        }
        public override async Task<bool> ExecuteProtocolAsync(Stream stream)
        {
            // Log protocol execution
            ByteDBServerLogger.WriteToFile(StartProcotolMessage);

            // Send Welcome packet on stream asynchronously
            await WelcomePacket.WriteAsync(stream);

            // Wait for response asynchronously
            ByteDBUnknownPacket responsePacket = await WaitForResponseInTimeAsync(stream, ProtocolTimeout);

            return false;
        }
    }
}
