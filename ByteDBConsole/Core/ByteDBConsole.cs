using ByteDBConsole.Core.Config;
using ByteDBConsole.Core.Console;
using ByteDBConsole.Core.Misc;

namespace ByteDBConsole
{
    /// <summary>
    /// Provides a class representing current console instance.
    /// </summary>
    internal static class ByteDBConsole
    {
        /// <summary>
        /// Default console prompt text.
        /// </summary>
        public const string PromptText = "bytedb>";

        //
        // ----------------------------------- METHODS -----------------------------------
        //

        /// <summary>
        /// Entry point for application.
        /// </summary>
        public static void Main(string[] args)
        {
            ByteDBConsoleConfig.InitializeConfig();

            while (true)
            {
                var input = ReadLine();
                if (string.IsNullOrWhiteSpace(input)) continue;

                    try
                    {
                        RunCommand(input);
                    }
                    catch (ByteDBCommandException ex)
                    {
                        ByteDBConsole.WriteException(ex);
                    }
            }
        }

        /// <summary>
        /// Writes a message in <see cref="ByteDBConsole"/> instance. <br /><br />
        /// Arguments:<br /> <paramref name="message"/> specifies message.
        /// </summary>
        public static void WriteLine(string message = "")
        {
            if (message.Length > 0)
            {
                Console.WriteLine(message);
            }
        }

        /// <summary>
        /// Writes an <see cref="Exception"/> in <see cref="ByteDBConsole"/> instance. <br /><br />
        /// Arguments:<br /> <paramref name="exception"/> specifies exception instance.
        /// </summary>
        public static void WriteException(Exception exception)
        {
            // Log the main exception
            Console.WriteLine($"{exception.GetType().Name}: " + exception.Message);
        }

        /// <summary>
        /// Reads a message in <see cref="ByteDBConsole"/> instance. <br /><br />
        /// Arguments:<br /> <paramref name="message"/> specifies message.
        /// </summary>
        public static string? ReadLine(string message = "")
        {
            Console.Write(PromptText + message);
            return Console.ReadLine();
        }

        /// <summary>
        /// Runs a command if it`s specified in <see cref="ByteDBConsoleCommands.Commands"/>. <br /><br />
        /// Arguments:<br /> <paramref name="command"/> specifies command.
        /// </summary>
        public static void RunCommand(string command)
        {
            if (!ByteDBConsoleCommands.Commands.ContainsKey(command))
                throw new ByteDBCommandException($"'{command}' is not registered as a command.");

            Delegate commandMethod = ByteDBConsoleCommands.Commands[command];

            if (commandMethod is Func<Task> asyncMethod)
                asyncMethod().GetAwaiter().GetResult();
            else if (commandMethod is Action syncMethod)
                syncMethod();
        }
    }
}