using VideoDownloader.Core.Services;

namespace VideoDownloader.Android.Services;

public class AndroidDialogService : IDialogService
{
    public Task ShowAlertAsync(string title, string content, string closeButtonText)
    {
        return Task.CompletedTask;
    }

    public Task<string?> PickFolderAsync(string title)
    {
        return Task.FromResult<string?>(null);
    }
}
