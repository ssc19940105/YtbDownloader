using System.IO;
using Newtonsoft.Json;

namespace YtbDownloader.Common
{
    public class ConfigManger
    {
        private readonly string configPath;

        public ConfigManger(string path)
        {
            configPath = path;
        }

        public T LoadConfig<T>() where T : new()
        {
            if (!File.Exists(configPath))
            {
                return new T();
            }
            else
            {
                var input = File.ReadAllText(configPath);
                var config = JsonConvert.DeserializeObject<T>(input);
                return config != null ? config : new T();
            }
        }

        public void SaveConfig<T>(T config)
        {
            File.WriteAllText(configPath, JsonConvert.SerializeObject(config));
        }
    }
}