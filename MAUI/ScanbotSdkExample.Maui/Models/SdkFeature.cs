namespace ScanbotSdkExample.Maui.Models;

public readonly struct SdkFeature(string title, Func<Task> action = null)
{
    public string Title { get; } = title;
    public Func<Task> Action { get; } = action;

    public bool IsHeader => Action is null;
    public bool IsVisible => Action is not null;
}