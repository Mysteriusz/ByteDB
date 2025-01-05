using System.Collections.Generic;
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

        public override bool StartProtocol(Stream stream, int timeout = 5)
        {
            ByteDBServerLogger.WriteToFile(StartProcotolMessage);

            List<byte> welcomePacketStructure = new List<byte>();
            welcomePacketStructure.AddRange(ByteDBServer.ServerEncoding.GetBytes(ByteDBServer.ServerWelcomePacketMessage));
            welcomePacketStructure.AddRange(ByteDBServer.ServerEncoding.GetBytes(ByteDBServer.Version));
            welcomePacketStructure.AddRange((byte[])ByteDBServer.ServerCapabilitiesInt);

            var welcomePacket = new ByteDBWelcomePacketV1(welcomePacketStructure.ToArray());
            welcomePacket.Write(stream);

            ByteDBCustomPacket responsePacket = WaitForResponseInTime(stream, timeout);

            try
            {
                if (responsePacket.IsEmpty)
                    throw new HandshakeTimeoutException();

                ByteDBResponsePacketV1 response = responsePacket.AsPacket<ByteDBResponsePacketV1>();

                bool valid = ByteDBResponsePacketV1.Validate(response);

                if (valid)
                    ByteDBServerLogger.WriteToFile("PACKET IN ORDER");
                else
                    throw new HandshakePacketException();
            }
            catch (HandshakeTimeoutException)
            {
                var error = new ByteDBErrorPacket(ByteDBServer.ServerEncoding.GetBytes(HandshakeTimeoutException.DefaultMessage));
                error.Write(stream);

                return false;
            }
            catch (HandshakePacketException)
            {
                var error = new ByteDBErrorPacket(ByteDBServer.ServerEncoding.GetBytes(HandshakePacketException.DefaultMessage));
                error.Write(stream);

                return false;
            }

            return true;
        }
        public override async Task<bool> StartProtocolAsync(Stream stream, int timeout = 5)
        {
            ByteDBServerLogger.WriteToFile(StartProcotolMessage);

            var welcomePacket = new ByteDBWelcomePacketV1(ByteDBServer.ServerEncoding.GetBytes(ByteDBServer.ServerWelcomePacketMessage));
            welcomePacket.Write(stream);

            ByteDBCustomPacket responsePacket = await WaitForResponseInTimeAsync(stream, timeout);

            try
            {
                if (responsePacket.IsEmpty)
                    throw new HandshakeTimeoutException();

                ByteDBResponsePacketV1 response = responsePacket.AsPacket<ByteDBResponsePacketV1>();

                bool valid = ByteDBResponsePacketV1.Validate(response);

                if (valid)
                    ByteDBServerLogger.WriteToFile("PACKET IN ORDER");
                else
                    throw new HandshakePacketException();
            }
            catch (HandshakeTimeoutException)
            {
                var error = new ByteDBErrorPacket(ByteDBServer.ServerEncoding.GetBytes(HandshakeTimeoutException.DefaultMessage));
                error.Write(stream);

                return false;
            }
            catch (HandshakePacketException)
            {
                var error = new ByteDBErrorPacket(ByteDBServer.ServerEncoding.GetBytes(HandshakePacketException.DefaultMessage));
                error.Write(stream);

                return false;
            }

            return true;
        }
    }
}
