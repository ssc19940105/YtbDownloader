using Anotar.Serilog;
using I18NPortable;
using System;
using System.IO;
using System.Text.Json;

namespace YtbDownloader.Common
{
    public class ConfigManger
    {
        private static II18N Strings => I18N.Current;

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
                    LogTo.Error(Strings["LoadConfigFailureMessage"]);
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
                LogTo.Error(Strings["SaveConfigFailureMessage"]);
                SaveFailure?.Invoke(this, null);
            }
        }
    }
}