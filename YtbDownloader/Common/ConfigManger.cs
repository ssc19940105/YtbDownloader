using Anotar.Serilog;
using System;
using System.IO;
using System.Text.Json;
using YtbDownloader.Properties;

namespace YtbDownloader.Common
{
    public class ConfigManger
    {
        private readonly string configPath;

        public event EventHandler LoadFailure;

        public event EventHandler SaveFailure;

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
                    LogTo.Error(Resources.LoadConfigFailureMessage);
                    LoadFailure?.Invoke(this, null);
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
                LogTo.Error(Resources.SaveConfigFailureMessage);
                SaveFailure?.Invoke(this, null);
            }
        }
    }
}