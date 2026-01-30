namespace ScanbotSdkExample.Maui.Utils;

public static class Alert
{
    /// <summary>
    /// Static function for alert. Task Awaitable.
    /// </summary>
    /// <param name="title">Title string.</param>
    /// <param name="message">Message string.</param>
    /// <returns>Returns a task object.</returns>
    public static async Task ShowAsync(string title, string message)
    {
        await MainThread.InvokeOnMainThreadAsync(async () => await App.Navigation.CurrentPage.DisplayAlertAsync(title, message, "Ok"));
    }

    /// <summary>
    /// Static function for alert with completion handler. Task Awaitable.
    /// </summary>
    /// <param name="title">Title string.</param>
    /// <param name="message">Message string.</param>
    /// <param name="accept">Accept string.</param>
    /// <param name="reject">Reject string. Optional defaults to "Close".</param>
    /// <returns>Returns a task object.</returns>
    public static async Task<bool> ShowAsync(string title, string message, string accept, string reject = "Close")
    {
        return await MainThread.InvokeOnMainThreadAsync(async () => await App.Navigation.CurrentPage.DisplayAlertAsync(title, message, accept, reject));
    }

    /// <summary>
    /// Static function for alert. Task Awaitable.
    /// </summary>
    /// <param name="exception">Exception Object.</param>
    /// <returns>Returns a task object.</returns>
    public static async Task ShowAsync(Exception exception)
    {
        await MainThread.InvokeOnMainThreadAsync(async () => await App.Navigation.CurrentPage.DisplayAlertAsync("Alert", exception.Message, "Close"));
    }
}