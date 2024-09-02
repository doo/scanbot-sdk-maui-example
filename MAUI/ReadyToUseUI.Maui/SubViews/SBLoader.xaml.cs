using System.ComponentModel;
using System.Diagnostics;

namespace ReadyToUseUI.Maui.SubViews;

public partial class SBLoader : ContentView
{
    private const string LoaderText = "•";
    private const int TextLimit = 5;
    
    public SBLoader()
    {
        InitializeComponent();
    }

    private bool _isBusy;
    
    /// <summary>
    /// Can be configured with Xaml Binding 
    /// </summary>
    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            _isBusy = value;
            this.IsVisible = value;
            IsRunning();
        }
    }
    
    private async void IsRunning()
    {
        var index = TextLimit;
        var text = string.Empty;
        do
        {
            if (index == TextLimit)
            {
                text = LoaderText;
                index = 1;
            }

            await MainThread.InvokeOnMainThreadAsync(() => sbActivityIndicator.Text = text);
            await Task.Delay(200);
            text += LoaderText;
            index++;

        } while (IsBusy);

        sbActivityIndicator.Text = string.Empty;
    }
}