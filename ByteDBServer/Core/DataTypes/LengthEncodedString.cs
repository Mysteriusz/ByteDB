using ByteDBServer.DataTypes.Models;
using System.Collections.Generic;
using System.Linq;

namespace ByteDBServer.Core.DataTypes
{
    internal class LengthEncodedString : DataType<string>
    {
        //
        // ----------------------------- CONSTANTS ----------------------------- 
        //

        public Int2 Length => new Int2((ushort)_bytes.Count);

        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //

        private string _value = "";
        private List<byte> _bytes = new List<byte>();

        public override string Value 
        {
            get { return _value; }
            set { _value = value; _bytes = GetBytes(value).ToList(); }
        }
        public override byte[] Bytes 
        {
            get { return new byte[] { Length.Bytes[0], Length.Bytes[1] }.Concat(_bytes).ToArray(); }
            set { _bytes = value.ToList(); _value = GetString(value); }
        }

        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public LengthEncodedString() { }
        public LengthEncodedString(string value)
        {
            Value = value;
        }
        public LengthEncodedString(byte[] bytes, int length, int index = 0)
        {
            Read(bytes, index, length);
        }
        public LengthEncodedString(byte[] bytes, int index, int length, out int resultIndex)
        {
            Read(bytes, index, length, out resultIndex);
        }

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        public void Read(byte[] bytes, int index, int length)
        {
            List<byte> buffer = new List<byte>();

            while (index < length)
                buffer.Add(bytes[index++]);

            Bytes = buffer.ToArray();
        }
        public void Read(byte[] bytes, int index, int length, out int resultIndex)
        {
            List<byte> buffer = new List<byte>();

            while (index < length)
                buffer.Add(bytes[index++]);

            resultIndex = index;
            Bytes = buffer.ToArray();
        }

        //
        // ----------------------------- IMPLICIT ----------------------------- 
        //

        public static implicit operator LengthEncodedString(string value)
        {
            return new LengthEncodedString(value);
        }
        public static implicit operator LengthEncodedString(byte[] value)
        {
            return new LengthEncodedString(value, value.Length);
        }
        public static implicit operator byte[](LengthEncodedString value)
        {
            return value.Bytes;
        }
        public static implicit operator string(LengthEncodedString value)
        {
            return value.Value;
        }
    }
}
