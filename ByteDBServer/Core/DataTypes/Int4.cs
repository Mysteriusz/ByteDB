using System;
using System.Diagnostics;

namespace ByteDBServer.Core.DataTypes
{
    [DebuggerDisplay("Value = {Value}, Bytes = {Bytes[0]}, {Bytes[1], Bytes[2], Bytes[3]}")]
    internal class Int4 : DataType<uint>
    {
        //
        // ----------------------------- CONSTANTS ----------------------------- 
        //

        public const uint MaxValue = 0xffffffff;
        public const uint MinValue = 0x00000000;
        
        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public Int4 () { }
        public Int4(uint value)
        {
            Bytes = GetBytes(value);
        }
        public Int4 (byte b1, byte b2, byte b3, byte b4) 
        {
            Bytes = [b1, b2, b3, b4];
        }
        public Int4 (byte[] array, int index = 0) 
        {
            Value = GetInt(array, index);
        }
        
        //
        // ----------------------------- METHODS ----------------------------- 
        //

        public static uint GetInt(byte[] bytes, int index = 0)
        {
            return BitConverter.ToUInt32(bytes, index);
        }

        //
        // ----------------------------- EXPLICIT ----------------------------- 
        //

        public static explicit operator Int4(uint value)
        {
            return new Int4(value); 
        }
        public static explicit operator Int4(byte[] value)
        {
            return new Int4(value);
        }
        public static explicit operator byte[](Int4 value)
        {
            return value.Bytes;
        }
        public static explicit operator uint(Int4 value)
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
                return this.Value == other.Value;

            return false;
        }
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
        public override string ToString()
        {
            return $"Int4: Value = {Value}, Bytes = {string.Join(", ", Bytes)}";
        }
    }
}
