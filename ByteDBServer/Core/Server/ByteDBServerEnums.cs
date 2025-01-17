namespace ByteDBServer.Core.Server
{
    public enum ServerCapabilities : uint
    {
        QUERY_HANDLING  = 0x00000001,
        SUPPORTS_TRANSACTIONS = 0x00000002,
        MULTI_USER_ACCESS = 0x00000004,
    }
    public enum ByteDBPacketType : byte
    {
        WELCOME_PACKET = 0x01,
        RESPONSE_PACKET = 0x02,
        OKAY_PACKET = 0x03,
        QUERY_PACKET = 0x04,
        EMPTY_PACKET = 0x05,
        ERROR_PACKET = 0xff,
    }
    public enum ServerAuthenticationType : byte
    {
        SHA_256 = 0x01,
        SHA_384 = 0x02,
        SHA_512 = 0x03,
    }
    public enum LogType
    {
        MESSAGE,
        WARNING,
        ERROR,
    }
}
