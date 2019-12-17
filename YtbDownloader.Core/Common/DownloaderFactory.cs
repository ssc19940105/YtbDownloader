using YtbDownloader.Core.Downloaders;
using YtbDownloader.Core.Interfaces;

namespace YtbDownloader.Core.Common
{
    public static class DownloaderFactory
    {
        public static IDownloader Create()
        {
            return new Downloader();
        }
    }
}