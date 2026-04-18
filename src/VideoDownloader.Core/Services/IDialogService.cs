namespace VideoDownloader.Core.Services
{
    public interface IDialogService
    {
        Task ShowAlertAsync(string title, string content, string closeButtonText);
    }
}
