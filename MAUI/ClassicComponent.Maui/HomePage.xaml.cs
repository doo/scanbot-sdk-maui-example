using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using SBSDK = ScanbotSDK.MAUI.ScanbotSDK;
namespace ClassicComponent.Maui;

public partial class HomePage : ContentPage
{
	public class SdkFeature
	{

		public string Title { get; private set; }
		public Action ClickAction { get; private set; }


		public SdkFeature(string title, Action clickAction)
		{
			Title = title;
			ClickAction = clickAction;
		}
	}
	
	private List<SdkFeature> _sdkFeatures;

	public List<SdkFeature> SDKFeatures
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
		_sdkFeatures = new List<SdkFeature>
		{
							new SdkFeature("Classic Barcode Scanner", () => Navigation.PushAsync(new MainPage())),
							new SdkFeature("Classic Document Scanner", () => Navigation.PushAsync(new ClassicDocumentScannerPage()))
		};
		this.BindingContext = this;
		InitializeComponent();
	}

	private void SdkFeatureSelected(object sender, SelectionChangedEventArgs e)
	{
		if (!SBSDK.IsLicenseValid)
		{
			return;
		}
		
		if (e?.CurrentSelection != null && e.CurrentSelection.Count > 0 && e.CurrentSelection.First() is SdkFeature feature)
		{
			feature?.ClickAction?.Invoke();
        }

        FeaturesCollectionView.SelectedItem = null;
    }
}
