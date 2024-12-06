using Android.Graphics;
using Android.OS;
using AndroidX.AppCompat.App;
using IO.Scanbot.Sdk.Core;
using IO.Scanbot.Sdk.Core.Processor;
using IO.Scanbot.Sdk.Imagefilters;

namespace ReadyToUseUI.Droid.Snippets;

public class PageFilterSnippet : AppCompatActivity
{
	private IO.Scanbot.Sdk.ScanbotSDK _scanbotSdk;

	protected override void OnCreate(Bundle savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
		
		// Returns the singleton instance of the Sdk.
		_scanbotSdk = new IO.Scanbot.Sdk.ScanbotSDK(this);
		
		if (_scanbotSdk.LicenseInfo.IsValid)
		{
			StartFiltering();
		}
	}

	private void StartFiltering()
	{
		var document = _scanbotSdk.DocumentApi.LoadDocument("Your_save_doc_Id");

		var binarizationFilter = new ScanbotBinarizationFilter(OutputMode.Antialiased);
		var brightnessFilter = new BrightnessFilter(0.4);
		var imageRotation = ImageRotation.Clockwise90;
		
		foreach (var documentPage in document.Pages)
		{
			documentPage.Apply(imageRotation, documentPage.Polygon,  new List<ParametricFilter> {binarizationFilter, brightnessFilter});
		}
	}
}