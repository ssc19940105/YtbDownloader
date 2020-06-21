using Anotar.Serilog;
using System;
using System.IO;
using System.Text.Json;

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
                try
                {
                    var input = File.ReadAllText(configPath);
                    var config = JsonSerializer.Deserialize<T>(input);
                    return config != null ? config : new T();
                }
                catch (Exception ex)
                {
                    LogTo.Error(ex.ToString());
                    return new T();
                }
            }
        }

        public void SaveConfig<T>(T config)
        {
            try
            {
                File.WriteAllText(configPath, JsonSerializer.Serialize(config));
            }
            catch (Exception ex)
            {
                LogTo.Error(ex.ToString());
            }
        }
    }
}