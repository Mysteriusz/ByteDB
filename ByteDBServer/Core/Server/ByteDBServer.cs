using System;
using System.Collections.Generic;
using System.ServiceProcess;
using ByteDBServer.Core.Config;
using ByteDBServer.Core.DataTypes;
using ByteDBServer.Core.Misc.Logs;
using ByteDBServer.Core.Services;

namespace ByteDBServer.Core.Server
{
    internal static class ByteDBServer
    {
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

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ByteDBServerConfig.InitializeConfig();
            ByteDBServerLogger.CreateFile(ByteDBServerLogger.LogFileName);

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new ByteDBServerService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
