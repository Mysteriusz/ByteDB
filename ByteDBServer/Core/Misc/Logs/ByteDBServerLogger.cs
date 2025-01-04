using System;
using System.IO;

namespace ByteDBServer.Core.Misc.Logs
{
    internal static class ByteDBServerLogger
    {
        //
        // ----------------------------- CONSTANTS ----------------------------- 
        //

        public static readonly string LogFilePath = Path.Combine(AppContext.BaseDirectory, "Misc", "Logs");
        public static readonly string LogFileName = $"Log-{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt";

        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //

        public static FileInfo LogFile { get; } = new FileInfo(Path.Combine(LogFilePath, LogFileName));

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        public static void WriteToFile(string text)
        {
            using (StreamWriter writer = LogFile.AppendText())
                writer.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] - ({text})");
        }
        public static void WriteExceptionToFile(Exception exception)
        {
            using (StreamWriter writer = LogFile.AppendText())
                writer.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] - ({exception.GetType().Name}): {exception.Message}");
        }

        public static void CreateFile(string fileName)
        {
            File.Create(LogFile.FullName);
        }
    }
}
