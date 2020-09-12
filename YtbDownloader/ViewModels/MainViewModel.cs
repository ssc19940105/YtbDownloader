using Anotar.Catel;
using Catel.IoC;
using Catel.MVVM;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Input;
using YtbDownloader.Common;
using YtbDownloader.Core.Common;
using YtbDownloader.Core.Downloaders;
using YtbDownloader.Core.Interfaces;
using YtbDownloader.Models;
using YtbDownloader.Properties;
using YtbDownloader.Validators;

namespace YtbDownloader.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IDownloader downloader;

        private readonly ConfigManger configManger;

        public Config Config { get; }

        public string StartButtonContent { get; private set; }

        public string LogContent { get; private set; }

        public double ProgressValue { get; private set; }

        public ICommand SetOutputDirCommand { get; }

        public ICommand StartCommand { get; }

        public ICommand OpenOutputDirCommand { get; }

        public ICommand ClearLogCommand { get; }

        public ICommand WindowClosingCommand { get; }

        private void OpenOutputDir()
        {
            if (Directory.Exists(Config.OutputDir))
            {
                Process.Start("explorer", Config.OutputDir);
            }
            else
            {
                LogContent += $"{new LogReceivedEventArgs(Resources.CheckOutputDirMessage)}\n";
            }
        }

        private void SetOutputDir()
        {
            using var dialog = new FolderBrowserDialog();
            if (DialogResult.OK == dialog.ShowDialog())
            {
                Config.OutputDir = dialog.SelectedPath;
            }
        }

        private void Start()
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
                MessageBox.Show(Resources.ExitWarningMessage,
                                Resources.WarningCaption,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                e.Cancel = true;
                return;
            }
            configManger.Save(Config);
        }

        public MainViewModel()
        {
            downloader = InitializeDownloader();
            StartButtonContent = Resources.StartBtnHelpText;
            StartCommand = new Command(Start);
            SetOutputDirCommand = new Command(SetOutputDir);
            OpenOutputDirCommand = new Command(OpenOutputDir);
            ClearLogCommand = new Command(() => LogContent = string.Empty);
            WindowClosingCommand = new Command<CancelEventArgs>(WindowClosing);
            configManger = new ConfigManger(Path.Combine(Catel.IO.Path.GetApplicationDataDirectory(), "Config.xml"));
            Config = configManger.Load<Config>();
        }

        private void ConfigManger_LoadFailure(object sender, EventArgs e)
        {
            MessageBox.Show(Resources.LoadConfigFailureMessage,
                            Resources.WarningCaption,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
        }

        private void ConfigManger_SaveFailure(object sender, EventArgs e)
        {
            MessageBox.Show(Resources.SaveConfigFailureMessage,
                            Resources.WarningCaption,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
        }

        private IDownloader InitializeDownloader()
        {
            ServiceLocator.Default.RegisterType<IDownloader, Downloader>();
            var downloader = ServiceLocator.Default.ResolveType<IDownloader>();
            downloader.LogReceived += Downloader_LogReceived;
            downloader.DowndloadStart += Downloader_DowndloadStart;
            downloader.DowndloadComplete += Downloader_DowndloadComplete;
            return downloader;
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

        private void Downloader_DowndloadStart(object sender, EventArgs e)
        {
            StartButtonContent = Resources.StopBtnHelpText;
        }

        private void Downloader_DowndloadComplete(object sender, EventArgs e)
        {
            StartButtonContent = Resources.StartBtnHelpText;
        }
    }
}