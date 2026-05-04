using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using TVChannelManager.Library.Models;

namespace TVChannelManager.Library
{
    public static class FileManager
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        public static List<TVChannel> LoadData(string path)
        {
            if (!File.Exists(path))
                return new List<TVChannel>();
            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<TVChannel>>(json, Options) ?? new List<TVChannel>();
        }

        public static void SaveData(List<TVChannel> data, string path)
        {
            string json = JsonSerializer.Serialize(data, Options);
            File.WriteAllText(path, json);
        }
    }
}