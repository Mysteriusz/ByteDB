using ByteDBServer.Core.Config;
using ByteDBServer.Core.Services;
using System.ServiceProcess;

namespace ByteDBServer
{
    internal static class ByteDBServer
    {
        public static string Version => ByteDBServerConfig.ServerVersion;

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
