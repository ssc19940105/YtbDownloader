using Anotar.Serilog;
using I18NPortable;
using Ninject;
using Prism.Commands;
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
using YtbDownloader.Validators;

namespace YtbDownloader.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private IDownloader downloader;

        private readonly ConfigManger configManger;

#pragma warning disable CS0067

        public event PropertyChangedEventHandler PropertyChanged;

#pragma warning restore CS0067

        public IConfig Config { get; }

        public static II18N Strings => I18N.Current;

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
                LogContent += $"{new LogReceivedEventArgs(Strings["CheckOutputDirMessage"])}\n";
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
                var result = ConfigValidator.Instance.Validate(Config);
                if (result.IsValid)
                {
                    downloader.Download(Config);
                }
                else
                {
                    foreach (var failure in result.Errors)
                    {
                        LogContent += $"{new LogReceivedEventArgs(failure.ErrorMessage)}\n";
                    }
                }
            }
        }

        private void WindowClosing(CancelEventArgs e)
        {
            if (downloader?.IsBusy == true &&
                DialogResult.Yes != MessageBox.Show(Strings["ExitWarningMessage"],
                                                    Strings["ExitWarningCaption"],
                                                    MessageBoxButtons.YesNo,
                                                    MessageBoxIcon.Warning))
            {
                e.Cancel = true;
                return;
            }
            configManger.SaveConfig(Config);
        }

        public MainViewModel()
        {
            InitializeDownloader();
            StartButtonContent = Strings["StartBtnHelpText"];
            configManger = new ConfigManger("Config.json");
            configManger.LoadFailure += ConfigManger_LoadFailure;
            configManger.SaveFailure += ConfigManger_SaveFailure;
            Config = configManger.LoadConfig<Config>();
            StartCommand = new DelegateCommand(Start);
            SetOutputDirCommand = new DelegateCommand(SetOutputDir);
            OpenOutputDirCommand = new DelegateCommand(OpenOutputDir);
            ClearLogCommand = new DelegateCommand(() => LogContent = string.Empty);
            WindowClosingCommand = new DelegateCommand<CancelEventArgs>(WindowClosing);
        }

        private void ConfigManger_LoadFailure(object sender, EventArgs e)
        {
            MessageBox.Show(Strings["LoadConfigFailureMessage"],
                            Strings["WarningBoxTitle"],
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
        }

        private void ConfigManger_SaveFailure(object sender, EventArgs e)
        {
            MessageBox.Show(Strings["SaveConfigFailureMessage"],
                            Strings["WarningBoxTitle"],
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
        }

        private void InitializeDownloader()
        {
            using var kernel = new StandardKernel();
            kernel.Bind<IDownloader>().To<Downloader>();
            downloader = kernel.Get<IDownloader>();
            downloader.LogReceived += Downloader_LogReceived;
            downloader.DowndloadStart += Downloader_DowndloadStart;
            downloader.DowndloadComplete += Downloader_DowndloadComplete;
        }

        private void Downloader_LogReceived(object sender, LogReceivedEventArgs e)
        {
            LogTo.Information(e.EventMessage);
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
            StartButtonContent = Strings["StopBtnHelpText"];
        }

        private void Downloader_DowndloadComplete(object sender, EventArgs e)
        {
            StartButtonContent = Strings["StartBtnHelpText"];
        }
    }
}