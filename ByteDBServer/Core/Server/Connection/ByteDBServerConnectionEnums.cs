namespace ByteDBServer.Core.Server.Connection
{
    public enum ByteDBPacketType : byte
    {
        WelcomePacket = 0x01,
        ErrorPacket = 0x02,
    }
}
