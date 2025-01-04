namespace ByteDBServer.Core.Server.Connection
{
    public enum ByteDBPacketType : byte
    {
        WelcomePacket = 0x01,
        ResponsePacket = 0x02,
        OkayPacket = 0x03,
        ErrorPacket = 0x04,
    }
}
