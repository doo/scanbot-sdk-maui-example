using MobileCoreServices;
using ScanbotSDK.iOS;
using ClassicComponent.iOS.ViewControllers;
using System.Diagnostics;

namespace ClassicComponent.iOS
{
    public partial class MainSelectionTableViewController : UITableViewController
    {
        private CameraDemoDelegateHandler cameraHandler = new CameraDemoDelegateHandler();
        private CroppingDemoDelegateHandler croppingHandler = new CroppingDemoDelegateHandler();

        private GenericDocumentRecognizerDelegate gdrDelegate;

        private UIImagePickerController imagePicker;

        private UIImage documentImage, originalImage;

        public MainSelectionTableViewController(IntPtr handle) : base(handle)
        {
            gdrDelegate = new GenericDocumentRecognizerDelegate(this);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            copyrightLabel.Text = "Copyright (c) " + DateTime.Now.Year.ToString() + " doo GmbH. All rights reserved.";
        }

        partial void ApplyImageFilterTouchUpInside(UIButton sender)
        {
            if (!CheckScanbotSDKLicense()) { return; }
            if (!CheckDocumentImageUrl()) { return; }

            UIAlertController actionSheetAlert = UIAlertController.Create("Select filter type", "", UIAlertControllerStyle.ActionSheet);

            foreach (var filter in Enum.GetValues<SBSDKImageFilterType>())
            {
                if (filter.ToString().ToLower() == "none") { continue; }
                actionSheetAlert.AddAction(UIAlertAction.Create(filter.ToString(), UIAlertActionStyle.Default, (action) =>
                {
                    ApplyFilterOnDocumentImage(filter);
                }));
            }

            actionSheetAlert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
            PresentViewController(actionSheetAlert, true, null);
        }

        protected void ApplyFilterOnDocumentImage(SBSDKImageFilterType filter)
        {
            Task.Run(() =>
            {
                // The SDK call is sync!
                var resultImage = documentImage.ImageFilteredByFilter(filter);
                Debug.WriteLine("Image filter result: " + resultImage);
                ShowImageView(resultImage);
            });
        }

        partial void PerformOCRUpInside(UIButton sender)
        {
            if (!CheckScanbotSDKLicense()) { return; }
            if (!CheckDocumentImageUrl()) { return; }

            Task.Run(() =>
            {

                Debug.WriteLine("Performing OCR ...");

                var images = SBSDKUIPageFileStorage.DefaultStorage.AllPageFileIDs;
                //var result = SBSDK.PerformOCR(images, new[] { "en", "de" });
                //Debug.WriteLine("OCR result: " + result.RecognizedText);
                //ShowMessage("OCR Text", result.RecognizedText);
            });
        }

        partial void DocumentDetectionTouchUpInside(UIButton sender)
        {
            if (!CheckScanbotSDKLicense()) { return; }

            // Select image from PhotoLibrary and run document detection

            imagePicker = new UIImagePickerController
            {
                SourceType = UIImagePickerControllerSourceType.PhotoLibrary,
                MediaTypes = new string[] { UTType.Image }
            };
            imagePicker.FinishedPickingMedia += Handle_FinishedPickingMedia;
            imagePicker.Canceled += delegate
            {
                imagePicker.DismissModalViewController(true);
            };

            //Display the imagePicker controller:
            this.PresentModalViewController(imagePicker, true);
        }

        partial void CroppingUITouchUpInside(UIButton sender)
        {
            if (!CheckScanbotSDKLicense()) { return; }
            if (!CheckOriginalImageUrl()) { return; }

            var image = originalImage;
            var cropViewController = new CroppingDemoNavigationController(image);
            cropViewController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            cropViewController.NavigationBar.BarStyle = UIBarStyle.Black;
            cropViewController.NavigationBar.TintColor = UIColor.White;
            croppingHandler.parentController = this;
            cropViewController.croppingDelegate = this.croppingHandler;
            PresentViewController(cropViewController, true, null);
        }

        partial void CameraUITouchUpInside(UIButton sender)
        {
            if (!CheckScanbotSDKLicense()) { return; }

            var cameraViewController = new CameraDemoViewController();
            cameraHandler.parentController = this;
            cameraViewController.cameraDelegate = this.cameraHandler;
            NavigationController.PushViewController(cameraViewController, true);
        }

        partial void GenericDocumentRecognizerTouchUpInside(UIButton sender)
        {
            if (!CheckScanbotSDKLicense()) { return; }

            var configuration = SBSDKUIGenericDocumentRecognizerConfiguration.DefaultConfiguration;
            configuration.TextConfiguration.CancelButtonTitle = "Done";
            configuration.BehaviorConfiguration.DocumentType = SBSDKUIDocumentType.IdCardFrontBackDE;

            gdrDelegate.rootVc.SetTarget(this);
            var scanner = SBSDKUIGenericDocumentRecognizerViewController.CreateNewWithConfiguration(configuration, gdrDelegate);
            NavigationController.PushViewController(scanner, true);
        }

