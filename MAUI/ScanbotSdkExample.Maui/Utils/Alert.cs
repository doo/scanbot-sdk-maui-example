namespace ScanbotSdkExample.Maui.Utils;

public static class Alert
{
    public static async void Show(string title, string message)
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await App.Navigation.CurrentPage.DisplayAlertAsync(title, message, "Close");
        });
    }
        
    public static async void Show(string title, string message, Action completion)
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            var result = await App.Navigation.CurrentPage.DisplayAlertAsync(title, message, "View Detected Image", "Close");
            if (result)
            {
                completion?.Invoke();
            }
        });
    }
}