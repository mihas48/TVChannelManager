using Newtonsoft.Json;
using TVChannelManager.Library.Models;

namespace TVChannelManager.Library
{
    public static class FileManager
    {
        public static void SaveData(List<TVChannel> channels, string path)
        {
            string json = JsonConvert.SerializeObject(channels, Formatting.Indented);
            File.WriteAllText(path, json);
        }

        public static List<TVChannel> LoadData(string path)
        {
            string json = File.ReadAllText(path);
            
            List<TVChannel> channels = new List<TVChannel>(JsonConvert.DeserializeObject<List<TVChannel>>(json));

            return channels;
        }
    }
}
