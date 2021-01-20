using System;
using YtbDownloader.Core.Common;

namespace YtbDownloader.Core.Interfaces
{
    public interface IDownloader
    {
        bool IsBusy { get; }

        event EventHandler DownloadStarted;

        event EventHandler DownloadCompleted;

        event EventHandler<LogReceivedEventArgs> LogReceived;

        void Download(IConfig config);

        void Cancel();
    }
}