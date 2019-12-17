using System;
using YtbDownloader.Core.Common;

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

        DownloadEngine Engine { get; set; }
    }
}