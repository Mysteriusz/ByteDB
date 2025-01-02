using ByteDBConsole.Core.Methods;

namespace ByteDBConsole.Core.Console
{
    internal class ByteDBConsoleCommands
    {
        public static Dictionary<string, Delegate> Commands = new()
        {
            { "start", async () => await ByteDBServerMethods.StartServer() },
            { "stop", async () => await ByteDBServerMethods.StopServer() },

            { "serverinfo", () => ByteDBConsoleMethods.ServerInfo() },
            { "servername", () => ByteDBConsoleMethods.ServerName() },
            { "serveruptime", () => ByteDBConsoleMethods.ServerUptime() },
            { "serverstate", () => ByteDBConsoleMethods.ServerState() },
            { "exit", () => ByteDBConsoleMethods.ExitConsole() },
        };
    }
}
