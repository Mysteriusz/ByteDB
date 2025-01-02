using ByteDBServer.Core.Misc;
using System.Diagnostics;

namespace ByteDBServer.Core.DataTypes
{
    [DebuggerDisplay("Value = {Value}, Bytes = {Bytes[0]}, {Bytes[1]}")]
    internal struct Int2
    {
        //
        // ----------------------------- CONSTANTS ----------------------------- 
        //
        public const int MaxValue = 0xffff;
        public const int MinValue = 0x0000;
        
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

        public Int2 () { }
        public Int2(int value)
        {
            Bytes = GetBytes(value);
        }
        public Int2 (byte b1, byte b2) 
        {
            Bytes = [b1, b2];
        }
        public Int2 (byte[] array, int index = 0) 
        {
            Value = GetInt(array, index);
        }
        
        //
        // ----------------------------- METHODS ----------------------------- 
        //

        public static byte[] GetBytes(int value)
        {
            if (value > MaxValue)
                throw new Int2OverflowException();

            byte b1 = (byte)(value >> 8);
            byte b2 = (byte)(value & 0xff);
            byte[] bytes = [b1, b2];

            return bytes;
        }
        public static int GetInt(byte[] bytes, int index = 0)
        {
            return (bytes[index] << 8) | bytes[index + 1];
        }

        //
        // ----------------------------- EXPLICIT ----------------------------- 
        //

        public static explicit operator Int2(int value)
        {
            return new Int2(value); 
        }
        public static explicit operator Int2(byte[] value)
        {
            return new Int2(value);
        }
        public static explicit operator byte[](Int2 value)
        {
            return value.Bytes;
        }
        public static explicit operator int(Int2 value)
        {
            return value.Value;
        }

        //
        // ----------------------------- ARITHMETIC ----------------------------- 
        //

        public static Int2 operator +(Int2 i1, Int2 i2)
        {
            return new Int2(GetBytes(i1.Value + i2.Value));
        }
        public static Int2 operator -(Int2 i1, Int2 i2)
        {
            return new Int2(GetBytes(i1.Value - i2.Value));
        }
        public static Int2 operator *(Int2 i1, Int2 i2)
        {
            return new Int2(GetBytes(i1.Value * i2.Value));
        }
        public static Int2 operator /(Int2 i1, Int2 i2)
        {
            return new Int2(GetBytes(i1.Value / i2.Value));
        }
        public static Int2 operator ++(Int2 value)
        {
            return new Int2(value.Value + 1);
        }
        public static Int2 operator --(Int2 value)
        {
            return new Int2(value.Value - 1);
        }

        //
        // ----------------------------- COMPARING ----------------------------- 
        //

        public static bool operator >(Int2 i1, Int2 i2)
        {
            return i1.Value > i2.Value;
        }
        public static bool operator <(Int2 i1, Int2 i2)
        {
            return i1.Value < i2.Value;
        }
        public static bool operator >=(Int2 i1, Int2 i2)
        {
            return i1.Value >= i2.Value;
        }
        public static bool operator <=(Int2 i1, Int2 i2)
        {
            return i1.Value <= i2.Value;
        }
        public static bool operator ==(Int2 i1, Int2 i2)
        {
            return i1.Value == i2.Value;
        }
        public static bool operator !=(Int2 i1, Int2 i2)
        {
            return i1.Value != i2.Value;
        }

        //
        // ----------------------------- BITWISE ----------------------------- 
        //

        public static Int2 operator %(Int2 i1, Int2 i2)
        {
            return new Int2(i1.Value % i2.Value);
        }
        public static Int2 operator &(Int2 i1, Int2 i2)
        {
            return new Int2(i1.Value & i2.Value);
        }
        public static Int2 operator |(Int2 i1, Int2 i2)
        {
            return new Int2(i1.Value | i2.Value);
        }
        public static Int2 operator ^(Int2 i1, Int2 i2)
        {
            return new Int2(i1.Value ^ i2.Value);
        }
        public static Int2 operator ~(Int2 i1)
        {
            return new Int2(~i1.Value & MaxValue);
        }
        
        //
        // ----------------------------- OVERRIDES ----------------------------- 
        //

        public override bool Equals(object obj)
        {
            if (obj is Int2 other)
                return this.Value == other.Value;

            return false;
        }
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
        public override string ToString()
        {
            return $"Int2: Value = {Value}, Bytes = {string.Join(", ", Bytes)}";
        }
    }
}
