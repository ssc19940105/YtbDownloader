using System;

namespace YtbDownloader.Core.Interfaces
{
    public interface IConfig
    {
        bool IsProxy { get; set; }

        string ProxyUrl { get; set; }

        string DownloadUrl { get; set; }

        string OutputDir { get; set; }

        bool IsAudioOnly { get; set; }

        bool IsPlaylist { get; set; }

        bool IsDebug { get; set; }

        bool IsYouGet { get; set; }

        bool DownloadSub { get; set; }

        string SubLang { get; set; }

        bool IgnoreError { get; set; }
    }
}