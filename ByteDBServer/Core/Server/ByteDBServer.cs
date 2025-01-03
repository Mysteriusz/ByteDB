using ByteDBServer.Core.Config;
using ByteDBServer.Core.Server;
using ByteDBServer.Core.Services;
using DataTypesTesting.DataTypes;
using System.Collections.Generic;
using System.ServiceProcess;

namespace ByteDBServer
{
    internal static class ByteDBServer
    {
        public static string Version => ByteDBServerConfig.ServerVersion;
        public static int ListeningPort => ByteDBServerConfig.Port;
        public static int MaxConnections => ByteDBServerConfig.MaxConnections;

        public static Int4 ServerCapabilitiesInt => ByteDBServerConfig.ServerCapabilitiesInt;
        public static List<ServerCapabilities> ServerCapabilities => ByteDBServerConfig.ReadFlags<ServerCapabilities>(ServerCapabilitiesInt.Value);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ByteDBServerConfig.InitializeConfig();

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new ByteDBServerService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
