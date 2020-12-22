using Anotar.Catel;
using Catel.IoC;
using Catel.MVVM;
using I18NPortable;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Input;
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

        public Config Config { get; }

        public static II18N Strings => I18N.Current;

        public StringBuilder LogContent { get; }

        public string StartButtonContent { get; private set; }

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
                PrintLog($"{new LogReceivedEventArgs("CheckOutputDirMessage".Translate())}\n");
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
                        PrintLog($"{new LogReceivedEventArgs(failure.ErrorMessage)}\n");
                    }
                }
            }
        }

        private void WindowClosing(CancelEventArgs e)
        {
            if (downloader?.IsBusy == true)
            {
                MessageBox.Show("ExitWarningMessage".Translate(),
                                "WarningCaption".Translate(),
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
            LogContent = new StringBuilder();
            StartButtonContent = "StartBtnHelpText".Translate();
            StartCommand = new Command(Start);
            ClearLogCommand = new Command(ClearLog);
            SetOutputDirCommand = new Command(SetOutputDir);
            OpenOutputDirCommand = new Command(OpenOutputDir);
            WindowClosingCommand = new Command<CancelEventArgs>(WindowClosing);
            configManger = new ConfigManger(Path.Combine(Catel.IO.Path.GetApplicationDataDirectory(), "Config.xml"));
            Config = configManger.Load<Config>();
        }

        private void ConfigManger_LoadFailure(object sender, EventArgs e)
        {
            MessageBox.Show("LoadConfigFailureMessage".Translate(),
                            "WarningCaption".Translate(),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
        }

        private void ConfigManger_SaveFailure(object sender, EventArgs e)
        {
            MessageBox.Show("SaveConfigFailureMessage".Translate(),
                            "WarningCaption".Translate(),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
        }

        private IDownloader InitializeDownloader()
        {
            ServiceLocator.Default.RegisterType<IDownloader, Downloader>();
            var downloader = ServiceLocator.Default.ResolveType<IDownloader>();
            downloader.DowndloadStart += (sender, e) => StartButtonContent = "StopBtnHelpText".Translate();
            downloader.DowndloadComplete += (sender, e) => StartButtonContent = "StartBtnHelpText".Translate();
            downloader.LogReceived += Downloader_LogReceived;
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
                PrintLog($"{e}\n");
            }
        }

        private void PrintLog(string message)
        {
            LogContent.Append(message);
            RaisePropertyChanged(this, nameof(LogContent));
        }

        private void ClearLog()
        {
            LogContent.Clear();
            RaisePropertyChanged(this, nameof(LogContent));
        }
    }
}