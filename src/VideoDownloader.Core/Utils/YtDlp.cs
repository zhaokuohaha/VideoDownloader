using CliWrap;
using CliWrap.EventStream;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using VideoDownloader.Core.Models;

namespace VideoDownloader.Core.Utils
{
    public partial class YtDlp(string url, string videoFolder, string ytDlpPath, YtDlpOptions? options = null)
    {
        private List<string> BuildBaseArgs()
        {
            var args = new List<string>();
            if (options == null) return args;

            if (!string.IsNullOrWhiteSpace(options.ProxyUrl))
            {
                args.Add("--proxy");
                args.Add(options.ProxyUrl);
            }

            if (!string.IsNullOrWhiteSpace(options.RateLimit))
            {
                args.Add("--rate-limit");
                args.Add(options.RateLimit);
            }

            if (options.ConcurrentFragments > 1)
            {
                args.Add("--concurrent-fragments");
                args.Add(options.ConcurrentFragments.ToString());
            }

            if (options.Retries != 10)
            {
                args.Add("--retries");
                args.Add(options.Retries.ToString());
            }

            if (options.SocketTimeout.HasValue)
            {
                args.Add("--socket-timeout");
                args.Add(options.SocketTimeout.Value.ToString());
            }

            if (options.CookieSourceType == CookieSourceType.Browser)
            {
                args.Add("--cookies-from-browser");
                args.Add(options.CookieBrowserType.ToString().ToLower());
            }
            else if (options.CookieSourceType == CookieSourceType.File
                     && !string.IsNullOrWhiteSpace(options.CookieFilePath))
            {
                args.Add("--cookies");
                args.Add(options.CookieFilePath);
            }

            if (!string.IsNullOrWhiteSpace(options.UserAgent))
            {
                args.Add("--user-agent");
                args.Add(options.UserAgent);
            }

            if (!string.IsNullOrWhiteSpace(options.Referer))
            {
                args.Add("--referer");
                args.Add(options.Referer);
            }

            return args;
        }

        private async Task<string> QueryInternal(IReadOnlyList<string> args)
        {
            var stdOutBuffer = new StringBuilder();
            var stdErrBuffer = new StringBuilder();

            await Cli.Wrap(ytDlpPath)
                .WithArguments(args)
                .WithWorkingDirectory(videoFolder)
                .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer))
                .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer))
                .ExecuteAsync();

            return stdOutBuffer.ToString();
        }

        public async Task<VideoInfo?> GetVideoInfo()
        {
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            };

            var args = new List<string> { "-j", url };
            args.AddRange(BuildBaseArgs());

            var output = await QueryInternal(args);
            try
            {
                var videoInfo = JsonSerializer.Deserialize<VideoInfo>(output, serializeOptions)!;
                videoInfo.Videos = new ObservableCollection<VideoFormat>(videoInfo.Formats.Where(x => x.IsVideo));
                videoInfo.Audios = new ObservableCollection<VideoFormat>(videoInfo.Formats.Where(x => !x.IsVideo));

                return videoInfo;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> DownloadByFormat(string format, Action<double> onProgressChanged)
        {
            var args = new List<string> { "-f", format, url };
            args.AddRange(BuildBaseArgs());

            var cmd = Cli.Wrap(ytDlpPath)
                .WithArguments(args)
                .WithWorkingDirectory(videoFolder);
            await foreach (var cmdEvent in cmd.ListenAsync())
            {
                switch (cmdEvent)
                {
                    case StartedCommandEvent:
                        onProgressChanged?.Invoke(0);
                        break;
                    case StandardOutputCommandEvent stdOut:
                        var text = stdOut.Text;
                        var regex = DownloadProgressRegex().Match(text);
                        if (regex.Success && double.TryParse(regex.Groups[1].Value, out var progress))
                        {
                            onProgressChanged?.Invoke(progress);
                        }
                        break;
                    case StandardErrorCommandEvent:
                        // TODO log
                        break;
                    case ExitedCommandEvent exited:
                        return exited.ExitCode == 0;
                }
            }

            return true;
        }

        [GeneratedRegex(@"(\d+(.\d+)?)%")]
        private static partial Regex DownloadProgressRegex();
    }
}
