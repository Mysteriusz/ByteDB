using ByteDBConsole.Core.Config;
using ByteDBConsole.Core.Server;

namespace ByteDBConsole.Core.Methods
{
    internal class ByteDBConsoleMethods
    {
        public static void ServerInfo()
        {
            ServerName();
            ServerState();
        }
        public static void ServerName()
        {
            ByteDBConsole.WriteLine("Server Process Name: " + ByteDBConsoleConfig.ServerExecutableName);
        }
        public static void ServerState()
        {
            ByteDBConsole.WriteLine("Running: " + (ByteDBServer.IsRunning ? "Yes" : "No"));
        }
        public static void ServerUptime()
        {
            ByteDBConsole.WriteLine("Uptime: " + (ByteDBServer.IsRunning ? "Yes" : "No"));
        }
        public static void ExitConsole()
        {
            ByteDBConsole.WriteLine("Exiting");
            Environment.Exit(0);
        }
    }
}
