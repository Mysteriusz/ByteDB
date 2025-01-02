using ByteDBConsole.Core.Methods;

namespace ByteDBConsole.Core.Console
{
    /// <summary>
    /// Commands used in <see cref="ByteDBConsole"/> static instance.
    /// </summary>
    internal class ByteDBConsoleCommands
    {
        public static Dictionary<string, Delegate> Commands = new()
        {
            { "start", async () => await ByteDBServerMethods.StartServer() },
            { "stop", async () => await ByteDBServerMethods.StopServer() },

            { "serverinfo", () => ByteDBConsoleMethods.ServerInfo() },
            { "servername", () => ByteDBConsoleMethods.ServerName() },
            { "serverstate", () => ByteDBConsoleMethods.ServerState() },
            { "exit", () => ByteDBConsoleMethods.ExitConsole() },
        };
    }
}
