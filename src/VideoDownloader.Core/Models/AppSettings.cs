namespace VideoDownloader.Core.Models
{
    public class AppSettings
    {
        public string? DownloadFolderPath { get; set; }
        public string? ProxyUrl { get; set; }
        public string ThemeMode { get; set; } = "System";
    }
}
