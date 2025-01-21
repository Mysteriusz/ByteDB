using System.Collections.Generic;
using System;
using System.Linq;

namespace ByteDBServer.Core.Server.Tables
{
    public static class ByteDBTableConstraints
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
        public struct ConstrainInfo
        {
            public bool IsUnique { get; set; }
            public bool IsNotNull { get; set; }
            public bool HasDefault { get; set; }
            public object DefaultValue { get; set; }

            public ConstrainInfo(bool isUnique = false, bool isNotNull = false, bool hasDefault = false, object defaultValue = null)
            {
                IsUnique = isUnique;
                IsNotNull = isNotNull;
                HasDefault = hasDefault;
                DefaultValue = defaultValue;
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
        public static readonly Dictionary<string, ConstrainInfo> Constraints = new Dictionary<string, ConstrainInfo>()
        {
            { "NOTNULL", new ConstrainInfo(isNotNull: true, defaultValue: false) },
            { "UNIQUE", new ConstrainInfo(isUnique: true, defaultValue: false) },
        };

        public static bool ValidateConstraints(IEnumerable<string> constraints)
        {
            return constraints.All(Constraints.ContainsKey);
        }

        public static bool ValidateConstraints(IEnumerable<IEnumerable<string>> constraints)
        {
            return constraints.All(innerList => innerList.All(Constraints.ContainsKey));
        }

        public static bool ValidateTypes(IEnumerable<string> types)
        {
            return types.All(TypeDefinitions.ContainsKey);
        }

        public static bool ValidateConstraint(string constraint)
        {
            return Constraints.ContainsKey(constraint);
        }

        public static bool ValidateType(string type)
        {
            return TypeDefinitions.ContainsKey(type);
        }
    }
}
