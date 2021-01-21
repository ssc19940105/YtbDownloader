using Anotar.Catel;
using Catel.IoC;
using Catel.MVVM;
using I18NPortable;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Shell;
using YtbDownloader.Common;
using YtbDownloader.Core.Common;
using YtbDownloader.Core.Downloaders;
using YtbDownloader.Core.Interfaces;
using YtbDownloader.Models;
using YtbDownloader.Validators;

namespace YtbDownloader.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IDownloader downloader;

        private readonly ConfigManger configManger;

        public IConfig Config { get; }

        public static II18N Strings => I18N.Current;

        public string LogContent { get; private set; }

        public string DownloadBtnContent { get; private set; }

        public TaskbarItemProgressState ProgressState { get; private set; }

        public double ProgressValue { get; private set; }

        public ICommand SetOutputPathCommand { get; }

        public ICommand DownloadCommand { get; }

        public ICommand OpenOutputPathCommand { get; }

        public ICommand ClearLogCommand { get; }

        public ICommand WindowClosingCommand { get; }

        private void OpenOutputPath()
        {
            if (Directory.Exists(Config.OutputPath))
            {
                Process.Start("explorer", Config.OutputPath);
            }
            else
            {
                LogContent += $"{new LogReceivedEventArgs("CheckOutputDirMessage".Translate())}\n";
            }
        }

        private void SetOutputPath()
        {
            using var dialog = new FolderBrowserDialog();
            if (DialogResult.OK == dialog.ShowDialog())
            {
                Config.OutputPath = dialog.SelectedPath;
            }
        }

        private void Download()
        {
            if (downloader.IsBusy)
            {
                downloader.Cancel();
            }
            else
            {
                var validation = ConfigValidator.Instance.Validate(Config);
                if (validation.IsValid)
                {
                    downloader.Download(Config);
                }
                else
                {
                    foreach (var failure in validation.Errors)
                    {
                        LogContent += $"{new LogReceivedEventArgs(failure.ErrorMessage)}\n";
                    }
                }
            }
        }

        private void WindowClosing(CancelEventArgs e)
        {
            if (downloader?.IsBusy == true)
            {
                if (MessageBox.Show("ExitWarningMessage".Translate(),
                                "WarningCaption".Translate(),
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    downloader.Cancel();
                }
                else
                {
                    e.Cancel = true;
                }
            }
            configManger.Save(Config);
        }

        public MainViewModel()
        {
            DownloadBtnContent = "StartBtnHelpText".Translate();
            DownloadCommand = new Command(Download);
            SetOutputPathCommand = new Command(SetOutputPath);
            OpenOutputPathCommand = new Command(OpenOutputPath);
            ClearLogCommand = new Command(() => LogContent = string.Empty);
            WindowClosingCommand = new Command<CancelEventArgs>(WindowClosing);
            configManger = new ConfigManger(Path.Combine(Catel.IO.Path.GetApplicationDataDirectory(), "Config.xml"));
            Config = configManger.Load<Config>();
            ServiceLocator.Default.RegisterType<IDownloader, Downloader>();
            downloader = ServiceLocator.Default.ResolveType<IDownloader>();
            downloader.LogReceived += Downloader_LogReceived;
            downloader.DownloadStarted += Downloader_DownloadStarted;
            downloader.DownloadCompleted += Downloader_DownloadCompleted;
        }

        private void Downloader_LogReceived(object sender, LogReceivedEventArgs e)
        {
            LogTo.Info(e.EventMessage);
            var match = Regex.Match(e.EventMessage, @"\s*\d+\.?\d+%");
            if (match.Success && (e.EventMessage.StartsWith("[download]", StringComparison.CurrentCulture)
                || e.EventMessage.StartsWith(match.Value, StringComparison.CurrentCulture)))
            {
                ProgressValue = double.Parse(match.Value.Split('%')[0], CultureInfo.CurrentCulture);
            }
            else
            {
                LogContent += $"{e}\n";
            }
        }

        private void Downloader_DownloadStarted(object sender, EventArgs e)
        {
            DownloadBtnContent = "StopBtnHelpText".Translate();
            ProgressState = TaskbarItemProgressState.Normal;
        }

        private void Downloader_DownloadCompleted(object sender, EventArgs e)
        {
            DownloadBtnContent = "StartBtnHelpText".Translate();
            ProgressState = TaskbarItemProgressState.None;
        }
    }
}