        partial void CheckRecognizerTouchUpInside(UIButton sender)
        {
            if (!CheckScanbotSDKLicense()) { return; }
            var vc = new CheckRecognizerDemoViewController();
            NavigationController.PushViewController(vc, true);
        }

        bool CheckDocumentImageUrl()
        {
            if (documentImage == null)
            {
                ShowErrorMessage("Please snap a document image via Scanning UI or run Document Detection on an image file from the PhotoLibrary");
                return false;
            }
            return true;
        }

        bool CheckOriginalImageUrl()
        {
            if (originalImage == null)
            {
                ShowErrorMessage("Please snap a document image via Scanning UI or run Document Detection on an image file from the PhotoLibrary");
                return false;
            }
            return true;
        }

        bool CheckSelectedImages()
        {
            if (SBSDKUIPageFileStorage.DefaultStorage.AllPageFileIDs.Length == 0)
            {
                ShowErrorMessage("Please select at least one image from Gallery or via Camera UI");
                return false;
            }
            return true;
        }

        bool CheckScanbotSDKLicense()
        {
            if (ScanbotSDKGlobal.IsLicenseValid)
            {
                // Trial period, valid trial license or valid production license.
                return true;
            }

            ShowErrorMessage("Scanbot SDK (trial) license has expired!");
            return false;
        }

