using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace VideoDownloader.Core.Models
{
    public class VideoFormat : ObservableObject
    {
        public bool IsSelected { get; set; }

        public string? Format { get; set; }
        public string? FormatId { get; set; }
        public string? Ext { get; set; }
        public string? Resolution { get; set; }
        public double? FilesizeApprox { get; set; }
        public double? Filesize { get; set; }
        public double? Duration { get; set; }
        public string FileSizeStr => CalcFileSize();

        // 判断如果无 VideoExt 表示纯音频否则均为视频
        public string? VideoExt { get; set; }

        public bool IsVideo => VideoExt != default && VideoExt != "none";

        public int HeightPixels
        {
            get
            {
                if (string.IsNullOrEmpty(Resolution)) return 0;
                var parts = Resolution.Split('x');
                if (parts.Length == 2 && int.TryParse(parts[1], out var h)) return h;
                return 0;
            }
        }

        public string QualityLabel => HeightPixels > 0 ? $"{HeightPixels}p" : Resolution ?? "未知";

        private string CalcFileSize()
        {
            if (Filesize == default && FilesizeApprox == default)
            {
                return "未知大小";
            }

            var fileSize = Filesize == default ? FilesizeApprox : Filesize;
            return $"{Math.Round(fileSize!.Value / 1048576, 1)}MB";
        }
    }

    public class VideoInfo : VideoFormat
    {
        public string? Title { get; set; }
        public string? Thumbnail { get; set; }
        public ObservableCollection<VideoFormat> Formats { get; set; } = [];
        public ObservableCollection<VideoFormat> Videos { get; set; } = [];
        public ObservableCollection<VideoFormat> Audios { get; set; } = [];
        public bool AnyAudio => Audios.Any();
    }
}
