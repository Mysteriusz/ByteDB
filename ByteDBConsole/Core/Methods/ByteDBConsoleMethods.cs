using ByteDBConsole.Core.Config;
using ByteDBConsole.Core.Server;

namespace ByteDBConsole.Core.Methods
{
    internal class ByteDBConsoleMethods
    {
        /// <summary>
        /// Writes information about server in <see cref="ByteDBConsole"/> static instance.
        /// </summary>
        public static void ServerInfo()
        {
            ServerName();
            ServerState();
        }

        /// <summary>
        /// Writes server`s process name in <see cref="ByteDBConsole"/> static instance.
        /// </summary>
        public static void ServerName()
        {
            ByteDBConsole.WriteLine("Server Process Name: " + ByteDBConsoleConfig.ServerExecutableName);
        }

        /// <summary>
        /// Writes server`s process state in <see cref="ByteDBConsole"/> static instance.
        /// </summary>
        public static void ServerState()
        {
            ByteDBConsole.WriteLine("Running: " + (ByteDBServer.IsRunning ? "Yes" : "No"));
        }

        /// <summary>
        /// Closes <see cref="ByteDBConsole"/> static instance.
        /// </summary>
        public static void ExitConsole()
        {
            ByteDBConsole.WriteLine("Exiting");
            Environment.Exit(0);
        }
    }
}
