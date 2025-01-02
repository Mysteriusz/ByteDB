using ByteDBServer.Core.Misc;
using System;
using System.Diagnostics;

namespace ByteDBServer.Core.DataTypes
{
    [DebuggerDisplay("Value = {Value}, Bytes = {Bytes[0]}, {Bytes[1], Bytes[2], Bytes[3], Bytes[4], Bytes[5], Bytes[6], Bytes[7]}")]
    internal struct Int8
    {
        //
        // ----------------------------- CONSTANTS ----------------------------- 
        //

        public const ulong MaxValue = 0xffffffffffffffff;
        public const ulong MinValue = 0x0000000000000000;
        
        //
        // ----------------------------- PARAMETERS ----------------------------- 
        //
        
        public ulong Value
        {
            get { return GetInt(Bytes); }
            set { Bytes = GetBytes(value); }
        }
        public byte[] Bytes
        {
            get { return GetBytes(Value); }
            set { Value = GetInt(value); }
        }

        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public Int8 () { }
        public Int8(ulong value)
        {
            Bytes = GetBytes(value);
        }
        public Int8 (byte b1, byte b2, byte b3, byte b4, byte b5, byte b6) 
        {
            Bytes = [b1, b2, b3, b4, b5, b6];
        }
        public Int8 (byte[] array, int index = 0) 
        {
            Value = GetInt(array, index);
        }

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        public static byte[] GetBytes(ulong value)
        {
            if (value > MaxValue)
                throw new Int8OverflowException();

            byte b1 = (byte)(value >> 56);
            byte b2 = (byte)(value >> 48);
            byte b3 = (byte)(value >> 40);
            byte b4 = (byte)(value >> 32);
            byte b5 = (byte)(value >> 24);
            byte b6 = (byte)(value >> 16);
            byte b7 = (byte)(value >> 8);
            byte b8 = (byte)(value & 0xff);

            byte[] bytes = [b1, b2, b3, b4, b5, b6, b7, b8];

            return bytes;
        }
        public static ulong GetInt(byte[] bytes, int index = 0)
        {
            return BitConverter.ToUInt64(bytes, index);
        }

        //
        // ----------------------------- EXPLICIT ----------------------------- 
        //

        public static explicit operator Int8(ulong value)
        {
            return new Int8(value); 
        }
        public static explicit operator Int8(byte[] value)
        {
            return new Int8(value);
        }
        public static explicit operator byte[](Int8 value)
        {
            return value.Bytes;
        }
        public static explicit operator ulong(Int8 value)
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
                return this.Value == other.Value;

            return false;
        }
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
        public override string ToString()
        {
            return $"Int8: Value = {Value}, Bytes = {string.Join(", ", Bytes)}";
        }
    }
}
