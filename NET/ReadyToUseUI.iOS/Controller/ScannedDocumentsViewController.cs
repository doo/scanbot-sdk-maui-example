using ReadyToUseUI.iOS.Models;
using ReadyToUseUI.iOS.Utils;
using ReadyToUseUI.iOS.View;
using ScanbotSDK.iOS;

namespace ReadyToUseUI.iOS.Controller;

public partial class ScannedDocumentsViewController : UIViewController, IFilterControllerDelegate
{
	public ImageCollectionView ContentView { get; private set; }

	private SBSDKScannedDocument _scannedDocument;

	internal void NavigateData(string documentId)
	{
		_scannedDocument = new SBSDKScannedDocument(documentId);
	}

	public override void ViewDidLoad()
	{
		base.ViewDidLoad();

		ContentView = new ImageCollectionView();
		View = ContentView;

		Title = "Scanned documents";

		LoadPages();

		var toolBarButtons = new List<UIBarButtonItem>
		{
			new UIBarButtonItem(Texts.filter, UIBarButtonItemStyle.Done, OnFilterButtonClicked)
		};

		if (_scannedDocument.Pages.Length == 1) // Single page document
		{
			toolBarButtons.AddRange(new List<UIBarButtonItem>
			{
				new UIBarButtonItem(Texts.document_quality, UIBarButtonItemStyle.Done, OnAnalyzeDocumentClicked),
				new UIBarButtonItem(Texts.crop, UIBarButtonItemStyle.Done, OnManualCropClicked)
			});
		}

		SetToolbarItems(toolBarButtons.ToArray(), true);
		NavigationController?.SetToolbarHidden(false, false);
		NavigationController?.NavigationItem.SetRightBarButtonItem(new UIBarButtonItem(Texts.export, UIBarButtonItemStyle.Done, OnExportButtonClick), true);
	}

	private void OnAnalyzeDocumentClicked(object sender, EventArgs e)
	{
		// Get the cropped image
		var documentPageImage = _scannedDocument.PageAt(0)?.DocumentImage;
		if (documentPageImage == null)
		{
			return;
		}
        
		// Initialize document quality analyzer
		var documentAnalyzer = new SBSDKDocumentQualityAnalyzer();
        
		// Get the document quality analysis result by passing the image to the analyzer
		var documentQuality = documentAnalyzer.AnalyzeOnImage(documentPageImage);
        
		Utils.Alert.Show(this, "Document Quality", documentQuality.ToString());
	}

	private void OnManualCropClicked(object sender, EventArgs e)
	{
		var configuration = new SBSDKUI2CroppingConfiguration(documentUuid: _scannedDocument.Uuid, pageUuid: _scannedDocument.PageUuids.First() );
        
		// Set the colors
		// e.g
		configuration.Palette.SbColorPrimary = new SBSDKUI2Color(uiColor: Colors.ScanbotRed);
		configuration.Palette.SbColorOnPrimary = new SBSDKUI2Color(uiColor: Colors.NearWhite);
        
		// Configure the screen
		// e.g
		configuration.Cropping.TopBarTitle.Text = "Cropping Screen";
		configuration.Cropping.BottomBar.ResetButton.Visible = true;
		configuration.Cropping.BottomBar.RotateButton.Visible = true;
		configuration.Cropping.BottomBar.DetectButton.Visible = true;

		var controller = new SBSDKUI2CroppingViewController();
		controller.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
		PresentViewController(controller, false, null);
	}

	void LoadPages()
	{
		ContentView.Collection.Pages.Clear();
		ContentView.Collection.Pages.AddRange(_scannedDocument.Pages);
		ContentView.Collection.ReloadData();
	}

	void OpenDocument(NSUrl uri, bool ocr, string ocrResult = null)
	{
		InvokeOnMainThread(() => 
		{
			var controller = new PdfViewController(uri, ocr, ocrResult);
			NavigationController?.PushViewController(controller, true);
		});
	}

	UIAlertAction CreateButton(string text, Action<UIAlertAction> action, UIAlertActionStyle style = UIAlertActionStyle.Default)
	{
		return UIAlertAction.Create(text, style, action);
	}

	private void ShowErrorAlert()
	{
		var title = "Oops!";
		var body = "Something went wrong with saving your file. Please try again";
		Alert.Show(this, title, body);
	}

	private void OnFilterButtonClicked(object sender, EventArgs e)
	{
		var controller = new FilterController();
		controller.NavigateData(this, _scannedDocument);
		NavigationController?.PushViewController(controller, true);
	}

	public void DidSelectFilter(SBSDKParametricFilter filter)
	{
		LoadPages();
	}
}