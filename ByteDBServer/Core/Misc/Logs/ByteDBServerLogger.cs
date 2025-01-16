using System.IO;
using System;
using ByteDBServer.Core.Server;
using System.Threading.Tasks;

namespace ByteDBServer.Core.Misc.Logs
{
    internal static class ByteDBServerLogger
    {
        //
        // ----------------------------- CONSTANTS ----------------------------- 
        //

        /// <summary>
        /// Name of the log file.
        /// </summary>
        public static readonly string LogFileName = $"Log-{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt";

        /// <summary>
        /// Log file`s path.
        /// </summary>
        public static readonly string LogFilePath = Path.Combine("Core", "Misc", "Logs", LogFileName);

        /// <summary>
        /// Log file`s full path.
        /// </summary>
        public static readonly string LogFileFullPath = Path.Combine(AppContext.BaseDirectory, LogFilePath);

        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //

        /// <summary>
        /// Log file`s info from <see cref="LogFileFullPath"/>.
        /// </summary>
        public static FileInfo LogFile { get { return new FileInfo(LogFileFullPath); } }

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        /// <summary>
        /// Synchronously writes log to created file from <see cref="CreateFile(string)"/>.
        /// </summary>
        /// <param name="text">Log message.</param>
        /// <param name="logType">Type of log ex: <see cref="[MESSAGE]"/>, <see cref="[WARNING]"/>.</param>
        public static void WriteToFile(string text, LogType logType = LogType.MESSAGE)
        {
            using (StreamWriter writer = LogFile.AppendText())
                writer.WriteLine($"[{logType}] [{DateTime.Now:yyyy-MM-dd_HH:mm:ss}] - ({text})");
        }

        /// <summary>
        /// Asynchronously writes log to created file from <see cref="CreateFile(string)"/>.
        /// </summary>
        /// <param name="text">Log message.</param>
        /// <param name="logType">Type of log ex: <see cref="[MESSAGE]"/>, <see cref="[WARNING]"/>.</param>
        public static async Task WriteToFileAsync(string text, LogType logType = LogType.MESSAGE)
        {
            using (StreamWriter writer = LogFile.AppendText())
                await writer.WriteLineAsync($"[{logType}] [{DateTime.Now:yyyy-MM-dd_HH:mm:ss}] - ({text})");
        }

        /// <summary>
        /// Synchronously writes an exception to created file from <see cref="CreateFile(string)"/>.
        /// </summary>
        /// <param name="exception">Exception to write.</param>
        public static void WriteExceptionToFile(Exception exception)
        {
            using (StreamWriter writer = LogFile.AppendText())
            {
                writer.WriteLine($"[{DateTime.Now:yyyy-MM-dd_HH:mm:ss}] - ({exception.GetType().Name}): {exception.Message}");
                writer.WriteLine("Stack Trace:");
                writer.WriteLine(exception.StackTrace);

                if (exception.InnerException != null)
                {
                    writer.WriteLine("Inner Exception:");
                    WriteInnerException(writer, exception.InnerException);
                }

                writer.WriteLine();
            }
        }

        /// <summary>
        /// Asynchronously writes an exception to created file from <see cref="CreateFile(string)"/>.
        /// </summary>
        /// <param name="exception">Exception to write.</param>
        public static async Task WriteExceptionToFileAsync(Exception exception)
        {
            using (StreamWriter writer = LogFile.AppendText())
            {
                await writer.WriteLineAsync($"[{DateTime.Now:yyyy-MM-dd_HH:mm:ss}] - ({exception.GetType().Name}): {exception.Message}");
                await writer.WriteAsync("Stack Trace:");
                await writer.WriteAsync(exception.StackTrace);

                if (exception.InnerException != null)
                {
                    await writer.WriteLineAsync("Inner Exception:");
                    await WriteInnerExceptionAsync(writer, exception.InnerException);
                }

                await writer.WriteLineAsync();
            }
        }

        private static void WriteInnerException(StreamWriter writer, Exception innerException)
        {
            writer.WriteLine($"\t({innerException.GetType().Name}): {innerException.Message}");
            writer.WriteLine("\tStack Trace:");
            writer.WriteLine(innerException.StackTrace);

            if (innerException.InnerException != null)
            {
                writer.WriteLine("\tInner Exception:");
                WriteInnerException(writer, innerException.InnerException);
            }
        }
        private static async Task WriteInnerExceptionAsync(StreamWriter writer, Exception innerException)
        {
            await writer.WriteLineAsync($"\t({innerException.GetType().Name}): {innerException.Message}");
            await writer.WriteLineAsync("\tStack Trace:");
            await writer.WriteLineAsync(innerException.StackTrace);

            if (innerException.InnerException != null)
            {
                await writer.WriteLineAsync("\tInner Exception:");
                await WriteInnerExceptionAsync(writer, innerException.InnerException);
            }
        }

        /// <summary>
        /// Creates a log file on specified path.
        /// </summary>
        /// <param name="fileName">Full file path with name and extension.</param>
        public static void CreateFile(string fileName)
        {
            using (var fs = File.Create(fileName)) { }
        }
    }
}
