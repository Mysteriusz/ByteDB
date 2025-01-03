using ByteDBServer.DataTypes.Models;
using System.Collections.Generic;
using System.Linq;

namespace ByteDBServer.Core.DataTypes
{
    public class NullTerminatedString : DataType<string>
    {
        //
        // ----------------------------- CONSTANTS ----------------------------- 
        //

        public const byte Null = 0x00;

        //
        // ----------------------------- PARAMETERS ----------------------------- 
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
            get { return _bytes.Concat(new byte[] { 0x00 }).ToArray(); }
            set { _bytes = value.ToList(); _value = GetString(value); }
        }

        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public NullTerminatedString() { }
        public NullTerminatedString(string value)
        {
            Value = value;
        }
        public NullTerminatedString(byte[] bytes, int index = 0)
        {
            Read(bytes, index);
        }

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        public void Read(byte[] bytes, int index)
        {
            List<byte> buffer = new List<byte>();

            while (bytes[index] != Null)
                buffer.Add(bytes[index++]);

            Bytes = buffer.ToArray();
        }

        //
        // ----------------------------- IMPLICIT ----------------------------- 
        //

        public static implicit operator NullTerminatedString(string value)
        {
            return new NullTerminatedString(value);
        }
        public static implicit operator NullTerminatedString(byte[] value)
        {
            return new NullTerminatedString(value);
        }
        public static implicit operator byte[](NullTerminatedString value)
        {
            return value.Bytes;
        }
        public static implicit operator string(NullTerminatedString value)
        {
            return value.Value;
        }
    }
}
