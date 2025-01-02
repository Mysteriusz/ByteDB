using ByteDBServer.Core.Services;
using System.ServiceProcess;

namespace ByteDBServer
{
    internal static class ByteDBServer
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new ByteDBServerService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
