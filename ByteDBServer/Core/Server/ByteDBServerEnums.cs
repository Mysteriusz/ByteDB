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
        EmptyPacket = 0x05,
        ErrorPacket = 0xff,
    }
    public enum ServerAuthenticationType : byte
    {
        SHA224 = 0x01,
        SHA256 = 0x02,
        SHA384 = 0x03,
        SHA512 = 0x04,
        SHA512_224 = 0x05,
        SHA512_256 = 0x06,
    }
}
