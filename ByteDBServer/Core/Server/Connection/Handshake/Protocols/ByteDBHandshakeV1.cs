using System.IO;
using System.Threading.Tasks;
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

        public override bool StartProtocol(Stream stream, int timeout = 5)
        {
            // Log protocol execution
            ByteDBServerLogger.WriteToFile(StartProcotolMessage);

            // Instantiate packet
            ByteDBWelcomePacketV1 WelcomePacket = new ByteDBWelcomePacketV1(
                ByteDBServerInstance.ServerWelcomePacketMessage,
                ByteDBServerInstance.Version,
                ByteDBServerInstance.ServerCapabilitiesInt);
            
            // Write packet to stream
            WelcomePacket.Write(stream);

            // Wait for response synchronously
            ByteDBCustomPacket responsePacket = WaitForResponseInTime(stream, timeout);

            // Return if response packet is a valid ByteDBResponsePacketV1
            return ByteDBPacket.ValidatePacket<ByteDBResponsePacketV1>(stream, responsePacket);
        }
        public override async Task<bool> StartProtocolAsync(Stream stream, int timeout = 5)
        {
            // Log protocol execution
            ByteDBServerLogger.WriteToFile(StartProcotolMessage);
            
            // Instantiate packet
            ByteDBWelcomePacketV1 WelcomePacket = new ByteDBWelcomePacketV1(
                ByteDBServerInstance.ServerWelcomePacketMessage,
                ByteDBServerInstance.Version,
                ByteDBServerInstance.ServerCapabilitiesInt);

            // Write packet to stream
            WelcomePacket.Write(stream);

            // Wait for response asynchronously
            ByteDBCustomPacket responsePacket = await WaitForResponseInTimeAsync(stream, timeout);

            // Return if response packet is a valid ByteDBResponsePacketV1
            return ByteDBPacket.ValidatePacket<ByteDBResponsePacketV1>(stream, responsePacket);
        }
    }
}
