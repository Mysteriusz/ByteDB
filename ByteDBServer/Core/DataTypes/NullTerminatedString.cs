using System.Collections.Generic;

namespace ByteDBServer.Core.DataTypes
{
    public class NullTerminatedString : DataType<string>
    {
        public const byte Null = 0x00;

        public NullTerminatedString() { }
        public NullTerminatedString(byte[] bytes, int index)
        {
            Read(bytes, index);
        }

        public void Read(byte[] bytes, int index)
        {
            List<byte> buffer = new List<byte>();

            while (bytes[index] != Null)
                buffer.Add(bytes[index++]);

            Bytes = buffer.ToArray();
        }
    }
}
