using ByteDBServer.Core.Config;
using ByteDBServer.Core.Misc;

namespace ByteDBServer.Core.DataTypes
{
    public abstract class DataType<TValue>
    {
        //
        // ----------------------------- PARAMETERS ----------------------------- 
        //

        public TValue Value { get; set; }
        public byte[] Bytes { get; set; }

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        public static byte[] GetBytes(ushort value)
        {
            if (value > 0xffff)
                throw new Int2OverflowException();

            byte b1 = (byte)(value >> 8);
            byte b2 = (byte)(value & 0xff);

            return [b1, b2];
        }
        public static byte[] GetBytes(int value)
        {
            if (value > 0xffffff)
                throw new Int3OverflowException();

            byte b1 = (byte)(value >> 16);
            byte b2 = (byte)(value >> 8);
            byte b3 = (byte)(value & 0xff);
            
            return [b1, b2, b3];
        }
        public static byte[] GetBytes(uint value)
        {
            if (value > 0xffffffff)
                throw new Int4OverflowException();

            byte b1 = (byte)(value >> 24);
            byte b2 = (byte)(value >> 16);
            byte b3 = (byte)(value >> 8);
            byte b4 = (byte)(value & 0xff);

            return [b1, b2, b3, b4];
        }
        public static byte[] GetBytes(long value)
        {
            if (value > 0xffffffffffff)
                throw new Int6OverflowException();

            byte b1 = (byte)(value >> 40);
            byte b2 = (byte)(value >> 32);
            byte b3 = (byte)(value >> 24);
            byte b4 = (byte)(value >> 16);
            byte b5 = (byte)(value >> 8);
            byte b6 = (byte)(value & 0xff);

            return [b1, b2, b3, b4, b5, b6];
        }
        public static byte[] GetBytes(ulong value)
        {
            if (value > 0xffffffffffffffff)
                throw new Int8OverflowException();

            byte b1 = (byte)(value >> 56);
            byte b2 = (byte)(value >> 48);
            byte b3 = (byte)(value >> 40);
            byte b4 = (byte)(value >> 32);
            byte b5 = (byte)(value >> 24);
            byte b6 = (byte)(value >> 16);
            byte b7 = (byte)(value >> 8);
            byte b8 = (byte)(value & 0xff);

            return [b1, b2, b3, b4, b5, b6, b7, b8];
        }
        public static byte[] GetBytes(string value) => ByteDBServerConfig.Encoding.GetBytes(value);
        public static string GetString(byte[] bytes) => ByteDBServerConfig.Encoding.GetString(bytes);
    }
}
