using ByteDBServer.Core.Misc;
using System.Diagnostics;

namespace ByteDBServer.Core.DataTypes
{
    [DebuggerDisplay("Value = {Value}, Bytes = {Bytes[0]}, {Bytes[1], Bytes[2]}")]
    internal struct Int3
    {
        //
        // ----------------------------- CONSTANTS ----------------------------- 
        //
        public const int MaxValue = 0xffffff;
        public const int MinValue = 0x000000;
        
        //
        // ----------------------------- PARAMETERS ----------------------------- 
        //
        
        public int Value
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

        public static byte[] GetBytes(int value)
        {
            if (value > MaxValue)
                throw new Int3OverflowException();

            byte b1 = (byte)(value >> 16);
            byte b2 = (byte)(value >> 8);
            byte b3 = (byte)(value & 0xff);
            byte[] bytes = [b1, b2, b3];

            return bytes;
        }
        public static int GetInt(byte[] bytes, int index = 0)
        {
            return (bytes[index] << 16) | (bytes[index + 1] << 8) | bytes[index + 2];
        }

        //
        // ----------------------------- EXPLICIT ----------------------------- 
        //

        public static explicit operator Int3(int value)
        {
            return new Int3(value); 
        }
        public static explicit operator Int3(byte[] value)
        {
            return new Int3(value);
        }
        public static explicit operator byte[](Int3 value)
        {
            return value.Bytes;
        }
        public static explicit operator int(Int3 value)
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
