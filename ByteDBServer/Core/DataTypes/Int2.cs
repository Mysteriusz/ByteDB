using DataTypesTesting.DataTypes.Models;
using System;
using System.Diagnostics;

namespace DataTypesTesting.DataTypes
{
    [DebuggerDisplay("Value = {Value}, Bytes = {Bytes[0]}, {Bytes[1], Bytes[2], Bytes[3], Bytes[4], Bytes[5], Bytes[6], Bytes[7]}")]
    internal class Int2 : DataType<ushort>
    {
        //
        // ----------------------------- CONSTANTS ----------------------------- 
        //

        public const ushort MaxValue = 0xffff;
        public const ushort MinValue = 0x0000;
        public const int Length = 2;

        //
        // ----------------------------- PARAMETERS ----------------------------- 
        //

        private ushort _value = 0;
        private byte[] _bytes = new byte[3];

        public override ushort Value
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

        public Int2() { }
        public Int2(ushort value)
        {
            Bytes = GetBytes(value);
        }
        public Int2(byte b1, byte b2, byte b3, byte b4, byte b5, byte b6)
        {
            Bytes = [b1, b2, b3, b4, b5, b6];
        }
        public Int2(byte[] array, int index = 0)
        {
            Value = GetInt(array, index);
        }

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        public static ushort GetInt(byte[] bytes, int index = 0)
        {
            if (bytes.Length < Length)
            {
                byte[] resized = new byte[Length];
                Array.Copy(bytes, 0, resized, Length - bytes.Length, bytes.Length);
                bytes = resized;
            }

            return (ushort)
            (
                (bytes[index] << 8)
                | bytes[index + 1]
            );
        }

        //
        // ----------------------------- IMPLICIT ----------------------------- 
        //

        public static implicit operator Int2(ushort value)
        {
            return new Int2(value);
        }
        public static implicit operator Int2(byte[] value)
        {
            return new Int2(value);
        }
        public static implicit operator byte[](Int2 value)
        {
            return value.Bytes;
        }
        public static implicit operator ushort(Int2 value)
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
            return new Int2((ushort)(value.Value + 1));
        }
        public static Int2 operator --(Int2 value)
        {
            return new Int2((ushort)(value.Value - 1));
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
            return new Int2((ushort)(i1.Value % i2.Value));
        }
        public static Int2 operator &(Int2 i1, Int2 i2)
        {
            return new Int2((ushort)(i1.Value & i2.Value));
        }
        public static Int2 operator |(Int2 i1, Int2 i2)
        {
            return new Int2((ushort)(i1.Value | i2.Value));
        }
        public static Int2 operator ^(Int2 i1, Int2 i2)
        {
            return new Int2((ushort)(i1.Value ^ i2.Value));
        }
        public static Int2 operator ~(Int2 i1)
        {
            return new Int2((ushort)(~i1.Value & MaxValue));
        }

        //
        // ----------------------------- OVERRIDES ----------------------------- 
        //

        public override bool Equals(object obj)
        {
            if (obj is Int2 other)
                return Value == other.Value;

            return false;
        }
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
        public override string ToString()
        {
            return $"Int2: Value = {_value}, Bytes = {BitConverter.ToString(_bytes)}";
        }
    }
}
