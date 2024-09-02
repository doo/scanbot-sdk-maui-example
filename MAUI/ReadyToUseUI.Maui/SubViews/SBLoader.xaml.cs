using System.ComponentModel;
using System.Diagnostics;

namespace ReadyToUseUI.Maui.SubViews;

public partial class SBLoader : ContentView
{
    public SBLoader()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty IsBusyProperty =
        BindableProperty.Create ("IsBusy", typeof(bool), typeof(SBLoader), false, BindingMode.Default, propertyChanged: ChangedProp);

    private static void ChangedProp(BindableObject bindable, object oldvalue, object newvalue)
    {
        Debug.WriteLine("Test");
    }

    /// <summary>
    /// Can be configured with Xaml Binding 
    /// </summary>
    public bool IsBusy
    {
        get => (bool)GetValue(IsBusyProperty);
        set => SetValue(IsBusyProperty, value);
    }

    // Use this function when there is no Xaml Binding.  
    internal void UpdateLoading(bool loading)
    {
        IsBusy = loading;
        IsVisible = loading;
        sbActivityIndicator.IsRunning = loading;
    }
}