using System;

namespace YtbDownloader.Core.Interfaces
{
    public interface IConfig
    {
        bool IsProxy { get; set; }

        Uri ProxyUrl { get; set; }

        Uri DownloadUrl { get; set; }

        string OutputDir { get; set; }

        bool IsAudioOnly { get; set; }

        bool IsPlaylist { get; set; }

        bool IsDebug { get; set; }

        bool IsYouGet { get; set; }

        bool IsDownloadSubs { get; set; }

        string SubLangs { get; set; }
    }
}