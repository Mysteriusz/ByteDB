using ByteDBConsole.Core.Config;
using ByteDBConsole.Core.Misc;
using ByteDBConsole.Core.Server;

namespace ByteDBConsole.Core.Methods
{
    internal class ByteDBServerMethods
    {
        public static async Task StartServer()
        {
            ByteDBConsole.WriteLine("Starting...");

            try
            {
                await ByteDBServer.StartAsync();
                ByteDBConsole.WriteLine("Server started!");
            }
            catch (ByteDBProcessException ex)
            {
                ByteDBConsole.WriteException(ex);
            }
        }
        public static async Task StopServer()
        {
            ByteDBConsole.WriteLine("Stopping...");

            try
            {
                await ByteDBServer.StopAsync();
                ByteDBConsole.WriteLine("Server stopped!");
            }
            catch (ByteDBProcessException ex)
            {
                ByteDBConsole.WriteException(ex);
            }
        }
        public static async Task RestartServer()
        {
            ByteDBConsole.WriteLine("Restarting...");

            try
            {
                await ByteDBServer.RestartAsync();
                ByteDBConsole.WriteLine("Server restarted!");
            }
            catch (ByteDBProcessException ex)
            {
                ByteDBConsole.WriteException(ex);
            }
        }
    }
}