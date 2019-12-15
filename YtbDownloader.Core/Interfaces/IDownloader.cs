using System;
using YtbDownloader.Core.Common;

namespace YtbDownloader.Core.Interfaces
{
    public interface IDownloader
    {
        bool IsBusy { get; }

        event EventHandler DowndloadStart;

        event EventHandler DowndloadComplete;

        event EventHandler<LogReceivedEventArgs> LogReceived;

        void Download(IConfig config);

        void Cancel();
    }
}