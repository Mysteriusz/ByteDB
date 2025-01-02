using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using ByteDBConsole.Core.Misc;

namespace ByteDBConsole.Core.Config
{
    /// <summary>
    /// Provides config instance from config file.
    /// </summary>
    internal static class ByteDBConsoleConfig
    {
        public const string ConfigPath = "Core\\Config\\ConsoleConfig.json";

        public static void InitializeConfig()
        {
            string json = File.ReadAllText(ConfigPath);
            JsonObject? main = JsonSerializer.Deserialize<JsonObject>(json);

            if (main == null)
                throw new ByteDBConfigException();

            // Begin JSON File Value Reading
            JsonNode? name = main.TryGetPropertyValue("ServerExecutableName", out JsonNode? nameTemp) ? nameTemp : throw new Exception("Key 'ServerExecutableName' not found.");
            JsonNode? serverService = main.TryGetPropertyValue("ServerServiceName", out JsonNode? serviceTemp) ? serviceTemp : throw new Exception("Key 'ServerServiceName' not found.");

            ServerExecutableName = name!.ToString();
            ServerServiceName = serverService!.ToString();
        }

        public static string ServerServiceName { get; private set; } = "";
        public static string ServerExecutableName { get; private set; } = "";
    }
}
