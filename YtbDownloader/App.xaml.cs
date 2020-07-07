using Serilog;
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
            Log.Logger = new LoggerConfiguration().WriteTo
                .File(".\\logs\\.log", rollingInterval: RollingInterval.Day).CreateLogger();
        }
    }
}