        public void ShowMessage(string title, string message, UIViewController controller = null)
        {
            UIViewController presenter = controller != null ? controller : this;

            InvokeOnMainThread(() =>
            {
                var alertController = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
                alertController.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null));
                presenter.PresentViewController(alertController, true, null);
            });
        }

        public void ShowResultMessage(string message)
        {
            ShowMessage("Operation result", message);
        }

        public void ShowErrorMessage(string message)
        {
            ShowMessage("Error", message);
        }        

        protected void Handle_FinishedPickingMedia(object sender, UIImagePickerMediaPickedEventArgs e)
        {
            NSUrl referenceURL = e.Info[UIImagePickerController.ReferenceUrl] as NSUrl;
            if (referenceURL != null)
            {
                //NSUrl imgUrl = e.Info[UIImagePickerController.ReferenceUrl] as NSUrl;
                // Get the original image
                UIImage originalImage = e.Info[UIImagePickerController.OriginalImage] as UIImage;
                if (originalImage != null)
                {
                    Debug.WriteLine("Got the original image from gallery");
                    SBSDKUIPageFileStorage.DefaultStorage.AddImage(originalImage);
                    this.originalImage = originalImage;
                    ShowImageView(originalImage);
                    RunDocumentDetection(originalImage);
                }
            }

            // Dismiss the picker
            imagePicker.DismissModalViewController(true);
        }

        async void RunDocumentDetection(UIImage image)
        {
            var detectionResult = await Task.Run(() =>
            {
                // The SDK call is sync!
                SBSDKDocumentDetector detector = new SBSDKDocumentDetector();
                return detector.DetectDocumentPolygonOnImage(image, new CGRect(CGPoint.Empty, image.Size), false, false);
            });

            if (detectionResult.Status == SBSDKDocumentDetectionStatus.Ok)
            {
                var imageResult = image;

                if (detectionResult.Polygon != null && image != null)
                {
                    imageResult = image.ImageWarpedByPolygon(detectionResult.Polygon, imageScale: 1.0f);
                }

                Debug.WriteLine("Detection result image: " + imageResult);
                SBSDKUIPageFileStorage.DefaultStorage.AddImage(imageResult);
                documentImage = imageResult;

                ShowImageView(imageResult);
            }
            else
            {
                Debug.WriteLine("No Document detected! DetectionStatus = " + detectionResult.Status);
                ShowErrorMessage("No Document detected! DetectionStatus = " + detectionResult.Status);
            }
        }

        partial void CreateTiffFileTouchUpInside(UIButton sender)
        {
            if (!CheckScanbotSDKLicense()) { return; }
            if (!CheckDocumentImageUrl()) { return; }

            Task.Run(() =>
            {
                Debug.WriteLine("Creating TIFF file ...");
                var images = SBSDKUIPageFileStorage.DefaultStorage.AllPageFileIDs
                            .Select(id => SBSDKUIPageFileStorage.DefaultStorage.ImageWithPageFileID(id, SBSDKUIPageFileType.Original))
                            .ToArray();

                var tiffOutputFileUrl = GenerateRandomFileUrlInDemoTempStorage(".tiff");
//                SBSDK.WriteTiff(images, tiffOutputFileUrl, new TiffOptions { OneBitEncoded = true });

                var options = new SBSDKTIFFImageWriterParameters { Binarize = true };

                bool success = false;

                // TODO figure out if this is still needed
                //if (ScanbotSDKUI.DefaultImageStoreEncrypter != null)
                {
                    //var encrypter = ScanbotSDKUI.DefaultImageStoreEncrypter;
                    //success = SBSDKTIFFImageWriter.WriteTIFF(images.Select(i => UIImage.LoadFromData(encrypter.DecryptData(NSData.FromUrl(i)))).ToArray(), tiffOutputFileUrl, encrypter, options);
                }
                //else
                {
                    success = SBSDKTIFFImageWriter.WriteTIFF(images, tiffOutputFileUrl, options);
                }

                Debug.WriteLine("TIFF file created: " + tiffOutputFileUrl);
                ShowMessage("TIFF file created", "" + tiffOutputFileUrl);
            });
        }

        public SBSDKIndexedImageStorage CreateStorage(NSUrl[] uris)
        {
            var url = SBSDKStorageLocation.ApplicationSupportFolderURL;
            var tmp = NSUrl.FromFilename(string.Format("{0}/{1}", url.Scheme == "file" ? url.Path : url.AbsoluteString, Guid.NewGuid()));
            var location = new SBSDKStorageLocation(tmp);
            var format = SBSDKImageFileFormat.Jpeg;

            return new SBSDKIndexedImageStorage(location, format, ScanbotSDKUI.DefaultImageStoreEncrypter, uris);
        }

        partial void CreatePdfTouchUpInside(UIButton sender)
        {
            if (!CheckScanbotSDKLicense()) { return; }
            if (!CheckDocumentImageUrl()) { return; }

            Task.Run(() =>
            {
                Debug.WriteLine("Creating PDF file ...");
                var images = SBSDKUIPageFileStorage.DefaultStorage.AllPageFileIDs
                            .Select(id => SBSDKUIPageFileStorage.DefaultStorage.ImageURLWithPageFileID(id, SBSDKUIPageFileType.Original))
                            .ToArray();
                var pdfOutputFileUrl = GenerateRandomFileUrlInDemoTempStorage(".pdf");

                SBSDKPDFRenderer.RenderImageStorage(CreateStorage(images), null, SBSDKPDFRendererPageSize.FixedA4, ScanbotSDKUI.DefaultImageStoreEncrypter, pdfOutputFileUrl, out var error);
                Debug.WriteLine("PDF file created: " + pdfOutputFileUrl);
                ShowMessage("PDF file created", "" + pdfOutputFileUrl);
            });
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);
        }

        private void ShowImageView(UIImage hiresImage)
        {
            var previewImage = ExampleImageUtils.MaxResizeImage(hiresImage, 900, 900);
            InvokeOnMainThread(() =>
            {
                selectedImageView.Image = previewImage;
                selectImageLabel.Hidden = true;
            });
        }

        private NSUrl GenerateRandomFileUrlInDemoTempStorage(string fileExtension)
        {
            var targetFile = System.IO.Path.Combine(
                AppDelegate.Directory, new NSUuid().AsString().ToLower() + fileExtension);
            return NSUrl.FromFilename(targetFile);
        }

        private class CameraDemoDelegateHandler : CameraDemoDelegate
        {
            public MainSelectionTableViewController parentController;

            public override void DidCaptureDocumentImage(UIImage documentImage)
            {
                if (parentController != null)
                {
                    parentController.documentImage = documentImage;
                    SBSDKUIPageFileStorage.DefaultStorage.AddImage(documentImage);
                    parentController.ShowImageView(documentImage);
                }
            }

            public override void DidCaptureOriginalImage(UIImage originalImage)
            {
                if (parentController != null)
                {
                    SBSDKUIPageFileStorage.DefaultStorage.AddImage(originalImage);
                    parentController.originalImage = originalImage;
                }
            }
        }

        private class CroppingDemoDelegateHandler : CroppingDemoDelegate
        {
            public MainSelectionTableViewController parentController;

            public override void CropViewControllerDidFinish(UIImage croppedImage)
            {
                // Obtain cropped image from cropping view controller
                if (parentController != null)
                {
                    SBSDKUIPageFileStorage.DefaultStorage.AddImage(croppedImage);
                    parentController.documentImage = croppedImage;
                    parentController.ShowImageView(croppedImage);
                }
            }
        }

        private class GenericDocumentRecognizerDelegate : SBSDKUIGenericDocumentRecognizerViewControllerDelegate
        {
            public WeakReference<MainSelectionTableViewController> rootVc;
            public GenericDocumentRecognizerDelegate(MainSelectionTableViewController rootVc)
            {
                this.rootVc = new WeakReference<MainSelectionTableViewController>(rootVc);
            }

            public override void DidFinishWithDocuments(SBSDKUIGenericDocumentRecognizerViewController viewController, SBSDKGenericDocument[] documents)
            {
                if (documents == null || documents.Length == 0)
                {
                    return;
                }

                // We only take the first document for simplicity
                var firstDocument = documents.First();
                var fields = firstDocument.Fields
                    .Where((f) => f != null && f.Type != null && f.Type.Name != null && f.Value != null && f.Value.Text != null)
                    .Select((f) => string.Format("{0}: {1}", f.Type.Name, f.Value.Text))
                    .ToList();
                var description = string.Join("\n", fields);

                rootVc.TryGetTarget(out MainSelectionTableViewController vc);
                if (vc != null)
                {
                    vc.ShowResultMessage(description);
                }
            }
        }
    }
}