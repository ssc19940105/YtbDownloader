using System.IO;
using System.Web.Script.Serialization;

namespace YtbDownloader.Common
{
    public class ConfigManger
    {
        private readonly string configPath;
        private readonly JavaScriptSerializer serializer;

        public ConfigManger(string path)
        {
            configPath = path;
            serializer = new JavaScriptSerializer();
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
                var config = serializer.Deserialize<T>(input);
                return config != null ? config : new T();
            }
        }

        public void SaveConfig<T>(T config)
        {
            File.WriteAllText(configPath, serializer.Serialize(config));
        }
    }
}