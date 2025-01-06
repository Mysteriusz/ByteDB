using ByteDBServer.Core.Config;
using ByteDBServer.Core.Misc.Logs;
using ByteDBServer.Core.Server;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.ServiceProcess;

namespace ByteDBServer.Core.Services
{
    partial class ByteDBServerService : ServiceBase
    {
        private ByteDBServerListener _listener;

        public ByteDBServerService()
        {
            InitializeComponent();
         
            this.ServiceName = "ByteDBServerService";
            this.CanShutdown = true;
            this.AutoLog = true;

            ServiceStatus serviceStatus = new ServiceStatus()
            {
                dwCurrentState = ServiceState.SERVICE_START_PENDING,
                dwWaitHint = 10000
            };
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        protected override void OnStart(string[] args)
        {
            // TODO: Add code here to start your service.

            ByteDBServerLogger.WriteToFile("Server Started!");

            _listener = new ByteDBServerListener(ByteDBServerInstance.IpAddress, ByteDBServerInstance.ListeningPort);
            _listener.StartListening();
        }

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.

            ByteDBServerLogger.WriteToFile("Server Stopped!");

            _listener.StopListening();
            _listener = null;
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(System.IntPtr handle, ref ServiceStatus serviceStatus);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ServiceStatus
    {
        public int dwServiceType;
        public ServiceState dwCurrentState;
        public int dwControlsAccepted;
        public int dwWin32ExitCode;
        public int dwServiceSpecificExitCode;
        public int dwCheckPoint;
        public int dwWaitHint;
    };
    public enum ServiceState
    {
        SERVICE_STOPPED = 0x00000001,
        SERVICE_START_PENDING = 0x00000002,
        SERVICE_STOP_PENDING = 0x00000003,
        SERVICE_RUNNING = 0x00000004,
        SERVICE_CONTINUE_PENDING = 0x00000005,
        SERVICE_PAUSE_PENDING = 0x00000006,
        SERVICE_PAUSED = 0x00000007,
    }
}
