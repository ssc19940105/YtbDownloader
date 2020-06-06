using System;
using System.ComponentModel;
using YtbDownloader.Core.Interfaces;

namespace YtbDownloader.Models
{
    public class Config : INotifyPropertyChanged, IConfig
    {
#pragma warning disable CS0067

        public event PropertyChangedEventHandler PropertyChanged;

#pragma warning restore CS0067

        public bool IsProxy { get; set; }

        public Uri ProxyUrl { get; set; }

        public Uri DownloadUrl { get; set; }

        public string OutputDir { get; set; }

        public bool IsAudioOnly { get; set; }

        public bool IsPlaylist { get; set; }

        public bool IsDebug { get; set; }

        public bool IsYouGet { get; set; }

        public bool IsDownloadSubs { get; set; }

        public string SubLangs { get; set; }

        public bool IsIgnoreErrors { get; set; }
    }
}