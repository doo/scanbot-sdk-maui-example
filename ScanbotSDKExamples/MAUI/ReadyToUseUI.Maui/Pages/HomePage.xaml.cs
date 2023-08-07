using ReadyToUseUI.Maui.Models;
using ReadyToUseUI.Maui.Utils;
using ReadyToUseUI.Maui.ViewModels;

namespace ReadyToUseUI.Maui.Pages;

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
    async void CollectionViewSDKServices_ItemSelected(System.Object sender, Microsoft.Maui.Controls.SelectionChangedEventArgs e)
    {
        if (e?.CurrentSelection?.FirstOrDefault() is SDKService service && service != null)
        {
            if (!SDKUtils.CheckLicense(CurrentPage)) { return; }
            await ViewModel.InvokeSDKService(service.Title);
        }
        CollectionViewSDKServices.SelectedItem = null;
    }
}
