using Catel.Data;
using Catel.MVVM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using YtbDownloader.Core.Interfaces;
using YtbDownloader.Properties;

namespace YtbDownloader.Models
{
    public class Config : ViewModelBase, IConfig
    {
        public bool IsProxy { get; set; }

        public Uri ProxyUrl { get; set; }

        public Uri DownloadUrl { get; set; }

        public string OutputDir { get; set; }

        public bool IsAudioOnly { get; set; }

        public bool IsPlaylist { get; set; }

        public bool IsDebug { get; set; }

        public bool IsYouGet { get; set; }

        public bool DownloadSub { get; set; }

        public string SubLang { get; set; }

        public bool IgnoreError { get; set; }

        protected override void ValidateFields(List<IFieldValidationResult> validationResults)
        {
            if (!IsValidDownloadUrl(DownloadUrl))
            {
                validationResults?.Add(FieldValidationResult.CreateError(nameof(DownloadUrl), Resources.CheckDownloadUrlMessage));
            }

            if (!Directory.Exists(OutputDir))
            {
                validationResults?.Add(FieldValidationResult.CreateError(nameof(OutputDir), Resources.CheckOutputDirMessage));
            }

            if (IsProxy && !IsValidProxyUrl(ProxyUrl))
            {
                validationResults?.Add(FieldValidationResult.CreateError(nameof(ProxyUrl), Resources.CheckProxyUrlMessage));
            }

            if (!IsYouGet && DownloadSub && string.IsNullOrWhiteSpace(SubLang))
            {
                validationResults?.Add(FieldValidationResult.CreateError(nameof(SubLang), Resources.CheckSubLangsUrlMessage));
            }
        }

        private static bool IsValidProxyUrl(Uri url)
        {
            return url != null && Regex.IsMatch(url.OriginalString, @"^(http(s?)|socks\d)://([\w-]+\.)+[\w-]+:\d+(/[\w-./?%&=]*)?$");
        }

        private static bool IsValidDownloadUrl(Uri url)
        {
            return url != null && Regex.IsMatch(url.OriginalString, @"^http(s?)://([\w-]+\.)+[\w-]+(/[\w-./?%&:=]*)?$");
        }
    }
}