using Catel.MVVM;
using YtbDownloader.Core.Interfaces;

namespace YtbDownloader.Models
{
    public class Config : ViewModelBase, IConfig
    {
        public bool IsProxy { get; set; }

        public string ProxyUrl { get; set; }

        public string DownloadUrl { get; set; }

        public string OutputDir { get; set; }

        public bool IsAudioOnly { get; set; }

        public bool IsPlaylist { get; set; }

        public bool IsDebug { get; set; }

        public bool IsYouGet { get; set; }

        public bool DownloadSub { get; set; }

        public string SubLang { get; set; }

        public bool IgnoreError { get; set; }
    }
}