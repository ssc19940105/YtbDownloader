using I18NPortable;
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
            I18N.Current.SetNotFoundSymbol("$")
                .SetFallbackLocale("zh-CN").Init(GetType().Assembly);
            Log.Logger = new LoggerConfiguration().WriteTo
                .File(".\\logs\\log.txt", rollingInterval: RollingInterval.Day).CreateLogger();
        }
    }
}