using ReadyToUseUI.iOS.Repository;
using ReadyToUseUI.iOS.Utils;
using ReadyToUseUI.iOS.View;
using ReadyToUseUI.iOS.View.Collection;
using ReadyToUseUI.iOS.Models;
using ScanbotSDK.iOS;

namespace ReadyToUseUI.iOS.Controller
{
    public class ImageListController : UIViewController
    {
        public ImageCollectionView ContentView { get; private set; }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ContentView = new ImageCollectionView();
            View = ContentView;

            Title = "Scanned documents";

            LoadPages();

            var saveButton = new UIBarButtonItem(Texts.save, UIBarButtonItemStyle.Done, OnSaveButtonClick);
            NavigationItem.SetRightBarButtonItem(saveButton, false);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            LoadPages();

            ContentView.Collection.Selected += OnImageSelected;
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            ContentView.Collection.Selected -= OnImageSelected;
        }

        void LoadPages()
        {
            ContentView.Collection.Pages.Clear();
            ContentView.Collection.Pages.AddRange(PageRepository.Items);
            ContentView.Collection.ReloadData();
        }

        void OnImageSelected(object sender, CollectionEventArgs e)
        {
            PageRepository.Current = e.Page;

            var controller = new ProcessingController();
            NavigationController.PushViewController(controller, true);
        }

        private void OnSaveButtonClick(object sender, EventArgs e)
        {
            if (PageRepository.DocumentImageURLs.Length == 0)
                return;

            var input = PageRepository.DocumentImageURLs;
            var docs = NSSearchPathDirectory.DocumentDirectory;
            var url = NSFileManager.DefaultManager.GetUrls(docs, NSSearchPathDomain.User)[0];
            var alertController = UIAlertController.Create(Texts.save, Texts.SaveHow, UIAlertControllerStyle.ActionSheet);
            if (!ScanbotSDKGlobal.IsLicenseValid)
            {
                var title = "Oops";
                var body = "Your license has expired";
                Alert.Show(this, title, body);
                return;
            }

            var pdf = CreateButton(Texts.save_without_ocr, (action) => _ = CreatePDFAsync(input, url));
            var ocr = CreateButton(Texts.save_with_ocr, (action) => _ = PerformOCRAndCreatePDFAsync(input, url));
            var tiff = CreateButton(Texts.Tiff, (action) => WriteTIFF(input, url));
            var cancel = CreateButton("Cancel", delegate { }, UIAlertActionStyle.Cancel);

            alertController.AddAction(pdf);
            alertController.AddAction(ocr);
            alertController.AddAction(tiff);
            alertController.AddAction(cancel);

            UIPopoverPresentationController presentationPopover = alertController.PopoverPresentationController;
            if (presentationPopover != null)
            {
                presentationPopover.SourceView = View;
                presentationPopover.PermittedArrowDirections = UIPopoverArrowDirection.Up;
            }

            PresentViewController(alertController, true, null);
        }

        private async Task CreatePDFAsync(NSUrl[] inputUrls, NSUrl outputUrl)
        {
            try
            {
                var outputPdfUrl = await DocumentUtilities.CreatePDFAsync(inputUrls, outputUrl, SBSDKPDFRendererPageSize.A4, SBSDKPDFRendererPageOrientation.Auto, ScanbotSDKUI.DefaultImageStoreEncrypter);
                if (outputPdfUrl != null)
                {
                    OpenDocument(outputPdfUrl, false);
                }
                else
                {
                    ShowErrorAlert();
                }
            }
            catch (Exception ex)
            {
                Alert.Show(this, "Create PDF", ex.Message);
            }
        }

        private async Task PerformOCRAndCreatePDFAsync(NSUrl[] inputUrls, NSUrl outputUrl)
        {
            var recognitionMode = SBSDKOpticalCharacterRecognitionMode.Ml;
            // This is the new OCR configuration with ML which doesn't require the languages.
            SBSDKOpticalCharacterRecognizerConfiguration ocrConfiguration = SBSDKOpticalCharacterRecognizerConfiguration.MlConfiguration;

            // to use legacy configuration we have to pass the installed languages.
            if (recognitionMode == SBSDKOpticalCharacterRecognitionMode.Legacy)
            {
                var installedLanguages = SBSDKResourcesManager.InstalledLanguages;
                ocrConfiguration = SBSDKOpticalCharacterRecognizerConfiguration.LegacyConfigurationWithLanguages(installedLanguages);
            }

            SBSDKOpticalCharacterRecognizer recognizer = new SBSDKOpticalCharacterRecognizer(ocrConfiguration);

            try
            {
                // Please check the default parameters
                var (ocrResult, outputPdfUrl) = await DocumentUtilities.PerformOCRAsync(ocrRecognizer: recognizer, inputUrls: inputUrls, outputUrl: outputUrl, shouldGeneratePdf: true, encrypter: ScanbotSDKUI.DefaultImageStoreEncrypter);
                if (ocrResult != null)
                {
                    OpenDocument(outputPdfUrl, true, ocrResult.RecognizedText);
                }
                else
                {
                    ShowErrorAlert();
                }
            }
            catch (Exception ex)
            {
                Alert.Show(this, "Perform OCR", ex.Message);
            }
        }

        private void WriteTIFF(NSUrl[] inputUrls, NSUrl outputUrl)
        {
            // Please note that some compression types are only compatible for 1-bit encoded images (binarized black & white images)!
            var options = SBSDKTIFFImageWriterParameters.DefaultParametersForBinaryImages();
            options.Binarize = true;
            options.Compression = SBSDKTIFFImageWriterCompressionOptions.Ccittfax4;
            options.Dpi = 250;

            var (success, outputTiffUrl) = DocumentUtilities.CreateTIFF(options, inputUrls, outputUrl, ScanbotSDKUI.DefaultImageStoreEncrypter);
            if (success)
            {
                var title = "Write TIFF";
                var body = "TIFF file saved to: " + outputTiffUrl;
                Alert.Show(this, title, body);
            }
            else
            {
                ShowErrorAlert();
            }
        }

        void OpenDocument(NSUrl uri, bool ocr, string ocrResult = null)
        {
            InvokeOnMainThread(() =>
            {
                var controller = new PdfViewController(uri, ocr, ocrResult);
                NavigationController.PushViewController(controller, true);
            });
        }

        UIAlertAction CreateButton(string text, Action<UIAlertAction> action,
            UIAlertActionStyle style = UIAlertActionStyle.Default)
        {
            return UIAlertAction.Create(text, style, action);
        }

        private void ShowErrorAlert()
        {
            var title = "Oops!";
            var body = "Something went wrong with saving your file. Please try again";
            Alert.Show(this, title, body);
        }
    }
}
