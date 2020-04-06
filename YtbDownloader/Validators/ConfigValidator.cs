using FluentValidation;
using System;
using System.IO;
using System.Text.RegularExpressions;
using YtbDownloader.Common;
using YtbDownloader.Core.Interfaces;

namespace YtbDownloader.Validators
{
#pragma warning disable CA1710

    public class ConfigValidator : AbstractValidator<IConfig>
    {
#pragma warning restore CA1710

        private static ConfigValidator instance = null;
        private static readonly object _lock = new object();

        public static ConfigValidator Instance
        {
            get
            {
                lock (_lock)
                {
                    if (instance == null)
                    {
                        instance = new ConfigValidator();
                    }
                    return instance;
                }
            }
        }

        private ConfigValidator()
        {
            RuleFor(x => x.DownloadUrl).Must(IsValidDownloadUrl).WithMessage(ResourceHelper.FindResource("CheckDownloadUrlMessage"));
            RuleFor(x => x.OutputDir).Must(path => Directory.Exists(path)).WithMessage(ResourceHelper.FindResource("CheckOutputDirMessage"));
            RuleFor(x => x.ProxyUrl).Must(IsValidProxyUrl).When(x => x.IsProxy).WithMessage(ResourceHelper.FindResource("CheckProxyUrlMessage"));
            RuleFor(x => x.SubLangs).Must(x => !string.IsNullOrWhiteSpace(x)).When(x => x.IsDownloadSubs).WithMessage(ResourceHelper.FindResource("CheckSubLangsUrlMessage"));
        }

        private static bool IsValidProxyUrl(Uri url)
        {
            return url != null ? Regex.IsMatch(url.OriginalString, @"^(http(s?)|socks\d)://([\w-]+\.)+[\w-]+:\d+(/[\w-./?%&=]*)?$") : false;
        }

        private static bool IsValidDownloadUrl(Uri url)
        {
            return url != null ? Regex.IsMatch(url.OriginalString, @"^http(s?)://([\w-]+\.)+[\w-]+(/[\w-./?%&:=]*)?$") : false;
        }
    }
}