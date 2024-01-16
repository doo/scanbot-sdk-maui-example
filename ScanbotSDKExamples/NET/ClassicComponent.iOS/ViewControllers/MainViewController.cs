using System.Diagnostics;
using ClassicComponent.iOS.Models;
using ClassicComponent.iOS.Utils;
using ClassicComponent.iOS.ViewControllers;
using Scanbot.ImagePicker.iOS;
using ScanbotSDK.iOS;

namespace ClassicComponent.iOS
{
    internal interface ISDKServiceSource
    {
        List<SDKService> SDKServices { get; set; }
        void RowSelected(int index);
    }

    interface IModifyDocumentControllerDelegate
    {
        public void DidUpdateDocumentImage(SBSDKPolygon polygon = null, nint? rotation = null, SBSDKImageFilterType? filter = null);
    }

    internal interface ICameraDemoViewControllerDelegate
    {
        void DidCaptureDocumentImage(UIImage documentImage, UIImage originalImage, SBSDKPolygon polygon);
    }

    public partial class MainViewController : UIViewController, ISDKServiceSource, ICameraDemoViewControllerDelegate, IModifyDocumentControllerDelegate
    {
        public List<SDKService> SDKServices { get; set; }
        public ProgressHUD ProgressIndicator { get; private set; }

