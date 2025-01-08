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

        public static string Version => ByteDBServerConfig.ServerVersion;
        public static string IpAddress => ByteDBServerConfig.IpAddress;

        public static int ListeningPort => ByteDBServerConfig.Port;
        public static int MaxConnections => ByteDBServerConfig.MaxConnections;
        public static int BufferSize => ByteDBServerConfig.BufferSize;
        public static int ListeningDelay => ByteDBServerConfig.ListeningDelay;

        public static Int4 ServerCapabilitiesInt => ByteDBServerConfig.ServerCapabilitiesInt;
        public static List<ServerCapabilities> ServerCapabilities => ByteDBServerConfig.ReadFlags<ServerCapabilities>(ServerCapabilitiesInt.Value);
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
