using ReadyToUseUI.iOS.Models;
using ReadyToUseUI.iOS.Utils;
using ReadyToUseUI.iOS.View;
using ScanbotSDK.iOS;

namespace ReadyToUseUI.iOS.Controller;

public partial class ScannedDocumentsViewController : UIViewController
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
        NavigationItem.SetRightBarButtonItem(new UIBarButtonItem(Texts.export, UIBarButtonItemStyle.Done, OnExportButtonClick), true);
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

    public void DidSelectFilter(SBSDKParametricFilter filter)
    {
        LoadPages();
    }
}