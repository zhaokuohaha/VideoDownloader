using VideoDownloader.Core.Services;

namespace VideoDownloader.Android.Services;

public class AndroidPlatformService : IPlatformService
{
    public void OpenFolder(string path)
    {
        // Android uses intent to open file manager - placeholder for MVP
    }

    public string GetDefaultDownloadFolder()
    {
        var downloads = global::Android.OS.Environment.GetExternalStoragePublicDirectory(
            global::Android.OS.Environment.DirectoryDownloads)?.AbsolutePath;
        return downloads ?? "/storage/emulated/0/Download/VideoDownloader";
    }

    public string GetYtDlpPath()
    {
        // yt-dlp binary path on Android - will be bundled or downloaded at runtime
        return System.IO.Path.Combine(
            global::Android.App.Application.Context.FilesDir?.AbsolutePath ?? "",
            "yt-dlp");
    }

    public string GetFfmpegPath()
    {
        return System.IO.Path.Combine(
            global::Android.App.Application.Context.FilesDir?.AbsolutePath ?? "",
            "ffmpeg");
    }
}
