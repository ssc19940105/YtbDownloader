using FluentValidation;
using System;
using System.IO;
using System.Text.RegularExpressions;
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
            RuleFor(x => x.DownloadUrl).Must(IsValidDownloadUrl).WithMessage(Properties.Resources.CheckDownloadUrl);
            RuleFor(x => x.OutputDir).Must(path => Directory.Exists(path)).WithMessage(Properties.Resources.CheckOutputDir);
            RuleFor(x => x.ProxyUrl).Must(IsValidProxyUrl).When(x => x.IsProxy).WithMessage(Properties.Resources.CheckProxyUrl);
        }

        private static bool IsValidProxyUrl(Uri url)
        {
            return url != null ? Regex.IsMatch(url.OriginalString, @"^(http(s{0,1})|socks5)://([\w-]+\.)+[\w-]+:\d+(/[\w-./?%&=]*)?$") : false;
        }

        private static bool IsValidDownloadUrl(Uri url)
        {
            return url != null ? Regex.IsMatch(url.OriginalString, @"^http(s{0,1})://([\w-]+\.)+[\w-]+(/[\w-./?%&=]*)?$") : false;
        }
    }
}