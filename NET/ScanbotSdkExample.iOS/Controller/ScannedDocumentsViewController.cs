using ScanbotSdkExample.iOS.Models;
using ScanbotSdkExample.iOS.Utils;
using ScanbotSdkExample.iOS.View;
using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Controller;

public partial class ScannedDocumentsViewController : UIViewController
{
    ImageCollectionView ContentView { get; set; }

    private SBSDKScannedDocument _scannedDocument;

    internal void NavigateData(string documentId)
    {
        _scannedDocument = SBSDKScannedDocument.LoadDocumentWithDocumentUuid(documentId, out var error);
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
            new UIBarButtonItem(Texts.Filter, UIBarButtonItemStyle.Done, OnFilterButtonClicked)
        };

        if (_scannedDocument.Pages.Length == 1) // Single page document
        {
            toolBarButtons.AddRange(new List<UIBarButtonItem>
            {
                new UIBarButtonItem(Texts.DocumentQuality, UIBarButtonItemStyle.Done, OnAnalyzeDocumentClicked),
                new UIBarButtonItem(Texts.Crop, UIBarButtonItemStyle.Done, OnManualCropClicked)
            });
        }

        SetToolbarItems(toolBarButtons.ToArray(), true);
        NavigationController?.SetToolbarHidden(false, false);
        NavigationItem.SetRightBarButtonItem(new UIBarButtonItem(Texts.Export, UIBarButtonItemStyle.Done, OnExportButtonClick), true);
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
        var documentAnalyzer = new SBSDKDocumentQualityAnalyzer(new SBSDKDocumentQualityAnalyzerConfiguration(), out var initializationError);

        // Get the document quality analysis result by passing the image to the analyzer
        var documentQuality = documentAnalyzer.RunWithImage(documentPageImage, out var scanningError);
        Alert.Show(this, "Document Quality", Map(documentQuality?.Quality));
    }

    private void OnManualCropClicked(object sender, EventArgs e)
    {
        var configuration = new SBSDKUI2CroppingConfiguration(documentUuid: _scannedDocument.Uuid, pageUuid: _scannedDocument.PageUuids.First());

        // e.g. configure various colors.
        configuration.Appearance.TopBarBackgroundColor = new SBSDKUI2Color(UIColor.Red);
        configuration.Cropping.TopBarConfirmButton.Foreground.Color = new SBSDKUI2Color(UIColor.White);
        
        // e.g. customize a UI element's text
        configuration.Localization.CroppingTopBarCancelButtonTitle = "Cancel";

        SBSDKUI2CroppingViewController.PresentOn(this, configuration, completion: CroppingFinished);
    }

    private void CroppingFinished(SBSDKUI2CroppingResult result)
    {
        LoadPages();
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
        controller.NavigateData(_ => LoadPages(), _scannedDocument);
        NavigationController?.PushViewController(controller, true);
    }
    
    // Map document quality analysis result into string
    private string Map(SBSDKDocumentQuality documentQuality)
    {
        if (documentQuality == null) return "No Document";
        
        if (SBSDKDocumentQuality.VeryPoor.Equals(documentQuality))
            return "Very Poor";

        if (SBSDKDocumentQuality.Poor.Equals(documentQuality))
            return "Poor";

        if (SBSDKDocumentQuality.Reasonable.Equals(documentQuality))
            return "Reasonable";

        if (SBSDKDocumentQuality.Good.Equals(documentQuality))
            return "Good";

        if (SBSDKDocumentQuality.Excellent.Equals(documentQuality))
            return "Excellent";

        return "No Document";
    }
}