using CliWrap;
using CliWrap.EventStream;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using VideoDownloader.Models;
using Wpf.Ui.Controls;

namespace VideoDownloader.Utils
{
    public class YtDlp(string url, string videoFolder)
    {
        private readonly string ExeFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "yt-dlp.exe");
        
        private string _getVideoTitle => $"-e {url}";
        private string _getVideoThumbnail => $"--list-thumbnails {url}";
        private string _listFormats => $"-F {url}";
        private Func<string,string> _downloadByFormat => (formatId) => $"-f {formatId} {url}";

        private async Task<string> QueryInternal(string arg)
        {
            var stdOutBuffer = new StringBuilder();
            var stdErrBuffer = new StringBuilder();

            var result = await Cli.Wrap(ExeFile)
                .WithArguments(arg)
                .WithWorkingDirectory(videoFolder)
                // This can be simplified with `ExecuteBufferedAsync()`
                .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer))
                .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer))
                .ExecuteAsync();

            var stdOut = stdOutBuffer.ToString();
            //var stdErr = stdErrBuffer.ToString();

            return stdOut;
        }

        public async Task<string> GetVideoTitle()
        {
            var output = await QueryInternal(_getVideoTitle);
            return output;
        }

        public async Task<string> GetVideoThumbnail()
        {
            var output = await QueryInternal(_getVideoThumbnail);
            var titleLine = "ID Width   Height  URL";
            var isDataLine = false;
            var url = string.Empty;
            foreach (var line in output.Split('\n'))
            {
                if (isDataLine)
                {
                    url = line.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).LastOrDefault();
                    break;
                }

                if (line.Contains(titleLine))
                {
                    isDataLine = true;
                }
            }

            return url;
        }

        public async Task<List<VideoFormat>> GetVideoFormats()
        {
            var output = await QueryInternal(_listFormats);
            var titleLine = "RESOLUTION";
            var data = new List<VideoFormat>();

            var isDataLine = false;
            foreach (var line in output.Split('\n'))
            {
                if (isDataLine)
                {
                    var rowData = line.Split([' ', '|'])
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Select(x => x.Trim())
                        .ToList();

                    if (rowData.Count > 4)
                    {
                        data.Add(new VideoFormat
                        {
                            Id = rowData[0],
                            Ext = rowData[1],
                            Resolution = rowData[2],
                            FileSize = rowData.FirstOrDefault(x => x.EndsWith("MiB")) ?? "未知"
                        });
                    }
                }

                if (line.Contains(titleLine))
                {
                    isDataLine = true;
                }
            }

            return data;
        }

        internal async Task<bool> DownloadByFormat(VideoFormat format, Action<double> onProgressChanged)
        {
            var stdOutBuffer = new StringBuilder();
            var stdErrBuffer = new StringBuilder();

            var cmd = Cli.Wrap(ExeFile)
                .WithArguments(_downloadByFormat(format.Id))
                .WithWorkingDirectory(videoFolder);
            var index = 0;
            await foreach (var cmdEvent in cmd.ListenAsync())
            {
                switch (cmdEvent)
                {
                    case StartedCommandEvent started:
                        onProgressChanged?.Invoke(0);
                        break;
                    case StandardOutputCommandEvent stdOut:
                        var text = stdOut.Text;
                        var regx = Regex.Match(text, @"(\d+(.\d+)?)%");
                        if (regx.Success && double.TryParse(regx.Groups[1].Value, out var progress))
                        {
                            onProgressChanged?.Invoke(progress);
                        }
                        break;
                    case StandardErrorCommandEvent stdErr:
                        // TODO log
                        break;
                    case ExitedCommandEvent exited:
                        return exited.ExitCode == 0;
                }
            }

            return true;
        }
    }
}
