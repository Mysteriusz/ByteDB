using ByteDBServer.Core.Authentication;
using ByteDBServer.Core.DataTypes;
using ByteDBServer.Core.Misc.Logs;
using System.Collections.Generic;
using ByteDBServer.Core.Services;
using ByteDBServer.Core.Config;
using System.Threading.Tasks;
using System.ServiceProcess;
using System.Text;
using System;
using ByteDBServer.Core.Server.Databases;

namespace ByteDBServer.Core.Server
{
    internal static class ByteDBServerInstance
    {
        //
        // ----------------------------- CONSTANTS ----------------------------- 
        //

        public const string DefaultWelcomeMessage = "WelcomeToByteDB";

        //
        // ----------------------------- PROPERITES ----------------------------- 
        //

        // General Server Information
        public static string Version => ByteDBServerConfig.ServerVersion;
        public static string IpAddress => ByteDBServerConfig.IpAddress;
        public static string TablesPath => ByteDBServerConfig.TablesPath;
        public static string TablesExtension => ByteDBServerConfig.TablesExtension;
        public static Dictionary<string, ByteDBTable> Tables => ByteDBServerConfig.DataTables;

        // Networking and Listening
        public static int ListeningPort => ByteDBServerConfig.Port;
        public static int ListeningDelay => ByteDBServerConfig.ListeningDelay;

        // Thread Pool Configuration
        public static int HandlerPoolSize => ByteDBServerConfig.HandlerPoolSize;
        public static int MaxConnections => ByteDBServerConfig.MaxConnections;
        public static int BufferSize => ByteDBServerConfig.BufferSize;

        // Delays Tasks Configuration
        public static Task ShortDelayTask => ByteDBServerConfig.ShortDelayTask;
        public static Task ModerateDelayTask => ByteDBServerConfig.ModerateDelayTask;
        public static Task LongDelayTask => ByteDBServerConfig.LongDelayTask;

        // Server Capabilities
        public static Int4 ServerCapabilitiesInt => ByteDBServerConfig.ServerCapabilitiesInt;

        // Server Encoding and Authentication
        public static Encoding ServerEncoding => ByteDBServerConfig.Encoding;
        public static ServerAuthenticationType ServerAuthenticationType { get; } = ServerAuthenticationType.SHA_512;
        public static Dictionary<string, Encoding> EncodingType { get; } = new Dictionary<string, Encoding>()
        {
            { "UTF8", Encoding.UTF8 },
            { "ASCII", Encoding.ASCII },
        };

        // Server Table Values Types
        public static Dictionary<string, Type> TableValueTypes { get; } = new Dictionary<string, Type>()
        {
            { "Int64", Type.GetType("System.Int64") },
            { "Int32", Type.GetType("System.Int32") },
            { "Int16", Type.GetType("System.Int16") },
            { "String", Type.GetType("System.String") },
        };

        // Query Settings
        public const char QueryEndingChar = ';';
        public const char QueryStartArgumentChar = '(';
        public const char QueryEndArgumentChar = ')';
        public const char QueryArgumentDivider = ',';
        public const char QueryValueChar = '"';

        public static readonly HashSet<char> QueryOperators = new HashSet<char>()
        {
            '=',
        };
        public static readonly HashSet<string> QueryKeywords = new HashSet<string>()
        {
            "INSERT INTO",
            "FETCH FROM",
            "DELETE FROM",
            "WHERE VALUES",
            "WITH VALUES",
            "IF CONTAINS",
        };

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ByteDBServerLogger.CreateFile(ByteDBServerLogger.LogFileFullPath);
            ByteDBServerConfig.InitializeConfig();
            ByteDBAuthenticator.InitializeUsers();

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new ByteDBServerService()
            };
            ServiceBase.Run(ServicesToRun);
        }

        public static bool CheckCapability(ServerCapabilities capability, Int4 capabilitiesInt)
        {
            return (capabilitiesInt.Value & (uint)capability) != 0;
        }
    }
}
