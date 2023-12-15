using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using VideoDownloader.Models;
using CliWrap;
using System.IO;

namespace VideoDownloader.Utils
{
    public class YtDlp(string url)
    {
        private readonly string ExeFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "yt-dlp.exe");
        private readonly string Workdir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Videos");
        private string _getVideoTitle => $"-e {url}";
        private string _getVideoThumbnail => $"--list-thumbnails {url}";
        private string _listFormats => $"-F {url}";

        public YtDlp(string url, string workdir) : this(url)
        {
            if (!string.IsNullOrEmpty(workdir))
            {
                Workdir = workdir;
            }

            if (!Directory.Exists(Workdir))
            {
                Directory.CreateDirectory(Workdir);
            }
        }

        private async Task<string> QueryInternal(string arg)
        {
            var stdOutBuffer = new StringBuilder();
            var stdErrBuffer = new StringBuilder();

            var result = await Cli.Wrap(ExeFile)
                .WithArguments(arg)
                .WithWorkingDirectory(Workdir)
                // This can be simplified with `ExecuteBufferedAsync()`
                .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer))
                .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer))
                .ExecuteAsync();

            // Access stdout & stderr buffered in-memory as strings
            var stdOut = stdOutBuffer.ToString();
            var stdErr = stdErrBuffer.ToString();

            return stdOut;

            //var process =new Process
            //{
            //    StartInfo = new ProcessStartInfo
            //    {
            //        FileName = ExeFile,
            //        Arguments = arg,
            //        UseShellExecute = false,
            //        WindowStyle = ProcessWindowStyle.Normal,
            //    }
            //};
            //process.Start();

            //var output = process.StandardOutput.ReadToEnd();
            //return output;
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
            var titleLine = "ID EXT RESOLUTION";
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
                            FileSize = rowData[3]
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
    }
}
