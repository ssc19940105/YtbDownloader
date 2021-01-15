using Catel.IoC;
using Catel.Logging;
using I18NPortable;
using I18NPortable.Readers;
using System.Windows;
using YtbDownloader.Core.Downloaders;
using YtbDownloader.Core.Interfaces;

namespace YtbDownloader
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            ServiceLocator.Default.RegisterType<IDownloader, Downloader>();
            I18N.Current.AddLocaleReader(new TextKvpReader(), ".txt")
                .SetFallbackLocale("en-US").Init(GetType().Assembly);
            LogManager.AddListener(new FileLogListener
            {
                FilePath = "{AppDataRoaming}\\Logs\\{Date}.log"
            });
        }
    }
}