        private ImageProcessingParameters imageParameters;

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                ProgressIndicator?.ToggleLoading(value);
            }
        }

        private UIImage originalDocumentImage, editedDocumentImage;

        public MainViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.NavigationItem.Title = Texts.AppTitle;
            ProgressIndicator = ProgressHUD.Load(View.Frame);
            imageParameters = new ImageProcessingParameters();
            LoadFeatures();
            SetUpTableView();
            UpdateHintLabel(string.Empty);
            UpdateDocumentImageView(null);
            SetTapGestureToDocumentImage();
        }

        private void LoadFeatures()
        {
            SDKServices = new List<SDKService>
            {
                new SDKService { Title = SDKServiceTitle.ScanningUI, ServiceAction = LaunchScanningUI },
                new SDKService { Title = SDKServiceTitle.CroppingUI, ServiceAction = LaunchCroppingUI },
                new SDKService { Title = SDKServiceTitle.ImportImageFromLibrary, ServiceAction = () => _ = LaunchImportImageFromLibrary() },
                new SDKService { Title = SDKServiceTitle.ApplyImageFilter, ServiceAction = ApplyImageFilter },
                new SDKService { Title = SDKServiceTitle.CreateTIFF, ServiceAction = CreateTIFF },
                new SDKService { Title = SDKServiceTitle.CreatePDF, ServiceAction = CreatePDF },
                new SDKService { Title = SDKServiceTitle.PerformOCR, ServiceAction = PerformOCR },
                new SDKService { Title = SDKServiceTitle.CheckRecognizer, ServiceAction = () => NavigationController.PushViewController(new CheckRecognizerDemoViewController(), true)},
                new SDKService { Title = SDKServiceTitle.BarcodeScanAndCount, ServiceAction = LaunchBarcodeScanAndCount },
                new SDKService { Title = SDKServiceTitle.VINScanner, ServiceAction = LaunchVINScanner },
            };
        }

        private void UpdateDocumentImageView(UIImage updatedImage)
        {
            if (updatedImage == null)
            {
                imageViewVisibleConstraint.Active = false;
                imageViewHiddenConstraint.Active = true;
            }
            else
            {
                imageViewDocument.Image = updatedImage;
                imageViewHiddenConstraint.Active = false;
                imageViewVisibleConstraint.Active = true;
                UpdateHintLabel(string.Empty);
            }
        }

        private void UpdateHintLabel(string message)
        {
            if (message == null)
                message = string.Empty;

            if (message.Length == 0 && ScanbotSDKGlobal.LicenseStatus == dooLicenseStatus.Trial)
            {
                message = Texts.TrialLicenseHint;
            }

            if (message.Equals(Texts.LicenseExpired))
            {
                lblHint.TextColor = UIColor.Red;
            }
            else
            {
                lblHint.TextColor = UIColor.SystemGray;
            }

            lblHint.Text = message;
        }

        private void SetUpTableView()
        {
            tableViewSDKFeatures.Source = new SDKServiceSource(this);
            tableViewSDKFeatures.TableFooterView = new UIView();
        }

        private void SetTapGestureToDocumentImage()
        {
            imageViewDocument.UserInteractionEnabled = true;
            imageViewDocument.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                var viewController = new ViewFullScreenDocumentViewController(editedDocumentImage);
                NavigationController.PresentViewController(viewController, true, null);
            }));
        }

        bool CheckScanbotSDKLicense()
        {
            if (ScanbotSDKGlobal.IsLicenseValid)
            {
                // Trial period, valid trial license or valid production license.
                return true;
            }

            UpdateHintLabel(message: Texts.LicenseExpired);
            return false;
        }

        public void RowSelected(int index)
        {
            if (!CheckScanbotSDKLicense()) { return; }

            if (index < 0 && index > SDKServices.Count)
            {
                return;
            }

            SDKServices[index].ServiceAction?.Invoke();
        }

        private void LaunchBarcodeScanAndCount()
        {
            var viewController = Utilities.GetViewController<BarcodeScanAndCountViewController>(Texts.ClassicComponentStoryboard);
            NavigationController.PushViewController(viewController, true);
        }

        private void LaunchVINScanner()
        {
            var viewController = Utilities.GetViewController<VINScannerViewController>(Texts.ClassicComponentStoryboard);
            NavigationController.PushViewController(viewController, true);
        }

        #region Document Scanning UI

        private void LaunchScanningUI()
        {
            var cameraViewController = new CameraDemoViewController { cameraViewControllerDelegate = this };
            NavigationController.PushViewController(cameraViewController, true);
        }

        public void DidCaptureDocumentImage(UIImage documentImage, UIImage originalImage, SBSDKPolygon polygon)
        {
            this.imageParameters = new ImageProcessingParameters { Polygon = polygon };
            originalDocumentImage = originalImage;
            editedDocumentImage = documentImage;
            UpdateDocumentImageView(documentImage);
            
            DocumentUtilities.GetTemporaryStorage().AddImage(originalDocumentImage);
        }

        #endregion

        private void LaunchCroppingUI()
        {
            if (!CheckScanbotSDKLicense()) { return; }
            if (!CheckOriginalImageUrl()) { return; }

            UINavigationController viewController = CroppingDemoController.GetViewController(originalDocumentImage, imageParameters, this);
            NavigationController.PresentViewController(viewController, true, null);
        }

        private bool CheckOriginalImageUrl()
        {
            if (originalDocumentImage == null)
            {
                UpdateHintLabel("Please snap a document image via Scanning UI or run Document Detection on an image file from the PhotoLibrary");
                tableViewSDKFeatures.ScrollsToTop = true;
                return false;
            }
            return true;
        }

        private async Task LaunchImportImageFromLibrary()
        {
            var originalImage = await ImagePicker.Instance.PickImageAsync();
            if (originalImage != null)
            {
                Debug.WriteLine("Got the original image from gallery");
                IsBusy = true;
                var detectionResult = await Task.Run(() =>
                {
                    // The SDK call is sync!
                    SBSDKDocumentDetector detector = new SBSDKDocumentDetector();
                    return detector.DetectDocumentPolygonOnImage(originalImage, new CGRect(CGPoint.Empty, originalImage.Size), false, false);
                });

                if (detectionResult.Status == SBSDKDocumentDetectionStatus.Ok ||
                    detectionResult.Status == SBSDKDocumentDetectionStatus.OK_SmallSize)
                {
                    var documentImage = originalImage;

                    if (detectionResult.Polygon != null && originalImage != null)
                    {
                        documentImage = originalImage.ImageWarpedByPolygon(detectionResult.Polygon, imageScale: 1.0f);
                    }

                    Debug.WriteLine("Detection result image: " + documentImage);

                    DidCaptureDocumentImage(documentImage, originalImage, detectionResult.Polygon);
                }
                else
                {
                    Debug.WriteLine("No Document detected! DetectionStatus = " + detectionResult.Status);
                    UpdateHintLabel("No Document detected! DetectionStatus = " + detectionResult.Status);
                }
                IsBusy = false;
            }
        }

        public void DidUpdateDocumentImage(SBSDKPolygon polygon = null, nint? rotation = null, SBSDKImageFilterType? filter = null)
        {
            if (rotation != null)
            {
                imageParameters.Rotation = (int)rotation.Value;
            }

            if (polygon != null)
            {
                imageParameters.Polygon = polygon;
            }

            if (filter != null)
            {
                imageParameters.Filter = filter.Value;
            }

            editedDocumentImage = Utilities.GetProcessedImage(ref originalDocumentImage, imageParameters);
            UpdateDocumentImageView(editedDocumentImage);
        }

        #region PDF, OCR and TIFF Processing

        private void CreatePDF()
        {
            if (!CheckScanbotSDKLicense()) { return; }
            if (!CheckOriginalImageUrl()) { return; }

            Task.Run(async () =>
            {
                IsBusy = true;
                Debug.WriteLine("Creating PDF file ...");
                var urls = DocumentUtilities.GetTemporaryStorage().ImageURLs;
                var result = await DocumentUtilities.CreatePDFAsync(encrypter: ScanbotSDKUI.DefaultImageStoreEncrypter);
                IsBusy = false;
                Utilities.ShowMessage("PDF file created", "" + result.AbsoluteString);
            });
        }

        private void PerformOCR()
        {
            if (!CheckScanbotSDKLicense()) { return; }
            if (!CheckOriginalImageUrl()) { return; }
            Task.Run(async () =>
            {
                IsBusy = true;
                var recognitionMode = SBSDKOpticalCharacterRecognitionMode.Ml;
                // This is the new OCR configuration with ML which doesn't require the langauges.
                SBSDKOpticalCharacterRecognizerConfiguration ocrConfiguration = SBSDKOpticalCharacterRecognizerConfiguration.MlConfiguration;

                // to use legacy configuration we have to pass the installed languages.
                if (recognitionMode == SBSDKOpticalCharacterRecognitionMode.Legacy)
                {
                    var installedLanguages = SBSDKResourcesManager.InstalledLanguages;
                    ocrConfiguration = SBSDKOpticalCharacterRecognizerConfiguration.LegacyConfigurationWithLanguages(installedLanguages);
                }

                SBSDKOpticalCharacterRecognizer recognizer = new SBSDKOpticalCharacterRecognizer(ocrConfiguration);
                var urls = DocumentUtilities.GetTemporaryStorage().ImageURLs;

                try
                {
                    // Please check the default parameters
                    var (ocrResult, outputPdfUrl) = await DocumentUtilities.PerformOCRAsync(ocrRecognizer: recognizer, shouldGeneratePdf: true, encrypter: ScanbotSDKUI.DefaultImageStoreEncrypter);
                    IsBusy = false;
                    if (ocrResult != null)
                    {
                        Utilities.ShowMessage("OCR Text", ocrResult.RecognizedText);
                    }
                }
                catch (Exception ex)
                {
                    IsBusy = false;
                    Utilities.ShowMessage("OCR Text", ex.Message);
                }
            });
        }

        private void CreateTIFF()
        {
            if (!CheckScanbotSDKLicense()) { return; }
            if (!CheckOriginalImageUrl()) { return; }
            IsBusy = true;
            Debug.WriteLine("Creating TIFF file ...");
            var urls = DocumentUtilities.GetTemporaryStorage().ImageURLs;
            var options = SBSDKTIFFImageWriterParameters.DefaultParametersForBinaryImages();
            options.Binarize = true;
            options.Compression = SBSDKTIFFImageWriterCompressionOptions.Ccittfax4;
            options.Dpi = 250;

            var (success, outputTiffUrl) = DocumentUtilities.CreateTIFF(options, inputUrls: urls, ScanbotSDKUI.DefaultImageStoreEncrypter);
            if (success)
            {
                Utilities.ShowMessage("TIFF file created", "" + outputTiffUrl.AbsoluteString);
            }

            IsBusy = false;
        }

        #endregion

        private void ApplyImageFilter()
        {
            if (!CheckScanbotSDKLicense()) { return; }
            if (!CheckOriginalImageUrl()) { return; }

            UIAlertController actionSheetAlert = UIAlertController.Create("Select filter type", "", UIAlertControllerStyle.ActionSheet);

            foreach (var filter in Enum.GetValues<SBSDKImageFilterType>())
            {
                if (filter.ToString().ToLower() == "none") { continue; }
                actionSheetAlert.AddAction(UIAlertAction.Create(filter.ToString(), UIAlertActionStyle.Default, (action) =>
                {
                    DidUpdateDocumentImage(filter: filter);
                }));
            }

            actionSheetAlert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
            PresentViewController(actionSheetAlert, true, null);
        }
    }

    internal class SDKServiceSource : UITableViewSource
    {
        private ISDKServiceSource sourceDelegate;

        public SDKServiceSource(ISDKServiceSource sourceDelegate)
        {
            this.sourceDelegate = sourceDelegate;
        }

        public override nint RowsInSection(UITableView tableView, nint section) => sourceDelegate.SDKServices.Count;

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = new UITableViewCell(UITableViewCellStyle.Default, "cell");
            var title = sourceDelegate.SDKServices[indexPath.Row].Title;
            Utilities.ConfigureDefaultCell(cell, title);
            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            sourceDelegate?.RowSelected(indexPath.Row);
        }
    }
}
