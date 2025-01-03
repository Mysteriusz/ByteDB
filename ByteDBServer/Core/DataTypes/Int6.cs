using System;
using System.Diagnostics;

namespace ByteDBServer.Core.DataTypes
{
    [DebuggerDisplay("Value = {Value}, Bytes = {Bytes[0]}, {Bytes[1], Bytes[2], Bytes[3], Bytes[4], Bytes[5]}")]
    internal class Int6 : DataType<long>
    {
        //
        // ----------------------------- CONSTANTS ----------------------------- 
        //

        public const long MaxValue = 0xffffffffffff;
        public const long MinValue = 0x000000000000;

        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public Int6 () { }
        public Int6(long value)
        {
            Bytes = GetBytes(value);
        }
        public Int6 (byte b1, byte b2, byte b3, byte b4, byte b5, byte b6) 
        {
            Bytes = [b1, b2, b3, b4, b5, b6];
        }
        public Int6 (byte[] array, int index = 0) 
        {
            Value = GetInt(array, index);
        }

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        public static long GetInt(byte[] bytes, int index = 0)
        {
            return BitConverter.ToInt64(bytes, index);
        }

        //
        // ----------------------------- EXPLICIT ----------------------------- 
        //

        public static explicit operator Int6(long value)
        {
            return new Int6(value); 
        }
        public static explicit operator Int6(byte[] value)
        {
            return new Int6(value);
        }
        public static explicit operator byte[](Int6 value)
        {
            return value.Bytes;
        }
        public static explicit operator long(Int6 value)
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
                return this.Value == other.Value;

            return false;
        }
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
        public override string ToString()
        {
            return $"Int6: Value = {Value}, Bytes = {string.Join(", ", Bytes)}";
        }
    }
}
