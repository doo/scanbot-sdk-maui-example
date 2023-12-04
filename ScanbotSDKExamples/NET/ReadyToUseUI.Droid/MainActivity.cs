using Android.Views;
using ReadyToUseUI.Droid.Views;
using ReadyToUseUI.Droid.Fragments;
using Android.Graphics;
using Android.Content;
using Android.Runtime;
using IO.Scanbot.Sdk.UI.View.Mrz.Configuration;
using IO.Scanbot.Sdk.UI.View.Mrz;
using IO.Scanbot.Sdk.UI.View.Camera.Configuration;
using IO.Scanbot.Sdk.UI.View.Camera;
using IO.Scanbot.Sdk.Persistence;
using ReadyToUseUI.Droid.Activities;
using ReadyToUseUI.Droid.Utils;
using IO.Scanbot.Sdk.Process;
using IO.Scanbot.Sdk.UI.View.Barcode.Configuration;
using IO.Scanbot.Sdk.UI.View.Barcode;
using IO.Scanbot.Sdk.Barcode.Entity;
using IO.Scanbot.Sdk.UI.View.Hic.Configuration;
using IO.Scanbot.Sdk.UI.View.Hic;
using IO.Scanbot.Hicscanner.Model;
using IO.Scanbot.Sdk.Camera;
using IO.Scanbot.Sdk.Core.Contourdetector;
using IO.Scanbot.Sdk.UI.View.Barcode.Batch.Configuration;
using IO.Scanbot.Sdk.UI.View.Barcode.Batch;
using IO.Scanbot.Sdk.UI.View.Genericdocument.Configuration;
using IO.Scanbot.Genericdocument.Entity;
using IO.Scanbot.Sdk.UI.View.Genericdocument;
using IO.Scanbot.Sdk.UI.Result;
using IO.Scanbot.Sdk.UI.View.Base;
using IO.Scanbot.Sdk.UI.View.Check.Configuration;
using IO.Scanbot.Sdk.UI.View.Check;
using IO.Scanbot.Sdk.Check.Entity;
using DocumentSDK.NET.Model;
using IO.Scanbot.Mrzscanner.Model;
using System.Diagnostics;

