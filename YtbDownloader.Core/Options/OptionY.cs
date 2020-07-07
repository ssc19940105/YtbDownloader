using CommandLine;
using System;

namespace YtbDownloader.Core.Options
{
    public class OptionY
    {
        [Value(0)]
        public Uri DownloadUrl { get; set; }

        [Option('v', "verbose")]
        public bool IsDebug { get; set; }

        [Option("proxy")]
        public Uri Proxy { get; set; }

        [Option('f', "format")]
        public string Format { get; set; }

        [Option('o', "output")]
        public string OutputTemplate { get; set; }

        [Option("no-playlist")]
        public bool NoPlaylist { get; set; }

        [Option("write-auto-sub")]
        public bool WriteAutoSub { get => !string.IsNullOrWhiteSpace(SubLang); }

        [Option("sub-lang")]
        public string SubLang { get; set; }

        [Option('i', "ignore-errors")]
        public bool IgnoreErrors { get; set; }
    }
}