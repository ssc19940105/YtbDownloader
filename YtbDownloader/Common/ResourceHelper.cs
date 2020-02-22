using System.Windows;

namespace YtbDownloader.Common
{
    public static class ResourceHelper
    {
        public static string FindResource(string name)
        {
            return Application.Current.FindResource(name).ToString();
        }
    }
}