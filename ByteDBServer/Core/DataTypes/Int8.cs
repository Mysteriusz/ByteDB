using ByteDBServer.DataTypes.Models;
using System;

namespace ByteDBServer.Core.DataTypes
{
    internal class Int8 : DataType<ulong>, IDisposable
    {
        //
        // ----------------------------- CONSTANTS ----------------------------- 
        //

        public const ulong MaxValue = 0xffffffffffffffff;
        public const ulong MinValue = 0x0000000000000000;
        public const byte Length = 8;

        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //

        private ulong _value = 0;
        private byte[] _bytes = new byte[8];

        public override ulong Value
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

        public Int8() { }
        public Int8(ulong value)
        {
            Value = value;
        }
        public Int8(byte b1, byte b2, byte b3, byte b4, byte b5, byte b6, byte b7, byte b8)
        {
            Bytes = [b1, b2, b3, b4, b5, b6, b7, b8];
        }
        public Int8(byte[] array, int index = 0)
        {
            Value = GetInt(array, index);
        }

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        public static ulong GetInt(byte[] bytes, int index = 0)
        {
            if (bytes.Length < Length)
            {
                byte[] resized = new byte[Length];
                Array.Copy(bytes, 0, resized, Length - bytes.Length, bytes.Length);
                bytes = resized;
            }

            return (ulong)
            (
                (bytes[index] << 56)
                | (bytes[index + 1] << 48)
                | (bytes[index + 2] << 40)
                | (bytes[index + 3] << 32)
                | (bytes[index + 4] << 24)
                | (bytes[index + 5] << 16)
                | (bytes[index + 6] << 8)
                | bytes[index + 7]
            );
        }

        //
        // ----------------------------- IMPLICIT ----------------------------- 
        //

        public static implicit operator Int8(ulong value)
        {
            return new Int8(value);
        }
        public static implicit operator Int8(byte[] value)
        {
            return new Int8(value);
        }
        public static implicit operator byte[](Int8 value)
        {
            return value.Bytes;
        }
        public static implicit operator ulong(Int8 value)
        {
            return value.Value;
        }

        //
        // ----------------------------- ARITHMETIC ----------------------------- 
        //

        public static Int8 operator +(Int8 i1, Int8 i2)
        {
            return new Int8(GetBytes(i1.Value + i2.Value));
        }
        public static Int8 operator -(Int8 i1, Int8 i2)
        {
            return new Int8(GetBytes(i1.Value - i2.Value));
        }
        public static Int8 operator *(Int8 i1, Int8 i2)
        {
            return new Int8(GetBytes(i1.Value * i2.Value));
        }
        public static Int8 operator /(Int8 i1, Int8 i2)
        {
            return new Int8(GetBytes(i1.Value / i2.Value));
        }
        public static Int8 operator ++(Int8 value)
        {
            return new Int8(value.Value + 1);
        }
        public static Int8 operator --(Int8 value)
        {
            return new Int8(value.Value - 1);
        }

        //
        // ----------------------------- COMPARING ----------------------------- 
        //

        public static bool operator >(Int8 i1, Int8 i2)
        {
            return i1.Value > i2.Value;
        }
        public static bool operator <(Int8 i1, Int8 i2)
        {
            return i1.Value < i2.Value;
        }
        public static bool operator >=(Int8 i1, Int8 i2)
        {
            return i1.Value >= i2.Value;
        }
        public static bool operator <=(Int8 i1, Int8 i2)
        {
            return i1.Value <= i2.Value;
        }
        public static bool operator ==(Int8 i1, Int8 i2)
        {
            return i1.Value == i2.Value;
        }
        public static bool operator !=(Int8 i1, Int8 i2)
        {
            return i1.Value != i2.Value;
        }

        //
        // ----------------------------- BITWISE ----------------------------- 
        //

        public static Int8 operator %(Int8 i1, Int8 i2)
        {
            return new Int8(i1.Value % i2.Value);
        }
        public static Int8 operator &(Int8 i1, Int8 i2)
        {
            return new Int8(i1.Value & i2.Value);
        }
        public static Int8 operator |(Int8 i1, Int8 i2)
        {
            return new Int8(i1.Value | i2.Value);
        }
        public static Int8 operator ^(Int8 i1, Int8 i2)
        {
            return new Int8(i1.Value ^ i2.Value);
        }
        public static Int8 operator ~(Int8 i1)
        {
            return new Int8(~i1.Value & MaxValue);
        }

        //
        // ----------------------------- OVERRIDES ----------------------------- 
        //

        public override bool Equals(object obj)
        {
            if (obj is Int8 other)
                return Value == other.Value;

            return false;
        }
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
        public override string ToString()
        {
            return $"Int8: Value = {_value}, Bytes = {BitConverter.ToString(_bytes)}";
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
