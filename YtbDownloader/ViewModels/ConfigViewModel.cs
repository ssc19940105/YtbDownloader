using System;
using System.ComponentModel;
using YtbDownloader.Core.Common;
using YtbDownloader.Core.Interfaces;

namespace YtbDownloader.ViewModels
{
    public class ConfigViewModel : INotifyPropertyChanged, IConfig
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsProxy { get; set; }

        public Uri ProxyUrl { get; set; }

        public Uri DownloadUrl { get; set; }

        public string OutputDir { get; set; }

        public bool IsAudioOnly { get; set; }

        public bool IsPlaylist { get; set; }

        public bool IsDebug { get; set; }

        public DownloadEngine Engine { get; set; }

        public ConfigViewModel()
        {
            IsProxy = Properties.Settings.Default.IsProxy;
            ProxyUrl = Properties.Settings.Default.ProxyUrl;
            DownloadUrl = Properties.Settings.Default.DownloadUrl;
            IsAudioOnly = Properties.Settings.Default.IsAudioOnly;
            OutputDir = Properties.Settings.Default.OutputDir;
            IsPlaylist = Properties.Settings.Default.IsPlaylist;
            IsDebug = Properties.Settings.Default.IsDebug;
            Engine = Properties.Settings.Default.Engine;
        }

        ~ConfigViewModel()
        {
            Properties.Settings.Default.IsProxy = IsProxy;
            Properties.Settings.Default.ProxyUrl = ProxyUrl;
            Properties.Settings.Default.DownloadUrl = DownloadUrl;
            Properties.Settings.Default.IsAudioOnly = IsAudioOnly;
            Properties.Settings.Default.OutputDir = OutputDir;
            Properties.Settings.Default.IsPlaylist = IsPlaylist;
            Properties.Settings.Default.IsDebug = IsDebug;
            Properties.Settings.Default.Engine = Engine;
            Properties.Settings.Default.Save();
        }
    }
}