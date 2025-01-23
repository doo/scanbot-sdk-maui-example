using ClassicComponent.Maui.Models;
using ClassicComponent.Maui.Utils;
using SBSDK = ScanbotSDK.MAUI.ScanbotSDKMain;
namespace ClassicComponent.Maui;

public partial class HomePage : ContentPage
{
	private List<ClassicCollectionItem> _sdkFeatures;

	public List<ClassicCollectionItem> SDKFeatures
	{
		get => _sdkFeatures;
		set
		{
			_sdkFeatures = value;
			OnPropertyChanged(nameof(SDKFeatures));
		}
	}

	public HomePage()
	{
		_sdkFeatures = new List<ClassicCollectionItem>
		{
							new ("Classic Barcode Scanner", () => Navigation.PushAsync(new ClassicBarcodeScannerPage())),
							new ("Classic Barcode Scanner (Custom Implementation)", () => Navigation.PushAsync(new CustomBarcodeScannerPage())),
							new ("Classic Document Scanner", () => Navigation.PushAsync(new ClassicDocumentScannerPage()))
		};
		this.BindingContext = this;
		InitializeComponent();
	}

	private async void SdkFeatureSelected(object sender, SelectionChangedEventArgs e)
	{
		if (!SBSDK.IsLicenseValid)
		{
			await DisplayAlert("Error", "Your SDK license has expired", "Close");
			return;
		}
			
		var permissionGranted = await Validation.CheckAndRequestCameraPermission();
		if (permissionGranted != PermissionStatus.Granted)
		{
			return;
		}
		
		if (e?.CurrentSelection != null && e.CurrentSelection.Count > 0 && e.CurrentSelection.First() is ClassicCollectionItem collectionItem)
		{
			collectionItem?.ClickAction?.Invoke();
        }

        FeaturesCollectionView.SelectedItem = null;
    }
}
