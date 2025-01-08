using ByteDBServer.Core.Server.Connection.Packets;
using ByteDBServer.Core.Server.Connection.Custom;
using ByteDBServer.Core.Server.Connection.Models;
using ByteDBServer.Core.Misc.Logs;
using ByteDBServer.Core.Misc;
using System.Threading.Tasks;
using System.IO;
using System;

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
                    ByteDBServerInstance.DefaultWelcomeMessage,
                    ByteDBServerInstance.Version,
                    ByteDBServerInstance.ServerCapabilitiesInt,
                    SaltSize,
                    Authenticator.Salt,
                    (byte)ByteDBServerInstance.ServerAuthenticationType
                );
            }
        }
        public ByteDBOkayPacket OkayPacket
        {
            get
            {
                return new ByteDBOkayPacket
                (
                    "HandshakeSuccessfull"
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
            try
            {
                // Log protocol execution
                ByteDBServerLogger.WriteToFile(StartProcotolMessage);

                // Send Welcome packet to stream synchronously
                WelcomePacket.Write(stream);

                // Wait for response synchronously
                ByteDBUnknownPacket responsePacket = WaitForResponseInTime(stream, ProtocolTimeout);

                // Check if response packet is a finalizer packet
                if (responsePacket.FIN)
                    throw new ByteDBConnectionException();
                // Check if response packet was never received packet
                else if (responsePacket.TIMEOUT)
                    throw new ByteDBTimeoutException();

                // Cast response packet to correct packet
                ByteDBResponsePacketV1 casted = ByteDBPacket.ToPacket<ByteDBResponsePacketV1>(responsePacket);

                // Check if response data is correct
                if (!Authenticator.ValidateAuthentication(casted.AuthScramble, casted.Username))
                    throw new ByteDBPacketDataException();
            }
            catch (Exception ex)
            {
                // Check the type of exception and assign packet message
                string exceptionMessage = ex switch
                {
                    ByteDBConnectionException => "ConnectionWasClosed",
                    ByteDBPacketException => "PacketOutOfOrder",
                    ByteDBTimeoutException => "HandshakeTimeout",
                    ByteDBPacketDataException => "IncorrectPacketData",
                    _ => "UnexpectedError"
                };

                // Write exception to file
                ByteDBServerLogger.WriteExceptionToFile(ex);

                // Write error packet to stream
                var errorPacket = new ByteDBErrorPacket(exceptionMessage);
                errorPacket.Write(stream);

                return false;
            }

            ByteDBServerLogger.WriteToFile("Handshake Successfull!");
            
            // Write okay packet to stream
            OkayPacket.Write(stream);
            
            return true;
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
