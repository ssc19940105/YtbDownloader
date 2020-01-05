using Ninject;
using Prism.Commands;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Input;
using YtbDownloader.Core.Common;
using YtbDownloader.Core.Downloaders;
using YtbDownloader.Core.Interfaces;
using YtbDownloader.Models;
using YtbDownloader.Validators;

namespace YtbDownloader.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private IDownloader downloader;

        public IConfig Config { get; }

        public string StartButtonContent { get; private set; } = "开始";

        public string LogContent { get; private set; }

        public double ProgressValue { get; private set; }

        public ICommand SetOutputDirCommand { get; }

        public ICommand StartCommand { get; }

        public ICommand OpenOutputDirCommand { get; }

        public ICommand ClearLogCommand { get; }

        public ICommand SaveLogCommand { get; }

        public ICommand WindowClosingCommand { get; }

        private void OpenOutputDir()
        {
            if (Directory.Exists(Config.OutputDir))
            {
                Process.Start(Config.OutputDir);
            }
            else
            {
                LogContent += $"{new LogReceivedEventArgs(Properties.Resources.CheckOutputDir)}\n";
            }
        }

        private void SaveLog()
        {
            using (var dialog = new SaveFileDialog()
            {
                FileName = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss", CultureInfo.CurrentCulture),
                Filter = Properties.Resources.SaveLogDialogFilter,
                Title = Properties.Resources.SaveLogDialogTitle
            })
            {
                if (DialogResult.OK == dialog.ShowDialog())
                {
                    using (var file = File.Create(dialog.FileName))
                    {
                        if (!string.IsNullOrWhiteSpace(LogContent))
                        {
                            var content = Encoding.Default.GetBytes(LogContent);
                            file.Write(content, 0, content.Length);
                        }
                    }
                    MessageBox.Show(Properties.Resources.SaveLogCompletionMessage,
                                    Properties.Resources.SaveLogCompletionCaption,
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
            }
        }

        private void SetOutputDir()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (DialogResult.OK == dialog.ShowDialog())
                {
                    Config.OutputDir = dialog.SelectedPath;
                }
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
            if (downloader?.IsBusy == true && e != null &&
                DialogResult.Yes != MessageBox.Show(Properties.Resources.ExitWarning,
                                                    Properties.Resources.ExitWarningCaption,
                                                    MessageBoxButtons.YesNo,
                                                    MessageBoxIcon.Warning))
            {
                e.Cancel = true;
            }
        }

        public MainViewModel()
        {
            Config = new Config();
            InitializeDownloader();
            StartCommand = new DelegateCommand(Start);
            SaveLogCommand = new DelegateCommand(SaveLog);
            SetOutputDirCommand = new DelegateCommand(SetOutputDir);
            OpenOutputDirCommand = new DelegateCommand(OpenOutputDir);
            ClearLogCommand = new DelegateCommand(() => LogContent = string.Empty);
            WindowClosingCommand = new DelegateCommand<CancelEventArgs>(WindowClosing);
        }

        private void InitializeDownloader()
        {
            using (var kernel = new StandardKernel())
            {
                kernel.Bind<IDownloader>().To<Downloader>();
                downloader = kernel.Get<IDownloader>();
                downloader.LogReceived += Downloader_LogReceived;
                downloader.DowndloadStart += Downloader_DowndloadStart;
                downloader.DowndloadComplete += Downloader_DowndloadComplete;
            }
        }

        private void Downloader_LogReceived(object sender, LogReceivedEventArgs e)
        {
            var match = Regex.Match(e.EventMessage, @"\d+\.?\d+%");
            if (match.Success)
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
            StartButtonContent = "停止";
        }

        private void Downloader_DowndloadComplete(object sender, EventArgs e)
        {
            StartButtonContent = "开始";
        }
    }
}