using ByteDBServer.Core.Misc;
using System.Diagnostics;

namespace ByteDBServer.Core.DataTypes
{
    [DebuggerDisplay("Value = {Value}, Bytes = {Bytes[0]}")]
    internal struct Int1
    {
        //
        // ----------------------------- CONSTANTS ----------------------------- 
        //
        public const int MaxValue = 0xff;
        public const int MinValue = 0x00;
        
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

        public Int1 () { }
        public Int1(int value)
        {
            Bytes = GetBytes(value);
        }
        public Int1 (byte b1) 
        {
            Bytes = [b1];
        }
        public Int1 (byte[] array, int index = 0) 
        {
            Value = GetInt(array, index);
        }
        
        //
        // ----------------------------- METHODS ----------------------------- 
        //

        public static byte[] GetBytes(int value)
        {
            if (value > MaxValue)
                throw new Int1OverflowException();

            byte b1 = (byte)(value & 0xff);
            byte[] bytes = [b1];

            return bytes;
        }
        public static int GetInt(byte[] bytes, int index = 0)
        {
            return bytes[index];
        }

        //
        // ----------------------------- EXPLICIT ----------------------------- 
        //

        public static explicit operator Int1(int value)
        {
            return new Int1(value); 
        }
        public static explicit operator Int1(byte[] value)
        {
            return new Int1(value);
        }
        public static explicit operator byte[](Int1 value)
        {
            return value.Bytes;
        }
        public static explicit operator int(Int1 value)
        {
            return value.Value;
        }

        //
        // ----------------------------- ARITHMETIC ----------------------------- 
        //

        public static Int1 operator +(Int1 i1, Int1 i2)
        {
            return new Int1(GetBytes(i1.Value + i2.Value));
        }
        public static Int1 operator -(Int1 i1, Int1 i2)
        {
            return new Int1(GetBytes(i1.Value - i2.Value));
        }
        public static Int1 operator *(Int1 i1, Int1 i2)
        {
            return new Int1(GetBytes(i1.Value * i2.Value));
        }
        public static Int1 operator /(Int1 i1, Int1 i2)
        {
            return new Int1(GetBytes(i1.Value / i2.Value));
        }
        public static Int1 operator ++(Int1 value)
        {
            return new Int1(value.Value + 1);
        }
        public static Int1 operator --(Int1 value)
        {
            return new Int1(value.Value - 1);
        }

        //
        // ----------------------------- COMPARING ----------------------------- 
        //

        public static bool operator >(Int1 i1, Int1 i2)
        {
            return i1.Value > i2.Value;
        }
        public static bool operator <(Int1 i1, Int1 i2)
        {
            return i1.Value < i2.Value;
        }
        public static bool operator >=(Int1 i1, Int1 i2)
        {
            return i1.Value >= i2.Value;
        }
        public static bool operator <=(Int1 i1, Int1 i2)
        {
            return i1.Value <= i2.Value;
        }
        public static bool operator ==(Int1 i1, Int1 i2)
        {
            return i1.Value == i2.Value;
        }
        public static bool operator !=(Int1 i1, Int1 i2)
        {
            return i1.Value != i2.Value;
        }

        //
        // ----------------------------- BITWISE ----------------------------- 
        //

        public static Int1 operator %(Int1 i1, Int1 i2)
        {
            return new Int1(i1.Value % i2.Value);
        }
        public static Int1 operator &(Int1 i1, Int1 i2)
        {
            return new Int1(i1.Value & i2.Value);
        }
        public static Int1 operator |(Int1 i1, Int1 i2)
        {
            return new Int1(i1.Value | i2.Value);
        }
        public static Int1 operator ^(Int1 i1, Int1 i2)
        {
            return new Int1(i1.Value ^ i2.Value);
        }
        public static Int1 operator ~(Int1 i1)
        {
            return new Int1(~i1.Value & MaxValue);
        }
        
        //
        // ----------------------------- OVERRIDES ----------------------------- 
        //

        public override bool Equals(object obj)
        {
            if (obj is Int1 other)
                return this.Value == other.Value;

            return false;
        }
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
        public override string ToString()
        {
            return $"Int1: Value = {Value}, Bytes = {string.Join(", ", Bytes)}";
        }
    }
}
