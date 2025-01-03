using DataTypesTesting.DataTypes.misc;

namespace DataTypesTesting.DataTypes.Models
{
    public abstract class DataType<TValue>
    {
        //
        // ----------------------------- PARAMETERS ----------------------------- 
        //

        public abstract TValue Value { get; set; }
        public abstract byte[] Bytes { get; set; }

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        public static byte[] GetBytes(ushort value)
        {
            if (value > Int2.MaxValue)
                throw new Int2OverflowException();

            return
            [
            (byte)(value >> 8),
            (byte)value
            ];
        }
        public static byte[] GetBytes(int value)
        {
            if (value > Int3.MaxValue)
                throw new Int3OverflowException();

            return
            [
            (byte)(value >> 16),
            (byte)(value >> 8),
            (byte)value
            ];
        }
        public static byte[] GetBytes(uint value)
        {
            if (value > Int4.MaxValue)
                throw new Int4OverflowException();

            return
            [
            (byte)(value >> 24),
            (byte)(value >> 16),
            (byte)(value >> 8),
            (byte)value
            ];
        }
        public static byte[] GetBytes(long value)
        {
            if (value > Int6.MaxValue)
                throw new Int6OverflowException();

            return
            [
            (byte)(value >> 40),
            (byte)(value >> 32),
            (byte)(value >> 24),
            (byte)(value >> 16),
            (byte)(value >> 8),
            (byte)value
            ];
        }
        public static byte[] GetBytes(ulong value)
        {
            if (value > Int8.MaxValue)
                throw new Int8OverflowException();

            return
            [
            (byte)(value >> 56),
            (byte)(value >> 48),
            (byte)(value >> 40),
            (byte)(value >> 32),
            (byte)(value >> 24),
            (byte)(value >> 16),
            (byte)(value >> 8),
            (byte)value
            ];
        }

        //public static byte[] GetBytes(string value) => ByteDBServerConfig.Encoding.GetBytes(value);
        //public static string GetString(byte[] bytes) => ByteDBServerConfig.Encoding.GetString(bytes);
    }
}
