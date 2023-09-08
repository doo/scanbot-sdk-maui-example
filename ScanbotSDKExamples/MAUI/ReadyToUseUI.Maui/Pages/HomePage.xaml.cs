using ReadyToUseUI.Maui.Models;
using ReadyToUseUI.Maui.Utils;
using ReadyToUseUI.Maui.ViewModels;

namespace ReadyToUseUI.Maui.Pages;

public partial class HomePage : ContentPage
{
    private HomePageViewModel ViewModel { get; set; }
    public HomePage()
    {
        ViewModel = new HomePageViewModel(this.Navigation);
        this.BindingContext = ViewModel;
        InitializeComponent();
    }

    /// Item Selected method invoked on the ListView item selection.
    async void CollectionViewSDKServices_ItemSelected(System.Object sender, Microsoft.Maui.Controls.SelectionChangedEventArgs e)
    {
        if (e?.CurrentSelection?.FirstOrDefault() is SDKService service && service != null)
        {
            if (!SDKUtils.CheckLicense(this)) { return; }
            await ViewModel.InvokeSDKService(service.Title);
        }
        CollectionViewSDKServices.SelectedItem = null;
    }
}
