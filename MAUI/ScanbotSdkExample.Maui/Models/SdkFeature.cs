namespace ScanbotSdkExample.Maui.Models;

public struct SdkFeature
{
    public SdkFeature(string title, Func<Task> doTask = null)
    {
        Title = title;
        DoTask = doTask;
    }

    public string Title { get; private set; }
    public Func<Task> DoTask { get; private set; }

    public bool ShowHeading => DoTask == null;
    public bool ShowFeature => DoTask != null;
}