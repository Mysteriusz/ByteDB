using ByteDBServer.Core.Server.Protocols.Models;
using ByteDBServer.Core.Server.Packets.Models;
using ByteDBServer.Core.Server.Packets.Custom;
using ByteDBServer.Core.Server.Packets;
using ByteDBServer.Core.Misc.Logs;
using ByteDBServer.Core.Misc;
using System.Threading.Tasks;
using System.IO;
using System;
using ByteDBServer.Core.Server.Networking.Models;
using ByteDBServer.Core.Authentication;
using ByteDBServer.Core.Authentication.Models;
using System.IO.Compression;

namespace ByteDBServer.Core.Server.Protocols
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

        public override bool ExecuteProtocol(ByteDBClient client)
        {
            try
            {
                //ByteDBServerLogger.WriteToFile("Sending Welcome Packet...");

                // Send Welcome packet on stream asynchronously
                WelcomePacket.Write(client.Stream);

                //ByteDBServerLogger.WriteToFile("Welcome Packet sent, waiting for response...");

                // Wait for response asynchronously
                using (ByteDBUnknownPacket responsePacket = WaitForResponseInTime(client.Stream, ProtocolTimeout))
                {
                    // Check if response packet is a finalizer packet
                    if (responsePacket.FIN)
                        throw new ByteDBConnectionException();

                    // Check if response packet was never received packet
                    else if (responsePacket.TIMEOUT)
                        throw new ByteDBTimeoutException();

                    // Cast response packet to correct packet
                    using (ByteDBResponsePacketV1 casted = ByteDBPacket.ToPacket<ByteDBResponsePacketV1>(responsePacket))
                    {
                        // Check if response data is correct
                        if (!Authenticator.ValidateAuthentication(casted.AuthScramble, casted.Username, out ByteDBUser requestedUser))
                            throw new ByteDBPacketDataException();

                        // Assign user to client
                        client.UserData = requestedUser;

                        // Read requested flags from packet
                        client.RequestedCapabilities = ByteDBAuthenticator.ReadFlags<ServerCapabilities>(casted.Capabilities);

                        // Read requested flags int from packet
                        client.RequestedCapabilitiesInt = casted.Capabilities;
                    }
                }
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
                using (var errorPacket = new ByteDBErrorPacket(exceptionMessage))
                    errorPacket.Write(client.Stream);

                return false;
            }

            // Write okay packet to stream
            OkayPacket.Write(client.Stream);

            return true;
        }
        public override async Task<bool> ExecuteProtocolAsync(ByteDBClient client)
        {
            try
            {
                //ByteDBServerLogger.WriteToFile("Sending Welcome Packet...");

                // Send Welcome packet on stream asynchronously
                await WelcomePacket.WriteAsync(client.Stream);

                //ByteDBServerLogger.WriteToFile("Welcome Packet sent, waiting for response...");

                // Wait for response asynchronously
                using (ByteDBUnknownPacket responsePacket = await WaitForResponseInTimeAsync(client.Stream, ProtocolTimeout))
                {
                    // Check if response packet is a finalizer packet
                    if (responsePacket.FIN)
                        throw new ByteDBConnectionException();

                    // Check if response packet was never received packet
                    else if (responsePacket.TIMEOUT)
                        throw new ByteDBTimeoutException();

                    // Cast response packet to correct packet
                    using (ByteDBResponsePacketV1 casted = ByteDBPacket.ToPacket<ByteDBResponsePacketV1>(responsePacket))
                    {
                        // Check if response data is correct
                        if (!Authenticator.ValidateAuthentication(casted.AuthScramble, casted.Username, out ByteDBUser requestedUser))
                            throw new ByteDBPacketDataException();

                        // Assign user to client
                        client.UserData = requestedUser;

                        // Read requested flags from packet
                        client.RequestedCapabilities = ByteDBAuthenticator.ReadFlags<ServerCapabilities>(casted.Capabilities);

                        // Read requested flags int from packet
                        client.RequestedCapabilitiesInt = casted.Capabilities;
                    }
                }
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
                using (var errorPacket = new ByteDBErrorPacket(exceptionMessage))
                    await errorPacket.WriteAsync(client.Stream);

                return false;
            }

            // Write okay packet to stream
            await OkayPacket.WriteAsync(client.Stream);

            return true;
        }
    }
}
