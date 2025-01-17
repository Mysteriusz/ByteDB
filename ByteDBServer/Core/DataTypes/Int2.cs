﻿using ByteDBServer.DataTypes.Models;
using System;

namespace ByteDBServer.Core.DataTypes
{
    internal class Int2 : DataType<ushort>, IDisposable
    {
        //
        // ----------------------------- CONSTANTS ----------------------------- 
        //

        public const ushort MaxValue = 0xffff;
        public const ushort MinValue = 0x0000;
        public const int Length = 2;

        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //

        private ushort _value = 0;
        private byte[] _bytes = new byte[2];

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
        public Int2(byte b1, byte b2)
        {
            Bytes = [b1, b2];
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
