using FluentValidation;
using I18NPortable;
using System;
using System.IO;
using System.Text.RegularExpressions;
using YtbDownloader.Core.Interfaces;

namespace YtbDownloader.Validators
{
    public class ConfigValidator : AbstractValidator<IConfig>
    {
        private static II18N Strings => I18N.Current;

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
            RuleFor(x => x.DownloadUrl).Must(IsValidDownloadUrl)
                .WithMessage(Strings["CheckDownloadUrlMessage"]);
            RuleFor(x => x.OutputDir).Must(path => Directory.Exists(path))
                .WithMessage(Strings["CheckOutputDirMessage"]);
            RuleFor(x => x.ProxyUrl).Must(IsValidProxyUrl).When(x => x.IsProxy)
                .WithMessage(Strings["CheckProxyUrlMessage"]);
            RuleFor(x => x.SubLangs).Must(x => !string.IsNullOrWhiteSpace(x)).When(x => x.IsDownloadSubs && !x.IsYouGet)
                .WithMessage(Strings["CheckSubLangsUrlMessage"]);
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