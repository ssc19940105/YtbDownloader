using Catel.Data;
using YtbDownloader.Core.Interfaces;

namespace YtbDownloader.Models
{
    public class Config : ObservableObject, IConfig
    {
        public bool IsProxy { get; set; }

        public string ProxyUrl { get; set; }

        public string DownloadUrl { get; set; }

        public string OutputPath { get; set; }

        public bool IsAudioOnly { get; set; }

        public bool IsPlaylist { get; set; }

        public bool IsDebug { get; set; }

        public bool IsYouGet { get; set; }

        public bool DownloadSub { get; set; }

        public string SubLang { get; set; }

        public bool IgnoreError { get; set; }
    }
}