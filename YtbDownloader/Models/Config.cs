using System;
using YtbDownloader.Core.Interfaces;

namespace YtbDownloader.Models
{
    public class Config : IConfig
    {
        public bool IsProxy { get; set; }

        public Uri ProxyUrl { get; set; }

        public Uri DownloadUrl { get; set; }

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