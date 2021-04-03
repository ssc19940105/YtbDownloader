using CommandLine;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Vanara.PInvoke;
using YtbDownloader.Core.Common;
using YtbDownloader.Core.Interfaces;
using YtbDownloader.Core.Options;

namespace YtbDownloader.Core.Downloaders
{
    public class Downloader : IDownloader
    {
        private Process process;
        private const string AudioFormat = "bestaudio[ext=m4a]/bestaudio";
        private const string VideoFormat = "bestvideo[ext=mp4]+bestaudio[ext=m4a]/best[ext=mp4]/best";
        private const string NoPlaylistOutputTemplate = "%(title)s.%(ext)s";
        private const string PlaylistOutputTemplate = "%(playlist)s/%(title)s.%(ext)s";

        public event EventHandler DownloadStarted;

        public event EventHandler DownloadCompleted;

        public event EventHandler<LogReceivedEventArgs> LogReceived;

        public bool IsBusy { get; private set; }

        private void OnStart()
        {
            IsBusy = true;
            DownloadStarted?.Invoke(this, null);
        }

        private void OnComplete()
        {
            IsBusy = false;
            DownloadCompleted?.Invoke(this, null);
        }

        private void OnLogReceived(string data)
        {
            LogReceived?.Invoke(this, new LogReceivedEventArgs(data));
        }

        public Downloader()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public void Cancel()
        {
            if (IsBusy)
            {
                Task.Run(() =>
                {
                    Kernel32.AttachConsole((uint)process.Id);
                    Kernel32.SetConsoleCtrlHandler(null, true);
                    Kernel32.GenerateConsoleCtrlEvent(Kernel32.CTRL_EVENT.CTRL_C_EVENT, 0);
                    process.WaitForExit(2000);
                    Kernel32.FreeConsole();
                    Kernel32.SetConsoleCtrlHandler(null, false);
                });
            }
        }

        public void Download(IConfig config)
        {
            if (config != null)
            {
                if (config.IsYouGet)
                {
                    InitializeTaskG(config);
                }
                else
                {
                    InitializeTaskY(config);
                }
                TaskStart();
            }
        }

        private void InitializeTaskY(IConfig config)
        {
            InitializeTask(new OptionY()
            {
                IsDebug = config.IsDebug,
                NoPlaylist = !config.IsPlaylist,
                DownloadUrl = config.DownloadUrl,
                Format = config.IsAudioOnly ? AudioFormat : VideoFormat,
                Proxy = config.IsProxy == true ? config.ProxyUrl : null,
                SubLang = config.DownloadSub ? config.SubLang : null,
                OutputTemplate = config.IsPlaylist == true ?
                Path.Combine(config.OutputPath, PlaylistOutputTemplate) :
                Path.Combine(config.OutputPath, NoPlaylistOutputTemplate),
                IgnoreError = config.IgnoreError
            });
        }

        private void InitializeTaskG(IConfig config)
        {
            var option = new OptionG()
            {
                IsDebug = config.IsDebug,
                IsPlaylist = config.IsPlaylist,
                OutputDir = config.OutputPath,
                DownloadUrl = config.DownloadUrl,
            };
            if (config.IsProxy)
            {
                var proxy = new Uri(config.ProxyUrl);
                switch (proxy.Scheme)
                {
                    case "http" or "https":
                        option.HttpProxy = $"{proxy.Host}:{proxy.Port}";
                        break;
                    case "sock4" or "socks5":
                        option.SocksProxy = $"{proxy.Host}:{proxy.Port}";
                        break;
                }
            }
            InitializeTask(option);
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
                    StandardOutputEncoding = option is OptionG ? Encoding.UTF8 : Encoding.GetEncoding((int)Kernel32.GetConsoleCP())
                }
            };
            process.OutputDataReceived += Process_DataReceived;
            process.ErrorDataReceived += Process_DataReceived;
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
                    catch (System.ComponentModel.Win32Exception e)
                    {
                        OnLogReceived(e.Message);
                        OnComplete();
                    }
                });
            }
        }

        private void Process_DataReceived(object sender, DataReceivedEventArgs e)
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
    }
}