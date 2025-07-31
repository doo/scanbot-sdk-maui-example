namespace ScanbotSdkExample.Maui.Utils;
public static class Alert
{
    public static void Show(string title, string message)
    {
        _ = MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await App.RootPage.DisplayAlert(title, message, "Close");
        });
    }
        
    public static async void Show(string title, string message, Action completion)
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            var result =  await App.RootPage.DisplayAlert(title, message, "View Detected Image", "Close");
            if (result)
            {
                completion?.Invoke();
            }
        });
    }
}