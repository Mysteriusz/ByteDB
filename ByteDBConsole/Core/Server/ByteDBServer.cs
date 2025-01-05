using System.Diagnostics;
using System.Reflection.PortableExecutable;
using ByteDBConsole.Core.Config;
using ByteDBConsole.Core.Misc;

namespace ByteDBConsole.Core.Server
{
    /// <summary>
    /// Provides a class representing current server instance.
    /// </summary>
    internal static class ByteDBServer
    {
        /// <summary>
        /// Gets the running state of the server.
        /// </summary>
        /// <returns>True if the server is running; otherwise, false.</returns>
        public static bool IsRunning
        {
            get
            {
                try
                {
                    ProcessStartInfo processInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/C sc query {ByteDBConsoleConfig.ServerServiceName}",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    using (Process process = Process.Start(processInfo)!)
                    {
                        using (var reader = process.StandardOutput)
                        {
                            string output = reader.ReadToEnd();

                            // Check if the service is running
                            return output.Contains("RUNNING");
                        }
                    }
                }
                catch
                {
                    ByteDBConsole.WriteException(new ByteDBProcessException("Problem checking service."));
                    return false;
                }
            }
        }

        //
        // ----------------------------------- METHODS -----------------------------------
        //

        /// <summary>
        /// Starts the server synchronously.
        /// </summary>
        /// <exception cref="ByteDBProcessException">
        /// Thrown if an error occurs while starting the server.
        /// </exception>
        public static void Start()
        {
            // Check if server is running and if so throw console exception
            if (IsRunning)
                throw new ByteDBProcessException("Server process is already running");

            string command = $"net start {ByteDBConsoleConfig.ServerServiceName}";
            
            try
            {
                QueryCMD(command, true);
            }
            catch (Exception ex)
            {
                throw new ByteDBProcessException(ex.Message);
            }
        }

        /// <summary>
        /// Starts the server asynchronously.
        /// </summary>
        /// <exception cref="ByteDBProcessException">
        /// Thrown if an error occurs while starting the server.
        /// </exception>
        public static async Task StartAsync()
        {
            // Check if server is running and if so throw console exception
            if (IsRunning)
                throw new ByteDBProcessException("Server process is already running");
            
            string command = $"net start {ByteDBConsoleConfig.ServerServiceName}";

            try
            {
                QueryCMD(command, true);
            }
            catch (Exception ex)
            {
                throw new ByteDBProcessException(ex.Message);
            }

            await Task.CompletedTask;
        }

        /// <summary>
        /// Stops the server synchronously.
        /// </summary>
        /// <exception cref="ByteDBProcessException">
        /// Thrown if an error occurs while stopping the server.
        /// </exception>
        public static void Stop()
        {
            // Check if server is not running and if so throw console exception
            if (!IsRunning)
                throw new ByteDBProcessException("Server process is not running");
            
            string command = $"net stop {ByteDBConsoleConfig.ServerServiceName}";

            try
            {
                QueryCMD(command, true);
            }
            catch (Exception ex)
            {
                throw new ByteDBProcessException(ex.Message);
            }
        }

        /// <summary>
        /// Stops the server asynchronously.
        /// </summary>
        /// <exception cref="ByteDBProcessException">
        /// Thrown if an error occurs while stopping the server.
        /// </exception>
        public static async Task StopAsync()
        {
            // Check if server is not running and if so throw console exception
            if (!IsRunning)
                throw new ByteDBProcessException("Server process is not running");

            string command = $"net stop {ByteDBConsoleConfig.ServerServiceName}";

            try
            {
                QueryCMD(command, true);
            }
            catch (Exception ex)
            {
                throw new ByteDBProcessException(ex.Message);
            }

            await Task.CompletedTask;
        }

        /// <summary>
        /// Restarts the server synchronously.
        /// </summary>
        /// <exception cref="ByteDBProcessException">
        /// Thrown if an error occurs while restarting the server.
        /// </exception>
        public static void Restart()
        {
            // Check if server is not running and if so throw console exception
            if (!IsRunning)
                throw new ByteDBProcessException("Server process is not running");

            Stop();
            Start();
        }

        /// <summary>
        /// Restarts the server asynchronously.
        /// </summary>
        /// <exception cref="ByteDBProcessException">
        /// Thrown if an error occurs while restarting the server.
        /// </exception>
        public static async Task RestartAsync()
        {
            // Check if server is not running and if so throw console exception
            if (!IsRunning)
                throw new ByteDBProcessException("Server process is not running");

            await StopAsync();
            await StartAsync();
        }

        private static void QueryCMD(string command, bool admin = false, bool nowindow = true)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/C " + command,
                Verb = admin ? "runas" : "",
                UseShellExecute = true,
                CreateNoWindow = nowindow,
            };

            try
            {
                using (Process process = Process.Start(processInfo)!)
                    process.WaitForExit();
            }
            catch (Exception ex)
            {
                throw new ByteDBProcessException(ex.Message);
            }
        }
    }
}
