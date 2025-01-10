using ByteDBServer.Core.Authentication;
using ByteDBServer.Core.DataTypes;
using ByteDBServer.Core.Misc.Logs;
using System.Collections.Generic;
using ByteDBServer.Core.Services;
using ByteDBServer.Core.Config;
using System.ServiceProcess;
using System.Text;

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

        // Networking and Listening
        public static int ListeningPort => ByteDBServerConfig.Port;
        public static int ListeningDelay => ByteDBServerConfig.ListeningDelay;

        // Thread Pool Configuration
        public static int HandlerPoolSize => ByteDBServerConfig.HandlerPoolSize;
        public static int MaxConnections => ByteDBServerConfig.MaxConnections;
        public static int BufferSize => ByteDBServerConfig.BufferSize;

        // Server Capabilities
        public static Int4 ServerCapabilitiesInt => ByteDBServerConfig.ServerCapabilitiesInt;
        public static List<ServerCapabilities> ServerCapabilities => ByteDBServerConfig.ReadFlags<ServerCapabilities>(ServerCapabilitiesInt.Value);

        // Server Encoding and Authentication
        public static Encoding ServerEncoding => ByteDBServerConfig.Encoding;
        public static ServerAuthenticationType ServerAuthenticationType = ServerAuthenticationType.SHA512;

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ByteDBServerConfig.InitializeConfig();
            ByteDBServerLogger.CreateFile(ByteDBServerLogger.LogFileFullPath);
            ByteDBAuthenticator.InitializeUsers();

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new ByteDBServerService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
