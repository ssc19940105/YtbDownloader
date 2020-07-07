using Catel.Logging;
using System.Windows;

namespace YtbDownloader
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            LogManager.AddListener(new FileLogListener
            {
                FilePath = "{AppDir}\\logs\\{Date}.log"
            });
        }
    }
}