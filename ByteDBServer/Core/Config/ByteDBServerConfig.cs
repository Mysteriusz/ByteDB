using System;
using System.Xml;
using System.Collections.Generic;
using ByteDBServer.Core.DataTypes;
using ByteDBServer.Core.Server;

namespace ByteDBServer.Core.Config
{
    internal static class ByteDBServerConfig
    {
        public const string ConfigPath = "Core\\Config\\ServerConfig.xml";

        public static void InitializeConfig()
        {
            XmlDocument config = new XmlDocument();
            config.LoadXml(ConfigPath);

            // Strings
            ServerVersion = config.Attributes.GetNamedItem("ServerVersion").Value;

            // Intigers
            ServerCapabilitiesInt = uint.Parse(config.Attributes.GetNamedItem("ServerCapabilities").Value);
            MaxConnections = int.Parse(config.Attributes.GetNamedItem("Port").Value);
            Port = int.Parse(config.Attributes.GetNamedItem("MaxConnections").Value);

            // Other
            Encoding = ByteDBServerEncoding.EncodingType[config.Attributes.GetNamedItem("Encoding").Value];
        }

        public static string ServerVersion { get; private set; }
        
        public static int MaxConnections { get; private set; }
        public static int Port { get; private set; }
        public static Int4 ServerCapabilitiesInt { get; private set; }

        public static System.Text.Encoding Encoding { get; private set; }

        public static List<TEnum> ReadFlags<TEnum>(uint flagValue) where TEnum : Enum
        {
            List<TEnum> result = new List<TEnum>();

            foreach (TEnum enumValue in Enum.GetValues(typeof(TEnum)))
                if ((flagValue & Convert.ToUInt32(enumValue)) != 0)
                    result.Add(enumValue);

            return result;
        }
    }
}
