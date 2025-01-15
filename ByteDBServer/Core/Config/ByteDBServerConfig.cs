using System;
using System.Text;
using System.Collections.Generic;
using ByteDBServer.Core.DataTypes;
using ByteDBServer.Core.Server;
using System.Xml.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Data;
using ByteDBServer.Core.Server.Databases;
using ByteDBServer.Core.Misc.Logs;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ByteDBServer.Core.Config
{
    internal static class ByteDBServerConfig
    {
        //
        // ----------------------------- CONSTANTS ----------------------------- 
        //
 
        public const string ConfigPath = "Core\\Config\\ServerConfig.xml";

        //
        // ----------------------------- SERVER DATA ----------------------------- 
        //

        public static string ServerVersion { get; private set; }
        public static Int4 ServerCapabilitiesInt { get; private set; }
        public static Encoding Encoding { get; private set; }

        //
        // ----------------------------- CONNECTION DATA ----------------------------- 
        //

        public static string IpAddress { get; private set; }
        public static string TablesPath { get; private set; }
        public static string TablesExtension { get; private set; }

        public static int Port { get; private set; }
        public static int MaxConnections { get; private set; }
        public static int BufferSize { get; private set; }
        public static int ListeningDelay { get; private set; }
        
        public static int HandlerPoolSize { get; private set; }

        public static int ShortDelayDuration { get; private set; }
        public static int ModerateDelayDuration { get; private set; }
        public static int LongDelayDuration { get; private set; }

        public static Task ShortDelayTask => Task.Delay(ShortDelayDuration);
        public static Task ModerateDelayTask => Task.Delay(ModerateDelayDuration);
        public static Task LongDelayTask => Task.Delay(LongDelayDuration);

        public static Dictionary<string, ByteDBTable> DataTables = new Dictionary<string, ByteDBTable>();
        
        //
        // ----------------------------- METHODS ----------------------------- 
        //

        public static void InitializeConfig()
        {
            XDocument config = XDocument.Load(Path.Combine(AppContext.BaseDirectory + ConfigPath));

            //
            // ----------------------------- READ ALL CONFIG VALUES ----------------------------- 
            //

            // Strings
            ServerVersion = config.Root.Element("ServerVersion").Value;
            IpAddress = config.Root.Element("Address").Value;
            TablesPath = Path.Combine(AppContext.BaseDirectory, config.Root.Element("TablesPath").Value);
            TablesExtension = config.Root.Element("TablesExtension").Value;

            // Intigers
            Port = int.Parse(config.Root.Element("Port").Value);
            BufferSize = int.Parse(config.Root.Element("BufferSize").Value);
            
            MaxConnections = int.Parse(config.Root.Element("MaxConnections").Value);
            HandlerPoolSize = int.Parse(config.Root.Element("HandlerPoolSize").Value);
            
            ServerCapabilitiesInt = new Int4(uint.Parse(config.Root.Element("ServerCapabilities").Value));

            ShortDelayDuration = int.Parse(config.Root.Element("ShortDelayDuration").Value);
            ModerateDelayDuration = int.Parse(config.Root.Element("ModerateDelayDuration").Value);
            LongDelayDuration = int.Parse(config.Root.Element("LongDelayDuration").Value);

            // Other
            Encoding = ByteDBServerInstance.EncodingType[config.Root.Element("Encoding").Value];
            foreach (var file in Directory.GetFiles(TablesPath))
            {
                try
                {
                    DataTables.Add(file, ByteDBTable.Load(Path.Combine(TablesPath, file)));
                }
                catch
                {
                    ByteDBServerLogger.WriteToFile("File: " + file + " Failed to load.", LogType.Error);
                    continue;
                }
            }
        }
    }
}
