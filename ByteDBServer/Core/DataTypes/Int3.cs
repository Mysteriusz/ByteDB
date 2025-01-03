using System;
using System.Diagnostics;

namespace ByteDBServer.Core.DataTypes
{
    [DebuggerDisplay("Value = {Value}, Bytes = {Bytes[0]}, {Bytes[1], Bytes[2]}")]
    internal class Int3 : DataType<int>
    {
        //
        // ----------------------------- CONSTANTS ----------------------------- 
        //

        public const int MaxValue = 0xffffff;
        public const int MinValue = 0x000000;

        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public Int3 () { }
        public Int3(int value)
        {
            Bytes = GetBytes(value);
        }
        public Int3 (byte b1, byte b2, byte b3) 
        {
            Bytes = [b1, b2, b3];
        }
        public Int3 (byte[] array, int index = 0) 
        {
            Value = GetInt(array, index);
        }
        
        //
        // ----------------------------- METHODS ----------------------------- 
        //

        public static int GetInt(byte[] bytes, int index = 0)
        {
            return BitConverter.ToInt32(bytes, index);
        }

        //
        // ----------------------------- EXPLICIT ----------------------------- 
        //

        public static implicit operator Int3(int value)
        {
            return new Int3(value); 
        }
        public static implicit operator Int3(byte[] value)
        {
            return new Int3(value);
        }
        public static implicit operator byte[](Int3 value)
        {
            return value.Bytes;
        }
        public static implicit operator int(Int3 value)
        {
            return value.Value;
        }

        //
        // ----------------------------- ARITHMETIC ----------------------------- 
        //

        public static Int3 operator +(Int3 i1, Int3 i2)
        {
            return new Int3(GetBytes(i1.Value + i2.Value));
        }
        public static Int3 operator -(Int3 i1, Int3 i2)
        {
            return new Int3(GetBytes(i1.Value - i2.Value));
        }
        public static Int3 operator *(Int3 i1, Int3 i2)
        {
            return new Int3(GetBytes(i1.Value * i2.Value));
        }
        public static Int3 operator /(Int3 i1, Int3 i2)
        {
            return new Int3(GetBytes(i1.Value / i2.Value));
        }
        public static Int3 operator ++(Int3 value)
        {
            return new Int3(value.Value + 1);
        }
        public static Int3 operator --(Int3 value)
        {
            return new Int3(value.Value - 1);
        }

        //
        // ----------------------------- COMPARING ----------------------------- 
        //

        public static bool operator >(Int3 i1, Int3 i2)
        {
            return i1.Value > i2.Value;
        }
        public static bool operator <(Int3 i1, Int3 i2)
        {
            return i1.Value < i2.Value;
        }
        public static bool operator >=(Int3 i1, Int3 i2)
        {
            return i1.Value >= i2.Value;
        }
        public static bool operator <=(Int3 i1, Int3 i2)
        {
            return i1.Value <= i2.Value;
        }
        public static bool operator ==(Int3 i1, Int3 i2)
        {
            return i1.Value == i2.Value;
        }
        public static bool operator !=(Int3 i1, Int3 i2)
        {
            return i1.Value != i2.Value;
        }

        //
        // ----------------------------- BITWISE ----------------------------- 
        //

        public static Int3 operator %(Int3 i1, Int3 i2)
        {
            return new Int3(i1.Value % i2.Value);
        }
        public static Int3 operator &(Int3 i1, Int3 i2)
        {
            return new Int3(i1.Value & i2.Value);
        }
        public static Int3 operator |(Int3 i1, Int3 i2)
        {
            return new Int3(i1.Value | i2.Value);
        }
        public static Int3 operator ^(Int3 i1, Int3 i2)
        {
            return new Int3(i1.Value ^ i2.Value);
        }
        public static Int3 operator ~(Int3 i1)
        {
            return new Int3(~i1.Value & MaxValue);
        }
        
        //
        // ----------------------------- OVERRIDES ----------------------------- 
        //

        public override bool Equals(object obj)
        {
            if (obj is Int3 other)
                return this.Value == other.Value;

            return false;
        }
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
        public override string ToString()
        {
            return $"Int3: Value = {Value}, Bytes = {string.Join(", ", Bytes)}";
        }
    }
}
