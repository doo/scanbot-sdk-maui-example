using DocumentSDK.MAUI.Example.Models;
using DocumentSDK.MAUI.Example.Utils;
using DocumentSDK.MAUI.Example.ViewModels;

namespace DocumentSDK.MAUI.Example.Pages;

public interface IPageInteraction
{
    ContentPage CurrentPage { get; }
}

public partial class HomePage : ContentPage, IPageInteraction
{
    private HomePageViewModel ViewModel { get; set; }
    public HomePage()
    {
        ViewModel = new HomePageViewModel(this);
        this.BindingContext = ViewModel;
        InitializeComponent();
    }

    /// Returns the lazy/runtime instance of the current page to the HomeViewModel.
    public ContentPage CurrentPage => this;

    /// Item Selected method invoked on the ListView item selection.
    async void ListViewSDKServices_ItemSelected(Object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is SDKService service)
        {
            if (!SDKUtils.CheckLicense(CurrentPage)) { return; }
            await ViewModel.InvokeSDKService(service.Title);
        }
        ListViewSDKServices.SelectedItem = null;
    }
}
