using CommandLine;

namespace YtbDownloader.Core.Options
{
    public class OptionG
    {
        [Value(0)]
        public string DownloadUrl { get; set; }

        [Option('o')]
        public string OutputDir { get; set; }

        [Option('x')]
        public string HttpProxy { get; set; }

        [Option("socks-proxy")]
        public string SocksProxy { get; set; }

        [Option('d', "debug")]
        public bool IsDebug { get; set; }

        [Option('l', "playlist")]
        public bool IsPlaylist { get; set; }
    }
}