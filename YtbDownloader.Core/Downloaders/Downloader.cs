using CommandLine;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using YtbDownloader.Core.Common;
using YtbDownloader.Core.Interfaces;
using YtbDownloader.Core.Options;
using YtbDownloader.Core.Win32;

namespace YtbDownloader.Core.Downloaders
{
    public class Downloader : IDownloader, IDisposable
    {
        private Process process;
        private const string AudioFormat = "bestaudio[ext=m4a]/bestaudio";
        private const string VideoFormat = "bestvideo[ext=mp4]+bestaudio[ext=m4a]/best[ext=mp4]/best";
        private const string NoPlaylistOutputTemplate = "%(title)s.%(ext)s";
        private const string PlaylistOutputTemplate = "%(playlist)s/%(title)s.%(ext)s";

        public event EventHandler DowndloadStart;

        public event EventHandler DowndloadComplete;

        public event EventHandler<LogReceivedEventArgs> LogReceived;

        public bool IsBusy { get; private set; }

        private void OnStart()
        {
            IsBusy = true;
            DowndloadStart?.Invoke(this, null);
        }

        private void OnComplete()
        {
            IsBusy = false;
            DowndloadComplete?.Invoke(this, null);
        }

        private void OnLogReceived(string data)
        {
            LogReceived?.Invoke(this, new LogReceivedEventArgs(data));
        }

        public Downloader()
        {
        }

        public void Cancel()
        {
            if (IsBusy)
            {
                NativeMethods.AttachConsole((uint)process.Id);
                NativeMethods.SetConsoleCtrlHandler(null, true);
                NativeMethods.GenerateConsoleCtrlEvent(CtrlTypes.CTRL_C_EVENT, 0);
                process.WaitForExit(2000);
                NativeMethods.FreeConsole();
                NativeMethods.SetConsoleCtrlHandler(null, false);
            }
        }

        public void Download(IConfig config)
        {
            if (config != null)
            {
                if (config.Engine == DownloadEngine.YouGet)
                {
                    var option = new OptionG()
                    {
                        IsDebug = config.IsDebug,
                        IsPlaylist = config.IsPlaylist,
                        OutputDir = config.OutputDir,
                        DownloadUrl = config.DownloadUrl,
                    };

                    if (config.IsProxy && config.ProxyUrl.Scheme == "socks5")
                    {
                        option.SocksProxy = $"{config.ProxyUrl.Host}:{config.ProxyUrl.Port}";
                    }
                    else if (config.IsProxy)
                    {
                        option.HttpProxy = $"{config.ProxyUrl.Host}:{config.ProxyUrl.Port}";
                    }
                    InitializeTask(option);
                }
                else
                {
                    InitializeTask(new OptionY()
                    {
                        IsDebug = config.IsDebug,
                        NoPlaylist = !config.IsPlaylist,
                        DownloadUrl = config.DownloadUrl,
                        Format = config.IsAudioOnly ? AudioFormat : VideoFormat,
                        Proxy = config.IsProxy == true ? config.ProxyUrl : null,
                        OutputTemplate = config.IsPlaylist == true ?
                        Path.Combine(config.OutputDir, PlaylistOutputTemplate) :
                        Path.Combine(config.OutputDir, NoPlaylistOutputTemplate)
                    });
                }
                TaskStart();
            }
        }

        private void InitializeTask<T>(T option)
        {
            process = new Process()
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo()
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    Arguments = Parser.Default.FormatCommandLine(option),
                    FileName = option is OptionG ? "you-get" : "youtube-dl",
                    StandardOutputEncoding = option is OptionG ? Encoding.UTF8 : Encoding.Default
                }
            };
            process.OutputDataReceived += Process_OutputDataReceived;
            process.ErrorDataReceived += Process_ErrorDataReceived;
            process.Exited += Process_Exited;
        }

        private void TaskStart()
        {
            if (process != null)
            {
                OnStart();

                Task.Run(() =>
                {
                    try
                    {
                        process.Start();
                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();
                        process.WaitForExit();
                    }
                    catch (Win32Exception e)
                    {
                        OnLogReceived(e.Message);
                        OnComplete();
                    }
                });
            }
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.Data))
            {
                OnLogReceived(e.Data);
            }
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.Data))
            {
                OnLogReceived(e.Data);
            }
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            process.Dispose();
            OnComplete();
        }

        #region IDisposable Support

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    process?.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}