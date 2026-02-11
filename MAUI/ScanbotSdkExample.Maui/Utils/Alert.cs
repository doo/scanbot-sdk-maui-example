namespace ScanbotSdkExample.Maui.Utils;

public static class Alert
{
    /// <summary>
    /// Static function for alert. Task Awaitable.
    /// </summary>
    /// <param name="title">Title string.</param>
    /// <param name="message">Message string.</param>
    /// <param name="button">Button title string. Optional, defaults to Ok.</param>
    /// <returns>Returns a task object.</returns>
    public static async Task ShowAsync(string title, string message, string button = "Ok")
    {
        await MainThread.InvokeOnMainThreadAsync(async () => await App.Navigation.CurrentPage.DisplayAlertAsync(title, message, button));
    }

    /// <summary>
    /// Static function for alert with completion handler. Task Awaitable.
    /// </summary>
    /// <param name="title">Title string.</param>
    /// <param name="message">Message string.</param>
    /// <param name="accept">Accept string.</param>
    /// <param name="reject">Reject string. Optional defaults to "Close".</param>
    /// <returns>Returns a task object.</returns>
    public static async Task<bool> ShowAsync(string title, string message, string accept, string reject)
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
        await ShowAsync("Alert", exception?.Message ?? "Something went wrong.", "Close");
    }
}