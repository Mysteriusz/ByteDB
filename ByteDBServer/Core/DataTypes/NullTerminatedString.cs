using System;
using System.Collections.Generic;

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

        public override string Value 
        {
            get => base.Value;
            set => base.Value = value;
        }
        public override byte[] Bytes
        {
            get
            {
                byte[] baseBytes = base.Bytes;
                byte[] result = new byte[baseBytes.Length + 1];
                Array.Copy(baseBytes, result, baseBytes.Length);
                result[baseBytes.Length] = Null;
                return result;
            }
            set => base.Bytes = value;
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
        // ----------------------------- EXPLICIT ----------------------------- 
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
    }
}
