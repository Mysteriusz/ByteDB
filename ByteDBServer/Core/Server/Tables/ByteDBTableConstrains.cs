using System.Collections.Generic;
using System;

namespace ByteDBServer.Core.Server.Tables
{
    public static class ByteDBTableConstrains
    {
        public struct TypeInfo 
        {
            public object MaxValue { get; set; }
            public object MinValue { get; set; }
            public Type SystemType { get; set; }

            public TypeInfo(object maxValue, object minValue, Type systemType)
            {
                MaxValue = maxValue;
                MinValue = minValue;
                SystemType = systemType;
            }
        }
        
        public static readonly Dictionary<string, TypeInfo> TypeDefinitions = new Dictionary<string, TypeInfo>
        {
            { "Byte", new TypeInfo(byte.MaxValue, byte.MinValue, typeof(Byte)) },

            { "Int16", new TypeInfo(short.MaxValue, short.MinValue, typeof(Int16)) },
            { "UInt16", new TypeInfo(ushort.MaxValue, ushort.MinValue, typeof(UInt16)) },

            { "Int32", new TypeInfo(int.MaxValue, int.MinValue, typeof(Int32)) },
            { "UInt32", new TypeInfo(uint.MaxValue, uint.MinValue, typeof(UInt32)) },

            { "Int64", new TypeInfo(long.MaxValue, long.MinValue, typeof(Int64)) },
            { "UInt64", new TypeInfo(ulong.MaxValue, ulong.MinValue, typeof(UInt64)) },

            { "Boolean", new TypeInfo(bool.TrueString, bool.FalseString, typeof(Boolean)) },

            { "SmallText", new TypeInfo(255, 0, typeof(string)) },
            { "MediumText", new TypeInfo(65535, 0, typeof(string)) },
            { "LongText", new TypeInfo(16777215, 0, typeof(string)) },
        };

        public static bool Validate(string value, string type)
        {
            if (!TypeDefinitions.ContainsKey(type))
                return false;

            TypeInfo typeInfo = TypeDefinitions[type];

            if (typeInfo.SystemType == typeof(string))
            {
                int maxLength = (int)typeInfo.MaxValue;

                if (value.Length > maxLength)
                    return false;
            }
            else
            {
                try
                {
                    object parsedValue = Convert.ChangeType(value, typeInfo.SystemType);

                    if (parsedValue is IComparable comparableValue)
                    {
                        if (typeInfo.MinValue is IComparable min && typeInfo.MaxValue is IComparable max)
                        {
                            return comparableValue.CompareTo(min) >= 0 && comparableValue.CompareTo(max) <= 0;
                        }
                    }
                }
                catch { return false; }
            }

            return true;
        }
    }
}
