using CliWrap;
using CliWrap.EventStream;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using VideoDownloader.Core.Models;

namespace VideoDownloader.Core.Utils
{
    public partial class YtDlp(string url, string videoFolder, string ytDlpPath)
    {
        private string _getVideoTitle => $"-j \"{url}\"";

        private Func<string, string> _downloadByFormat => (formatId) => $"-f \"{formatId}\" \"{url}\"";

        private async Task<string> QueryInternal(string arg)
        {
            var stdOutBuffer = new StringBuilder();
            var stdErrBuffer = new StringBuilder();

            var result = await Cli.Wrap(ytDlpPath)
                .WithArguments(arg)
                .WithWorkingDirectory(videoFolder)
                .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer))
                .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer))
                .ExecuteAsync();

            var stdOut = stdOutBuffer.ToString();

            return stdOut;
        }

        public async Task<VideoInfo?> GetVideoInfo()
        {
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            };
            var output = await QueryInternal(_getVideoTitle);
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
            var commandText = _downloadByFormat(format);
            var cmd = Cli.Wrap(ytDlpPath)
                .WithArguments(commandText)
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
