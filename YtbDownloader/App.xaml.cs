using Catel.Logging;
using I18NPortable;
using I18NPortable.Readers;
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
            I18N.Current.AddLocaleReader(new TextKvpReader(), ".txt")
                .SetFallbackLocale("en-US").Init(GetType().Assembly);
            LogManager.AddListener(new FileLogListener
            {
                FilePath = "{AppDataRoaming}\\Logs\\{Date}.log"
            });
        }
    }
}