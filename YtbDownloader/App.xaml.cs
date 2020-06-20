using I18NPortable;
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
        }
    }
}