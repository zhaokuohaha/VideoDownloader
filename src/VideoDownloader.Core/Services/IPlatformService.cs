namespace VideoDownloader.Core.Services
{
    public interface IPlatformService
    {
        void OpenFolder(string path);
        string GetDefaultDownloadFolder();
        string GetYtDlpPath();
        string GetFfmpegPath();
    }
}