namespace ReadyToUseUI.Droid
{
    [Activity(Label = "NET RTU UI", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : AndroidX.AppCompat.App.AppCompatActivity
    {
        private const int QR_BARCODE_DEFAULT_UI_REQUEST_CODE = 910;
        private const int IMPORT_BARCODE_REQUEST = 1338;

        private const int CAMERA_DEFAULT_UI_REQUEST_CODE = 1111;
        private const int IMPORT_IMAGE_REQUEST = 7777;

        private const int MRZ_DEFAULT_UI_REQUEST_CODE = 909;
        private const int REQUEST_EHIC_SCAN = 4715;
        private const int GENERIC_DOCUMENT_RECOGNIZER_REQUEST = 4716;
        private const int CHECK_RECOGNIZER_REQUEST = 4717;

        private readonly List<ListItemButton> buttons = new List<ListItemButton>();
        private ProgressBar progress;
        private IO.Scanbot.Sdk.ScanbotSDK scanbotSDK;
        private IO.Scanbot.Sdk.Persistence.PageFileStorage pageStorage;

        private TextView LicenseIndicator
        {
            get => FindViewById(Resource.Id.container).FindViewById<TextView>(Resource.Id.licenseIndicator);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            scanbotSDK = new IO.Scanbot.Sdk.ScanbotSDK(this);
            pageStorage = scanbotSDK.CreatePageFileStorage();

            var container = (LinearLayout)FindViewById(Resource.Id.container);

            var title = container.FindViewById<TextView>(Resource.Id.textView);
            title.Text = Texts.scanbot_sdk_demo;

            progress = FindViewById<ProgressBar>(Resource.Id.progressBar);


            var barcodes = (LinearLayout)container.FindViewById(Resource.Id.barcode_detectors);
            var barcodeTitle = (TextView)barcodes.FindViewById(Resource.Id.textView);
            barcodeTitle.Text = "Barcode Scanner".ToUpper();
            barcodes.AddChildren(buttons, new[]
            {
                new ListItemButton(this, "Scan Barcodes", BarcodeScanner),
                new ListItemButton(this, "Scan Batch Barcodes", BatchBarcodeScanner),
                new ListItemButton(this, "Import and Detect Barcodes", ImportBarcode)
            });

            var scanner = (LinearLayout)container.FindViewById(Resource.Id.document_scanner);
            var scannerTitle = (TextView)scanner.FindViewById(Resource.Id.textView);
            scannerTitle.Text = "Document Scanner".ToUpper();
            scanner.AddChildren(buttons, new[]
            {
                new ListItemButton(this, "Scan Document", ScanDocument),
                new ListItemButton(this, "Scan Document with Finder", ScanDocumentWithFinder),
                new ListItemButton(this, "Import Image", ImportImage),
                new ListItemButton(this, "View Images", ViewImages)
            });

            var detectors = (LinearLayout)container.FindViewById(Resource.Id.data_detectors);
            var detectorsTitle = (TextView)detectors.FindViewById(Resource.Id.textView);
            detectorsTitle.Text = "Data Detectors".ToUpper();
            detectors.AddChildren(buttons, new[]  
            {
                new ListItemButton(this, "Scan MRZ", ScanMrz),
                new ListItemButton(this, "Scan Health Insurance card", ScanEhic),
                new ListItemButton(this, "Generic Document Recognizer", ScanGenericDocument),
                new ListItemButton(this, "Check Recognizer", RecogniseCheck),
            });

            LicenseIndicator.Text = Texts.no_license_found_the_app_will_terminate_after_one_minute;
        }

        protected override void OnResume()
        {
            base.OnResume();

            CheckLicense();

            foreach (var button in buttons)
            {
                button.Click += OnButtonClick;
            }
        }

        protected override void OnPause()
        {
            base.OnPause();

            foreach (var button in buttons)
            {
                button.Click -= OnButtonClick;
            }
        }

        private void ViewImages()
        {
            var intent = new Intent(this, typeof(PagePreviewActivity));
            StartActivity(intent);
        }

        private void BarcodeScanner()
        {
            var configuration = new BarcodeScannerConfiguration();
            configuration.SetFinderTextHint("Please align the QR-/Barcode in the frame above to scan it");
            configuration.SetSelectionOverlayConfiguration(new SelectionOverlayConfiguration(
                            overlayEnabled: true,
                            automaticSelectionEnabled: false,
                            textFormat: IO.Scanbot.Sdk.Barcode.UI.BarcodeOverlayTextFormat.CodeAndType,
                            polygonColor: Color.Yellow,
                            textColor: Color.Yellow,
                            textContainerColor: Color.Black));
            var intent = BarcodeScannerActivity.NewIntent(this, configuration);
            StartActivityForResult(intent, QR_BARCODE_DEFAULT_UI_REQUEST_CODE);
        }

        private void BatchBarcodeScanner()
        {
            var configuration = new BatchBarcodeScannerConfiguration();
            configuration.SetSelectionOverlayConfiguration(new SelectionOverlayConfiguration(
                overlayEnabled: true,
                automaticSelectionEnabled: false,
                textFormat: IO.Scanbot.Sdk.Barcode.UI.BarcodeOverlayTextFormat.CodeAndType,
                polygonColor: Color.Yellow,
                textColor: Color.Yellow,
                textContainerColor: Color.Black));
            configuration.SetFinderTextHint("Please align the QR-/Barcode in the frame above to scan it");
            var intent = BatchBarcodeScannerActivity.NewIntent(this, configuration, null);
            StartActivityForResult(intent, QR_BARCODE_DEFAULT_UI_REQUEST_CODE);
        }

        private void ImportBarcode()
        {
            var intent = new Intent();
            intent.SetType("image/*");
            intent.SetAction(Intent.ActionGetContent);
            intent.PutExtra(Intent.ExtraLocalOnly, false);
            intent.PutExtra(Intent.ExtraAllowMultiple, false);

            var chooser = Intent.CreateChooser(intent, Texts.share_title);
            StartActivityForResult(chooser, IMPORT_BARCODE_REQUEST);
        }

        private void ScanDocument()
        {
            var configuration = new DocumentScannerConfiguration();

            configuration.SetCameraPreviewMode(CameraPreviewMode.FitIn);
            configuration.SetIgnoreBadAspectRatio(true);
            configuration.SetMultiPageEnabled(true);
            configuration.SetPageCounterButtonTitle("%d Page(s)");
            configuration.SetTextHintOK("Don't move.\nScanning document...");

            // further configuration properties
            //configuration.SetBottomBarBackgroundColor(Color.Blue);
            //configuration.SetBottomBarButtonsColor(Color.White);
            //configuration.SetFlashButtonHidden(true);
            // and so on...

            var intent = DocumentScannerActivity.NewIntent(this, configuration);
            StartActivityForResult(intent, CAMERA_DEFAULT_UI_REQUEST_CODE);
        }

        private void ScanDocumentWithFinder()
        {
            var configuration = new FinderDocumentScannerConfiguration();

            configuration.SetCameraPreviewMode(CameraPreviewMode.FitIn);
            configuration.SetIgnoreBadAspectRatio(true);
            configuration.SetTextHintOK("Don't move.\nScanning document...");
            configuration.SetOrientationLockMode(IO.Scanbot.Sdk.UI.View.Base.Configuration.CameraOrientationMode.Portrait);
            configuration.SetFinderAspectRatio(new IO.Scanbot.Sdk.AspectRatio(21.0, 29.7)); // a4 portrait

            // further configuration properties
            //configuration.SetFinderLineColor(Color.Red);
            //configuration.SetTopBarBackgroundColor(Color.Blue);
            //configuration.SetFlashButtonHidden(true);
            // and so on...

            var intent = FinderDocumentScannerActivity.NewIntent(this, configuration);
            StartActivityForResult(intent, CAMERA_DEFAULT_UI_REQUEST_CODE);
        }

        private void ImportImage()
        {
            var intent = new Intent();
            intent.SetType("image/*");
            intent.SetAction(Intent.ActionGetContent);
            intent.PutExtra(Intent.ExtraLocalOnly, false);
            intent.PutExtra(Intent.ExtraAllowMultiple, false);

            var chooser = Intent.CreateChooser(intent, Texts.share_title);
            StartActivityForResult(chooser, IMPORT_IMAGE_REQUEST);
        }

        private void ScanMrz()
        {
            var configuration = new MRZScannerConfiguration();
            configuration.SetSuccessBeepEnabled(false);

            var intent = MRZScannerActivity.NewIntent(this, configuration);
            StartActivityForResult(intent, MRZ_DEFAULT_UI_REQUEST_CODE);
        }

        private void ScanEhic()
        {
            var config = new HealthInsuranceCardScannerConfiguration();
            config.SetTopBarButtonsColor(Color.White);

            var intent = HealthInsuranceCardScannerActivity.NewIntent(this, config);
            StartActivityForResult(intent, REQUEST_EHIC_SCAN);
        }

        private void ScanGenericDocument()
        {
            var config = new GenericDocumentRecognizerConfiguration();
            config.SetAcceptedDocumentTypes(new List<RootDocumentType>
                    {
                        RootDocumentType.DeIdCardFront,
                        RootDocumentType.DeIdCardBack,
                    });

            var intent = GenericDocumentRecognizerActivity.NewIntent(this, config);
            StartActivityForResult(intent, GENERIC_DOCUMENT_RECOGNIZER_REQUEST);
        }

        private void RecogniseCheck()
        {
            var config = new CheckRecognizerConfiguration();
            config.SetAcceptedCheckStandards(new List<IO.Scanbot.Check.Entity.RootDocumentType>
                {
                    IO.Scanbot.Check.Entity.RootDocumentType.AUSCheck,
                    IO.Scanbot.Check.Entity.RootDocumentType.FRACheck,
                    IO.Scanbot.Check.Entity.RootDocumentType.INDCheck,
                    IO.Scanbot.Check.Entity.RootDocumentType.KWTCheck,
                    IO.Scanbot.Check.Entity.RootDocumentType.USACheck,
                });
            var intent = CheckRecognizerActivity.NewIntent(this, config);
            StartActivityForResult(intent, CHECK_RECOGNIZER_REQUEST);
        }

        /**
         * Scanner returned, parse results
         */
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode != Result.Ok)
            {
                return;
            }

            if (!scanbotSDK.LicenseInfo.IsValid)
            {
                Alert.ShowLicenseDialog(this);
                return;
            }

            switch (requestCode)
            {
                case QR_BARCODE_DEFAULT_UI_REQUEST_CODE:
                {
                    var result = (BarcodeScanningResult)data.GetParcelableExtra(RtuConstants.ExtraKeyRtuResult);
                    var fragment = BarcodeDialogFragment.CreateInstance(result);
                    fragment.Show(FragmentManager);
                    return;
                }
                case IMPORT_BARCODE_REQUEST:
                {
                    var bitmap = Utils.ImageUtils.ProcessGalleryResult(this, data);
                    var detector = scanbotSDK.CreateBarcodeDetector();
                    var result = detector.DetectFromBitmap(bitmap, 0);
                    // Estimate blur of imported barcode
                    // Estimating blur on already cropped barcodes should
                    // normally yield the best results, as there is little empty space
                    var estimator = scanbotSDK.CreateBlurEstimator();

                    var fragment = BarcodeDialogFragment.CreateInstance(result, estimator.EstimateInBitmap(bitmap, 0));
                    fragment.Show(FragmentManager);
                    return;
                }
                case CAMERA_DEFAULT_UI_REQUEST_CODE:
                {
                     var parcelable = data.GetParcelableArrayExtra(RtuConstants.ExtraKeyRtuResult);
                     var intent = new Intent(this, typeof(PagePreviewActivity));
                     StartActivity(intent);
                     return;
                }
                case IMPORT_IMAGE_REQUEST:
                {
                    progress.Visibility = ViewStates.Visible;
                    Alert.Toast(this, Texts.importing_and_processing);
                    var result = Utils.ImageUtils.ProcessGalleryResult(this, data);

                    var pageId = pageStorage.Add(result);
                    var page = new Page(pageId, new List<PointF>(), DetectionStatus.Ok, ImageFilterType.None);
                    page = scanbotSDK.CreatePageProcessor().DetectDocument(page);

                    var intent = new Intent(this, typeof(PagePreviewActivity));
                    progress.Visibility = ViewStates.Gone;
                    StartActivity(intent);
                    return;
                }
                case MRZ_DEFAULT_UI_REQUEST_CODE:
                {
                    var result = (MRZGenericDocument)data.GetParcelableExtra(RtuConstants.ExtraKeyRtuResult);
                    var fragment = MRZDialogFragment.CreateInstance(result);
                    fragment.Show(FragmentManager, MRZDialogFragment.NAME);
                    return;
                }
                case GENERIC_DOCUMENT_RECOGNIZER_REQUEST:
                {
                    var resultsArray = data.GetParcelableArrayListExtra(RtuConstants.ExtraKeyRtuResult);
                    if (resultsArray.Count == 0)
                    {
                        return;
                    }

                    var resultWrapper = (ResultWrapper)resultsArray[0];
                    var resultRepository = scanbotSDK.ResultRepositoryForClass(resultWrapper.Clazz);
                    var genericDocument = (GenericDocument)resultRepository.GetResultAndErase(resultWrapper.ResultId);
                    var fields = genericDocument.Fields.Cast<Field>().ToList();
                    var description = string.Join(";\n", fields
                            .Where(field => field != null)
                            .Select((field) =>
                            {
                                string outStr = "";
                                if (field.GetType() != null && field.GetType().Name != null)
                                {
                                    outStr += field.GetType().Name + " = ";
                                }
                                if (field.Value != null && field.Value.Text != null)
                                {
                                    outStr += field.Value.Text;
                                }
                                return outStr;
                            })
                            .ToList()
                        );

                    Debug.WriteLine("GDR Result: ", description);
                    ShowAlert("Result", description);
                    return;
                }
                case REQUEST_EHIC_SCAN:
                {
                    var result = (HealthInsuranceCardRecognitionResult)data.GetParcelableExtra(
                            RtuConstants.ExtraKeyRtuResult);

                    var fragment = HealthInsuranceCardFragment.CreateInstance(result);
                    fragment.Show(FragmentManager, HealthInsuranceCardFragment.NAME);
                    return;
                }
                case CHECK_RECOGNIZER_REQUEST:
                {
                    var resultWrapper = (ResultWrapper)data.GetParcelableExtra(RtuConstants.ExtraKeyRtuResult);
                    var resultRepository = scanbotSDK.ResultRepositoryForClass(resultWrapper.Clazz);
                    var checkResult = (CheckRecognizerResult)resultRepository.GetResultAndErase(resultWrapper.ResultId);
                    var fields = checkResult.Check.Fields;
                    var description = string.Join(";\n", fields
                            .Where(field => field != null)
                            .Select((field) =>
                            {
                                string outStr = "";
                                if (field.GetType() != null && field.GetType().Name != null)
                                {
                                    outStr += field.GetType().Name + " = ";
                                }
                                if (field.Value != null && field.Value.Text != null)
                                {
                                    outStr += field.Value.Text;
                                }
                                return outStr;
                            })
                            .ToList());
                    Debug.WriteLine("Check Recognizer Result: ", description);
                    ShowAlert("Result", description);
                    return;                            
                }
            }
        }

        private void ShowAlert(string title, string message)
        {
            var dialog = new AndroidX.AppCompat.App.AlertDialog.Builder(this);
            AndroidX.AppCompat.App.AlertDialog alert = dialog.Create();
            alert.SetTitle(title);
            alert.SetMessage(message);
            alert.SetButton((int)DialogButtonType.Neutral, "OK", (c, ev) =>
            {
                alert.Dismiss();
            });
            alert.Show();
        }

        private bool CheckLicense()
        {
            if (scanbotSDK.LicenseInfo.IsValid)
            {
                LicenseIndicator.Visibility = ViewStates.Gone;
            }
            else
            {
                LicenseIndicator.Visibility = ViewStates.Visible;
                Alert.Toast(this, "Invalid or missing license");
            }

            return scanbotSDK.LicenseInfo.IsValid;
        }

        private void OnButtonClick(object sender, EventArgs e)
        {
            if (!CheckLicense())
            {
                return;
            }

            if (sender is ListItemButton button && button.DoAction != null)
            {
                button.DoAction();
            }
        }
    }
}