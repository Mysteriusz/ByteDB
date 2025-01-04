using System;
using System.IO;

namespace ByteDBServer.Core.Misc.Logs
{
    internal static class ByteDBServerLogger
    {
        //
        // ----------------------------- CONSTANTS ----------------------------- 
        //

        public static readonly string LogFileName = $"Log-{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt";
        public static readonly string LogFilePath = Path.Combine("Core", "Misc", "Logs", LogFileName);
        public static readonly string LogFileFullPath = Path.Combine(AppContext.BaseDirectory, LogFilePath);

        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //

        public static FileInfo LogFile { get { return new FileInfo(LogFileFullPath); } }

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        public static void WriteToFile(string text)
        {
            using (StreamWriter writer = LogFile.AppendText())
                writer.WriteLine($"[{DateTime.Now:yyyy-MM-dd_HH:mm:ss}] - ({text})");
        }
        public static void WriteExceptionToFile(Exception exception)
        {
            using (StreamWriter writer = LogFile.AppendText())
                writer.WriteLine($"[{DateTime.Now:yyyy-MM-dd_HH:mm:ss}] - ({exception.GetType().Name}): {exception.Message}");
        }

        public static void CreateFile(string fileName)
        {
            using (var fs = File.Create(fileName)) { }
        }
    }
}
