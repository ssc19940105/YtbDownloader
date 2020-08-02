using Catel.IoC;
using Catel.Runtime.Serialization.Xml;
using System.IO;

namespace YtbDownloader.Common
{
    public class ConfigManger
    {
        private readonly string configPath;

        public ConfigManger(string path)
        {
            configPath = path;
        }

        public T Load<T>() where T : new()
        {
            if (File.Exists(configPath))
            {
                using var input = File.OpenRead(configPath);
                return (T)ServiceLocator.Default.ResolveType<IXmlSerializer>().Deserialize(typeof(T), input);
            }
            else
            {
                return new T();
            }
        }

        public void Save<T>(T config)
        {
            using var output = File.OpenWrite(configPath);
            ServiceLocator.Default.ResolveType<IXmlSerializer>().Serialize(config, output);
        }
    }
}