using ByteDBConsole.Core.Config;
using ByteDBConsole.Core.Console;
using ByteDBConsole.Core.Misc;

namespace ByteDBConsole
{
    internal static class ByteDBConsole
    {
        public const string PromptText = "bytedb>";

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

        public static void WriteLine(string message = "")
        {
            if (message.Length > 0)
            {
                Console.WriteLine(message);
            }
        }
        public static void WriteException(Exception exception)
        {
            // Log the main exception
            Console.WriteLine($"{exception.GetType().Name}: " + exception.Message);
        }
        public static string? ReadLine(string message = "")
        {
            Console.Write(PromptText + message);
            return Console.ReadLine();
        }

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