using ByteDBServer.DataTypes.Models;
using System;

namespace ByteDBServer.Core.DataTypes
{
    internal class Int6 : DataType<long>, IDisposable
    {
        //
        // ----------------------------- CONSTANTS ----------------------------- 
        //

        public const long MaxValue = 0xffffffffffff;
        public const long MinValue = 0x000000000000;
        public const byte Length = 6;

        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //

        private long _value = 0;
        private byte[] _bytes = new byte[6];

        public override long Value
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

        public Int6() { }
        public Int6(long value)
        {
            Bytes = GetBytes(value);
        }
        public Int6(byte b1, byte b2, byte b3, byte b4, byte b5, byte b6)
        {
            Bytes = [b1, b2, b3, b4, b5, b6];
        }
        public Int6(byte[] array, int index = 0)
        {
            Value = GetInt(array, index);
        }

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        public static long GetInt(byte[] bytes, int index = 0)
        {
            if (bytes.Length < Length)
            {
                byte[] resized = new byte[Length];
                Array.Copy(bytes, 0, resized, Length - bytes.Length, bytes.Length);
                bytes = resized;
            }

            return (long)
            (
                (bytes[index] << 40)
                | (bytes[index + 1] << 32)
                | (bytes[index + 2] << 24)
                | (bytes[index + 3] << 16)
                | (bytes[index + 4] << 8)
                | bytes[index + 5]
            );
        }

        //
        // ----------------------------- IMPLICIT ----------------------------- 
        //

        public static implicit operator Int6(long value)
        {
            return new Int6(value);
        }
        public static implicit operator Int6(byte[] value)
        {
            return new Int6(value);
        }
        public static implicit operator byte[](Int6 value)
        {
            return value.Bytes;
        }
        public static implicit operator long(Int6 value)
        {
            return value.Value;
        }

        //
        // ----------------------------- ARITHMETIC ----------------------------- 
        //

        public static Int6 operator +(Int6 i1, Int6 i2)
        {
            return new Int6(GetBytes(i1.Value + i2.Value));
        }
        public static Int6 operator -(Int6 i1, Int6 i2)
        {
            return new Int6(GetBytes(i1.Value - i2.Value));
        }
        public static Int6 operator *(Int6 i1, Int6 i2)
        {
            return new Int6(GetBytes(i1.Value * i2.Value));
        }
        public static Int6 operator /(Int6 i1, Int6 i2)
        {
            return new Int6(GetBytes(i1.Value / i2.Value));
        }
        public static Int6 operator ++(Int6 value)
        {
            return new Int6(value.Value + 1);
        }
        public static Int6 operator --(Int6 value)
        {
            return new Int6(value.Value - 1);
        }

        //
        // ----------------------------- COMPARING ----------------------------- 
        //

        public static bool operator >(Int6 i1, Int6 i2)
        {
            return i1.Value > i2.Value;
        }
        public static bool operator <(Int6 i1, Int6 i2)
        {
            return i1.Value < i2.Value;
        }
        public static bool operator >=(Int6 i1, Int6 i2)
        {
            return i1.Value >= i2.Value;
        }
        public static bool operator <=(Int6 i1, Int6 i2)
        {
            return i1.Value <= i2.Value;
        }
        public static bool operator ==(Int6 i1, Int6 i2)
        {
            return i1.Value == i2.Value;
        }
        public static bool operator !=(Int6 i1, Int6 i2)
        {
            return i1.Value != i2.Value;
        }

        //
        // ----------------------------- BITWISE ----------------------------- 
        //

        public static Int6 operator %(Int6 i1, Int6 i2)
        {
            return new Int6(i1.Value % i2.Value);
        }
        public static Int6 operator &(Int6 i1, Int6 i2)
        {
            return new Int6(i1.Value & i2.Value);
        }
        public static Int6 operator |(Int6 i1, Int6 i2)
        {
            return new Int6(i1.Value | i2.Value);
        }
        public static Int6 operator ^(Int6 i1, Int6 i2)
        {
            return new Int6(i1.Value ^ i2.Value);
        }
        public static Int6 operator ~(Int6 i1)
        {
            return new Int6(~i1.Value & MaxValue);
        }

        //
        // ----------------------------- OVERRIDES ----------------------------- 
        //

        public override bool Equals(object obj)
        {
            if (obj is Int6 other)
                return Value == other.Value;

            return false;
        }
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
        public override string ToString()
        {
            return $"Int6: Value = {_value}, Bytes = {BitConverter.ToString(_bytes)})";
        }

        //
        // ----------------------------- DISPOSAL ----------------------------- 
        //

        public void Dispose()
        {
            _value = 0;
            _bytes = null;

            GC.SuppressFinalize(this);
        }
    }
}
