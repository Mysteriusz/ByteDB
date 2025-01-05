namespace ByteDBServer.Core.Server
{
    public enum ServerCapabilities : uint
    {
        SERVER_HANDLE_QUERIES = 0x00000001,
    }

    public enum ByteDBPacketType : byte
    {
        WelcomePacket = 0x01,
        ResponsePacket = 0x02,
        OkayPacket = 0x03,
        ErrorPacket = 0x04,
        EmptyPacket = 0x05,
    }
}
