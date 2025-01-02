using System.Xml;

namespace ByteDBServer.Core.Config
{
    internal static class ByteDBServerConfig
    {
        public const string ConfigPath = "Core\\Config\\ServerConfig.xml";

        public static void InitializeConfig()
        {
            XmlDocument config = new XmlDocument();
            config.LoadXml(ConfigPath);

            ServerVersion = config.Attributes.GetNamedItem("ServerVersion").Value;
        }

        public static string ServerVersion { get; private set; }
    }
}
