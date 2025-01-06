using ByteDBServer.DataTypes.Models;
using System;

namespace ByteDBServer.Core.DataTypes
{
    internal class Int4 : DataType<uint>
    {
        //
        // ----------------------------- CONSTANTS ----------------------------- 
        //

        public const uint MaxValue = 0xffffffff;
        public const uint MinValue = 0x00000000;
        public const byte Length = 4;

        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //

        private uint _value = 0;
        private byte[] _bytes = new byte[4];

        public override uint Value
        {
            get { return _value; }
            set { _value = value; _bytes = GetBytes(value); }
        }
        public override byte[] Bytes
        {
            get { return _bytes; }
            set { _bytes = value; _value = GetInt(value); }
        }

        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public Int4() { }
        public Int4(uint value)
        {
            Bytes = GetBytes(value);
        }
        public Int4(byte b1, byte b2, byte b3, byte b4)
        {
            Bytes = [b1, b2, b3, b4];
        }
        public Int4(byte[] array, int index = 0)
        {
            Value = GetInt(array, index);
        }

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        public static uint GetInt(byte[] bytes, int index = 0)
        {
            if (bytes.Length < Length)
            {
                byte[] resized = new byte[Length];
                Array.Copy(bytes, 0, resized, Length - bytes.Length, bytes.Length);
                bytes = resized;
            }

            return (uint)
            (
                (bytes[index] << 24)
                | (bytes[index + 1] << 16)
                | (bytes[index + 2] << 8)
                | bytes[index + 3]
            );
        }

        //
        // ----------------------------- IMPLICIT ----------------------------- 
        //

        public static implicit operator Int4(uint value)
        {
            return new Int4(value);
        }
        public static implicit operator Int4(byte[] value)
        {
            return new Int4(value);
        }
        public static implicit operator byte[](Int4 value)
        {
            return value.Bytes;
        }
        public static implicit operator uint(Int4 value)
        {
            return value.Value;
        }

        //
        // ----------------------------- ARITHMETIC ----------------------------- 
        //

        public static Int4 operator +(Int4 i1, Int4 i2)
        {
            return new Int4(GetBytes(i1.Value + i2.Value));
        }
        public static Int4 operator -(Int4 i1, Int4 i2)
        {
            return new Int4(GetBytes(i1.Value - i2.Value));
        }
        public static Int4 operator *(Int4 i1, Int4 i2)
        {
            return new Int4(GetBytes(i1.Value * i2.Value));
        }
        public static Int4 operator /(Int4 i1, Int4 i2)
        {
            return new Int4(GetBytes(i1.Value / i2.Value));
        }
        public static Int4 operator ++(Int4 value)
        {
            return new Int4(value.Value + 1);
        }
        public static Int4 operator --(Int4 value)
        {
            return new Int4(value.Value - 1);
        }

        //
        // ----------------------------- COMPARING ----------------------------- 
        //

        public static bool operator >(Int4 i1, Int4 i2)
        {
            return i1.Value > i2.Value;
        }
        public static bool operator <(Int4 i1, Int4 i2)
        {
            return i1.Value < i2.Value;
        }
        public static bool operator >=(Int4 i1, Int4 i2)
        {
            return i1.Value >= i2.Value;
        }
        public static bool operator <=(Int4 i1, Int4 i2)
        {
            return i1.Value <= i2.Value;
        }
        public static bool operator ==(Int4 i1, Int4 i2)
        {
            return i1.Value == i2.Value;
        }
        public static bool operator !=(Int4 i1, Int4 i2)
        {
            return i1.Value != i2.Value;
        }

        //
        // ----------------------------- BITWISE ----------------------------- 
        //

        public static Int4 operator %(Int4 i1, Int4 i2)
        {
            return new Int4(i1.Value % i2.Value);
        }
        public static Int4 operator &(Int4 i1, Int4 i2)
        {
            return new Int4(i1.Value & i2.Value);
        }
        public static Int4 operator |(Int4 i1, Int4 i2)
        {
            return new Int4(i1.Value | i2.Value);
        }
        public static Int4 operator ^(Int4 i1, Int4 i2)
        {
            return new Int4(i1.Value ^ i2.Value);
        }
        public static Int4 operator ~(Int4 i1)
        {
            return new Int4(~i1.Value & MaxValue);
        }

        //
        // ----------------------------- OVERRIDES ----------------------------- 
        //

        public override bool Equals(object obj)
        {
            if (obj is Int4 other)
                return Value == other.Value;

            return false;
        }
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
        public override string ToString()
        {
            return $"Int4: Value = {_value}, Bytes = {BitConverter.ToString(_bytes)}";
        }
    }
}
