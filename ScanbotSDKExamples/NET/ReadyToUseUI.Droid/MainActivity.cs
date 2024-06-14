using Android.Views;
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
using IO.Scanbot.Ehicscanner.Model;
using IO.Scanbot.Sdk.Camera;
using IO.Scanbot.Sdk.Core.Contourdetector;
using IO.Scanbot.Sdk.UI.View.Barcode.Batch.Configuration;
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
using IO.Scanbot.Sdk.UI.View.Barcode.Batch;
using IO.Scanbot.Sdk.UI.View.Base.Configuration;
using IO.Scanbot.Sdk.UI.View.MC.Configuration;
using IO.Scanbot.Sdk.UI.View.Generictext.Entity;
using IO.Scanbot.Sdk.UI.View.Generictext.Configuration;
using IO.Scanbot.Sdk.UI.View.Vin.Configuration;
using IO.Scanbot.Sdk.UI.View.Vin;
using IO.Scanbot.Sdk.UI.View.Licenseplate.Configuration;
using IO.Scanbot.Sdk.UI.View.Licenseplate;
using IO.Scanbot.Sdk.UI.View.MC;
using IO.Scanbot.Sdk.Mcrecognizer.Entity;
using IO.Scanbot.Sdk.Ui_v2.Barcode.Common.Mappers;
using IO.Scanbot.Sdk.UI.View.Generictext;
using IO.Scanbot.Sdk.Vin;
using ReadyToUseUI.Droid.Snippets;
using BarcodeScannerActivityV2 = IO.Scanbot.Sdk.Ui_v2.Barcode.BarcodeScannerActivity;
using BarcodeScannerConfigurationV2 = IO.Scanbot.Sdk.Ui_v2.Barcode.Configuration.BarcodeScannerConfiguration;
using IO.Scanbot.Sdk.Ui_v2.Common;
using IO.Scanbot.Sdk.Ui_v2.Barcode.Configuration;
using BarcodeScannerConfiguration = IO.Scanbot.Sdk.UI.View.Barcode.Configuration.BarcodeScannerConfiguration;

namespace ReadyToUseUI.Droid
{
    [Activity(Label = "NET RTU UI", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : AndroidX.AppCompat.App.AppCompatActivity, IBarcodeItemMapper
    {
        private const int BARCODE_DEFAULT_UI_REQUEST_CODE_V2 = 911;

        private const int SCAN_DOCUMENT_REQUEST_CODE = 1000;

        private const int IMPORT_IMAGE_REQUEST = 2001;
        private const int IMPORT_BARCODE_REQUEST = 2002;

        private const int QR_BARCODE_DEFAULT_REQUEST = 3001;

        private const int SCAN_MRZ_REQUEST = 4001;
        private const int GENERIC_DOCUMENT_REQUEST = 4002;
        private const int SCAN_DATA_REQUEST = 4003;
        private const int SCAN_VIN_REQUEST = 4004;
        private const int SCAN_EU_LICENSE_REQUEST = 4005;
        private const int SCAN_EHIC_REQUEST = 4006;
        private const int SCAN_MEDICAL_CERTIFICATE_REQUEST = 4007;

        private const int CHECK_RECOGNIZER_REQUEST = 5001;

        private readonly List<ListItemButton> buttons = new List<ListItemButton>();
        private ProgressBar progress;

        private IO.Scanbot.Sdk.ScanbotSDK scanbotSDK;
        private PageFileStorage pageStorage;
        private TextView licenseIndicator;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            scanbotSDK = new IO.Scanbot.Sdk.ScanbotSDK(this);
            pageStorage = scanbotSDK.CreatePageFileStorage();

            var container = (LinearLayout)FindViewById(Resource.Id.container);
            var title = container.FindViewById<TextView>(Resource.Id.textView);
            title.Text = Texts.scanbot_sdk_demo;

            var barcodeDetectors = (LinearLayout)container.FindViewById(Resource.Id.barcode_data_scanner);
            var barcodeDetectorsTitle = (TextView)barcodeDetectors.FindViewById(Resource.Id.textView);
            barcodeDetectorsTitle.Text = "BARCODE DETECTORS V1";
            barcodeDetectors.AddChildren(buttons, new[]
            {
                new ListItemButton(this, "Scan Barcodes", ScanBarcode),
                new ListItemButton(this, "Scan Batch Barcodes", ScanBarcodesInBatch),
                new ListItemButton(this, "Import and Detect Barcodes", ImportAndDetectBarcode),
            });

            var barcodeDetectorV2 = (LinearLayout)container.FindViewById(Resource.Id.barcode_data_scanner_v2);
            var barcodeDetectorV2Title = (TextView)barcodeDetectorV2.FindViewById(Resource.Id.textView);
            barcodeDetectorV2Title.Text = "BARCODE DETECTORS V2";
            barcodeDetectorV2.AddChildren(buttons, new[]
            {
                new ListItemButton(this, "Scan - Single", ScanBarcodeV2_SingleScan),
                new ListItemButton(this, "Scan - Single - AR Overlay", ScanBarcodeV2_AR_Overlay),
                new ListItemButton(this, "Scan - Multiple", ScanBarcodeV2_MultiScanning),
                new ListItemButton(this, "Scan - Count & Map", ScanBarcodeV2_CountAndMap),
            });

            var scanner = (LinearLayout)container.FindViewById(Resource.Id.document_scanner);
            var scannerTitle = (TextView)scanner.FindViewById(Resource.Id.textView);
            scannerTitle.Text = "DOCUMENT SCANNER";
            scanner.AddChildren(buttons, new[]
            {
                new ListItemButton(this, "Scan Document", ScanDocument),
                new ListItemButton(this, "Scan Document with Finder", ScanDocumentWithFinder),
                new ListItemButton(this, "Import Image", ImportImage),
                new ListItemButton(this, "View Images", ViewImages)
            });

            var detectors = (LinearLayout)container.FindViewById(Resource.Id.data_detectors);
            var detectorsTitle = (TextView)detectors.FindViewById(Resource.Id.textView);
            detectorsTitle.Text = "DATA DETECTORS";
            detectors.AddChildren(buttons, new[]
            {
                new ListItemButton(this, "Scan MRZ", ScanMrz),
                new ListItemButton(this, "Scan Health Insurance card", ScanEhic),
                new ListItemButton(this, "Generic Document Recognizer", RecongnizeGenericDocument),
                new ListItemButton(this, "Check Recognizer", RecogniseCheck),
                new ListItemButton(this, "Text Data Recognizer", TextDataRecognizerTapped),
                new ListItemButton(this, "VIN Recognizer", VinRecognizerTapped),
                new ListItemButton(this, "License Plate Recognizer", LicensePlateRecognizerTapped),
                new ListItemButton(this, "Medical Certificate Recognizer", ScanMedicalCertificate),
            });

            progress = FindViewById<ProgressBar>(Resource.Id.progressBar);

            licenseIndicator = container.FindViewById<TextView>(Resource.Id.licenseIndicator);
            licenseIndicator.Text = Texts.no_license_found_the_app_will_terminate_after_one_minute;
            licenseIndicator.Visibility = string.IsNullOrEmpty(MainApplication.LicenseKey) ? ViewStates.Visible : ViewStates.Gone;

            foreach (var button in buttons)
            {
                button.Click += OnButtonClick;
            }
        }

        private void ScanBarcodeV2_SingleScan()
        {
            if (!CheckLicense())
            {
                return;
            }

            var configuration = new BarcodeScannerConfigurationV2();
            configuration.TopBar.CancelButton.Background.FillColor = new ScanbotColor("#FFFFFF");
            configuration.BackgroundColor = new ScanbotColor("#000000");

            var intent = BarcodeScannerActivityV2.NewIntent(this, configuration);
            StartActivityForResult(intent, BARCODE_DEFAULT_UI_REQUEST_CODE_V2);
        }

        private void ScanBarcodeV2_AR_Overlay()
        {
            if (!CheckLicense())
            {
                return;
            }

            var configuration = new BarcodeScannerConfigurationV2();
            configuration.TopBar.CancelButton.Background.FillColor = new ScanbotColor("#FFFFFF");
            configuration.BackgroundColor = new ScanbotColor("#000000");
            var result = new IO.Scanbot.Sdk.Ui_v2.Barcode.Configuration.SingleScanningMode();
            result.ArOverlay.Visible = true;
            result.ArOverlay.AutomaticSelectionEnabled = false;
            configuration.UseCase = result;

            var intent = BarcodeScannerActivityV2.NewIntent(this, configuration);
            StartActivityForResult(intent, BARCODE_DEFAULT_UI_REQUEST_CODE_V2);
        }

        private void ScanBarcodeV2_MultiScanning()
        {
            if (!CheckLicense())
            {
                return;
            }

            var configuration = new BarcodeScannerConfigurationV2();
            configuration.TopBar.CancelButton.Background.FillColor = new ScanbotColor("#FFFFFF");
            configuration.BackgroundColor = new ScanbotColor("#000000");
            configuration.CameraConfiguration.DefaultZoomFactor = 1.0;
            configuration.CameraConfiguration.OrientationLockMode = OrientationLockMode.Portrait;
            configuration.ViewFinder.Visible = true;
            configuration.UserGuidance.Title.Text = "Please align the QR-/Barcode in the frame above to scan it.";

            var result = new MultipleScanningMode();
            result.BarcodeInfoMapping.BarcodeItemMapper = this;
            result.Mode = MultipleBarcodesScanningMode.Unique;
            configuration.UseCase = result;

            var intent = BarcodeScannerActivityV2.NewIntent(this, configuration);
            StartActivityForResult(intent, BARCODE_DEFAULT_UI_REQUEST_CODE_V2);
        }

        public void MapBarcodeItem(IO.Scanbot.Sdk.Ui_v2.Barcode.Configuration.BarcodeItem barcodeItem, IBarcodeMappingResult result)
        {
            result.OnResult(new BarcodeMappedData(
                title: barcodeItem.TextWithExtension,
                subtitle: barcodeItem.Type.Name(),
                barcodeImage: BarcodeMappedDataExtension.BarcodeFormatKey));
        }

        private void ScanBarcodeV2_CountAndMap()
        {
            if (!CheckLicense())
            {
                return;
            }

            var configuration = new BarcodeScannerConfigurationV2();
            configuration.TopBar.CancelButton.Background.FillColor = new ScanbotColor("#FFFFFF");
            configuration.BackgroundColor = new ScanbotColor("#000000");

            configuration.ViewFinder.Visible = true;
            configuration.UserGuidance.Title.Text = "Please align the QR-/Barcode in the frame above to scan it.";

            var result = new MultipleScanningMode();
            result.BarcodeInfoMapping.BarcodeItemMapper = this;
            result.Mode = MultipleBarcodesScanningMode.Counting;

            result.ArOverlay.Visible = true;
            result.ArOverlay.AutomaticSelectionEnabled = false;
            configuration.UseCase = result;

            var intent = BarcodeScannerActivityV2.NewIntent(this, configuration);
            StartActivityForResult(intent, BARCODE_DEFAULT_UI_REQUEST_CODE_V2);
        }

        private void ScanBarcode()
        {
            var configuration = new BarcodeScannerConfiguration();
            configuration.SetSelectionOverlayConfiguration(
                new SelectionOverlayConfiguration(
                   overlayEnabled: true,
                   automaticSelectionEnabled: true,
                   textFormat: IO.Scanbot.Sdk.Barcode.UI.BarcodeOverlayTextFormat.CodeAndType,
                   polygonColor: Color.Yellow,
                   textColor: Color.Yellow,
                   textContainerColor: Color.Black));

            configuration.SetFinderTextHint("Please align the QR-/Barcode in the frame above to scan it.");
            configuration.SetTopBarButtonsColor(Color.White);
            configuration.SetTopBarBackgroundColor(Color.Black);

            var intent = BarcodeScannerActivity.NewIntent(this, configuration);
            StartActivityForResult(intent, QR_BARCODE_DEFAULT_REQUEST);
        }

        private void ScanBarcodesInBatch()
        {
            var configuration = new BatchBarcodeScannerConfiguration();
            configuration.SetSelectionOverlayConfiguration(
                new SelectionOverlayConfiguration(
                    overlayEnabled: true,
                    automaticSelectionEnabled: true,
                    textFormat: IO.Scanbot.Sdk.Barcode.UI.BarcodeOverlayTextFormat.CodeAndType,
                    polygonColor: Color.Yellow,
                    textColor: Color.Yellow,
                    textContainerColor: Color.Black));

            configuration.SetOrientationLockMode(CameraOrientationMode.Portrait);

            configuration.SetFinderTextHint("Please align the QR-/Barcode in the frame above to scan it.");
            var intent = BatchBarcodeScannerActivity.NewIntent(this, configuration);
            StartActivityForResult(intent, QR_BARCODE_DEFAULT_REQUEST);
        }

        private void ImportAndDetectBarcode()
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

            configuration.SetCameraPreviewMode(IO.Scanbot.Sdk.Camera.CameraPreviewMode.FitIn);
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
            StartActivityForResult(intent, SCAN_DOCUMENT_REQUEST_CODE);
        }

        private void ScanDocumentWithFinder()
        {
            var configuration = new FinderDocumentScannerConfiguration();

            configuration.SetCameraPreviewMode(IO.Scanbot.Sdk.Camera.CameraPreviewMode.FitIn);
            configuration.SetIgnoreBadAspectRatio(true);
            configuration.SetTextHintOK("Don't move.\nScanning document...");
            configuration.SetOrientationLockMode(CameraOrientationMode.Portrait);
            configuration.SetFinderAspectRatio(new IO.Scanbot.Sdk.AspectRatio(21.0, 29.7)); // a4 portrait

            // further configuration properties
            //configuration.SetFinderLineColor(Color.Red);
            //configuration.SetTopBarBackgroundColor(Color.Blue);
            //configuration.SetFlashButtonHidden(true);
            // and so on...

            var intent = FinderDocumentScannerActivity.NewIntent(this, configuration);
            StartActivityForResult(intent, SCAN_DOCUMENT_REQUEST_CODE);
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

        private void ViewImages() => StartActivity(new Intent(this, typeof(PagePreviewActivity)));

        private void ScanMrz()
        {
            var configuration = new MRZScannerConfiguration();
            configuration.SetCancelButtonTitle("Done");

            var intent = MRZScannerActivity.NewIntent(this, configuration);
            StartActivityForResult(intent, SCAN_MRZ_REQUEST);
        }

        private void ScanEhic()
        {
            var configuration = new HealthInsuranceCardScannerConfiguration();
            configuration.SetCancelButtonTitle("Done");

            var intent = HealthInsuranceCardScannerActivity.NewIntent(this, configuration);
            StartActivityForResult(intent, SCAN_EHIC_REQUEST);
        }

        private void RecongnizeGenericDocument()
        {
            var configuration = new GenericDocumentRecognizerConfiguration();
            configuration.SetCancelButtonTitle("Done");

            // Specify accepted types if needed
            configuration.SetAcceptedDocumentTypes(new List<RootDocumentType>
            {
                RootDocumentType.DeIdCardFront,
                //RootDocumentType.DeIdCardBack,
                //RootDocumentType.DePassport,
            });

            // Apply the parameters for fields
            // Use constants from NormalizedFieldNames objects from the corresponding document type

            //configuration.SetFieldsDisplayConfiguration
            //(
            //    new Dictionary<string, FieldProperties>()
            //    {
            //        { DeIdCardFront.NormalizedFieldNames.Photo,  new FieldProperties("My Id card photo", FieldProperties.DisplayState.AlwaysVisible) },
            //        { DePassport.NormalizedFieldNames.Photo,  new FieldProperties("My passport photo", FieldProperties.DisplayState.AlwaysVisible) },
            //        { MRZ.NormalizedFieldNames.CheckDigitGeneral,  new FieldProperties("Check digit general", FieldProperties.DisplayState.AlwaysVisible) },
            //    }
            //);

            var intent = GenericDocumentRecognizerActivity.NewIntent(this, configuration);
            StartActivityForResult(intent, GENERIC_DOCUMENT_REQUEST);
        }

        private void RecogniseCheck()
        {
            var config = new CheckRecognizerConfiguration();
            config.SetCancelButtonTitle("Done");
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

        private void TextDataRecognizerTapped()
        {
            // Launch the TextDataScanner UI
            var dataScannerStep = new TextDataScannerStep(
                 stepTag: "tag",
                 title: string.Empty,
                 guidanceText: string.Empty,
                 pattern: string.Empty,
                 shouldMatchSubstring: true,
                 validationCallback: new ValidationCallback(),
                 cleanRecognitionResultCallback: new RecognitionCallback(),
                 preferredZoom: 1.6f,
                 aspectRatio: new IO.Scanbot.Sdk.AspectRatio(4.0, 1.0),
                 unzoomedFinderHeight: 40f,
                 allowedSymbols: new List<Java.Lang.Character>(),
                 significantShakeDelay: 0);

            var config = new TextDataScannerConfiguration(dataScannerStep);
            config.SetCancelButtonTitle("Done");

            StartActivityForResult(TextDataScannerActivity.NewIntent(this, config), SCAN_DATA_REQUEST);
        }

        private void VinRecognizerTapped()
        {
            var configuration = new VinScannerConfiguration();
            configuration.SetCancelButtonTitle("Done");

            var intent = VinScannerActivity.NewIntent(this, configuration);
            StartActivityForResult(intent, SCAN_VIN_REQUEST);
        }

        private void LicensePlateRecognizerTapped()
        {
            var configuration = new LicensePlateScannerConfiguration();
            configuration.SetCancelButtonTitle("Done");
            configuration.SetTopBarButtonsColor(Color.Gray);
            configuration.SetTopBarBackgroundColor(Color.Black);

            var intent = LicensePlateScannerActivity.NewIntent(this, configuration);
            StartActivityForResult(intent, SCAN_EU_LICENSE_REQUEST);
        }

        private void ScanMedicalCertificate()
        {
            var configuration = new MedicalCertificateRecognizerConfiguration();
            configuration.SetTopBarBackgroundColor(Color.Black);

            var intent = MedicalCertificateRecognizerActivity.NewIntent(this, configuration);
            StartActivityForResult(intent, SCAN_MEDICAL_CERTIFICATE_REQUEST);
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
                case SCAN_DOCUMENT_REQUEST_CODE:
                    {
                        var parcelable = data.GetParcelableArrayExtra(RtuConstants.ExtraKeyRtuResult);
                        StartActivity(new Intent(this, typeof(PagePreviewActivity)));
                        return;
                    }
                case IMPORT_BARCODE_REQUEST:
                    {
                        var bitmap = Utils.ImageUtils.ProcessGalleryResult(this, data);
                        var detector = scanbotSDK.CreateBarcodeDetector();
                        var result = detector.DetectFromBitmap(bitmap, 0);

                        var qualityAnalyzer = scanbotSDK.CreateDocumentQualityAnalyzer();
                        var documentQualityResult = qualityAnalyzer.AnalyzeInBitmap(bitmap, 0);

                        var fragment = BarcodeDialogFragment.CreateInstance(result, documentQualityResult);
                        fragment.Show(FragmentManager);
                        return;
                    }
                case IMPORT_IMAGE_REQUEST:
                    {
                        progress.Visibility = ViewStates.Visible;

                        Alert.Toast(this, Texts.importing_and_processing);

                        var result = ImageUtils.ProcessGalleryResult(this, data);

                        var pageId = pageStorage.Add(result);
                        var page = new Page(pageId, new List<PointF>(), DetectionStatus.Ok, ImageFilterType.None);
                        page = scanbotSDK.CreatePageProcessor().DetectDocument(page);

                        progress.Visibility = ViewStates.Gone;

                        StartActivity(new Intent(this, typeof(PagePreviewActivity)));
                        return;
                    }
                case QR_BARCODE_DEFAULT_REQUEST:
                    {
                        var result = (BarcodeScanningResult)data.GetParcelableExtra(RtuConstants.ExtraKeyRtuResult);
                        var fragment = BarcodeDialogFragment.CreateInstance(result);
                        fragment.Show(FragmentManager);
                        return;
                    }
                case SCAN_MRZ_REQUEST:
                    {
                        var result = (MRZGenericDocument)data.GetParcelableExtra(RtuConstants.ExtraKeyRtuResult);
                        var fragment = MRZDialogFragment.CreateInstance(result);
                        fragment.Show(FragmentManager, MRZDialogFragment.NAME);
                        return;
                    }
                case GENERIC_DOCUMENT_REQUEST:
                    {
                        var resultsArray = data.GetParcelableArrayListExtra(RtuConstants.ExtraKeyRtuResult);
                        if (resultsArray?.Count == 0)
                        {
                            return;
                        }

                        var resultWrapper = (ResultWrapper)resultsArray[0];
                        var resultRepository = scanbotSDK.ResultRepositoryForClass(resultWrapper.Clazz);
                        var genericDocument = (GenericDocument)resultRepository.GetResultAndErase(resultWrapper.ResultId);
                        var fields = genericDocument.Fields.Cast<Field>().ToList();

                        var description = string.Join(";\n", fields
                            .Where(field => field != null)
                            .Select(field =>
                            {
                                string typeName = field.GetType().Name;
                                string valueText = field.Value?.Text;
                                return !string.IsNullOrEmpty(typeName) && !string.IsNullOrEmpty(valueText)
                                    ? $"{typeName} = {valueText}"
                                    : null;
                            })
                            .Where(outStr => outStr != null)
                            .ToList()
                        );

                        Alert.ShowAlert(this, "Result", description);
                        return;
                    }
                case SCAN_EHIC_REQUEST:
                    {
                        var result = (EhicRecognitionResult)data.GetParcelableExtra(RtuConstants.ExtraKeyRtuResult);
                        var fragment = HealthInsuranceCardFragment.CreateInstance(result);
                        fragment.Show(FragmentManager, HealthInsuranceCardFragment.NAME);
                        return;
                    }
                case SCAN_VIN_REQUEST:
                    {
                        var result = (VinScanResult)data.GetParcelableExtra(RtuConstants.ExtraKeyRtuResult);

                        Alert.Toast(this, $"VIN Scanned: {result.RawText}");
                        return;
                    }
                case SCAN_DATA_REQUEST:
                    {
                        var results = data.GetParcelableArrayExtra(RtuConstants.ExtraKeyRtuResult);
                        if (results == null || results.Length == 0)
                        {
                            return;
                        }
                        var textDataScannerStepResult = results.First() as TextDataScannerStepResult;
                        Alert.Toast(this, "Text Recognizer Result: " + textDataScannerStepResult.Text);
                        return;
                    }
                case SCAN_EU_LICENSE_REQUEST:
                    {
                        var results = data.GetParcelableArrayListExtra(RtuConstants.ExtraKeyRtuResult);

                        if (results == null || results.Count == 0)
                        {
                            return;
                        }

                        Alert.Toast(this, $"EU_LICENSE Scanned: {results[0]}");
                        return;
                    }
                case SCAN_MEDICAL_CERTIFICATE_REQUEST:
                    {
                        var resultWrapper = (ResultWrapper)data.GetParcelableExtra(RtuConstants.ExtraKeyRtuResult);
                        var resultRepository = scanbotSDK.ResultRepositoryForClass(resultWrapper.Clazz);
                        var checkResult = (MedicalCertificateRecognizerResult)resultRepository.GetResultAndErase(resultWrapper.ResultId);

                        var fragment = MedicalCertificateResultDialogFragment.CreateInstance(checkResult);
                        fragment.Show(FragmentManager, MedicalCertificateResultDialogFragment.NAME);
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

                        Alert.ShowAlert(this, "Result", description);
                        return;
                    }
                case BARCODE_DEFAULT_UI_REQUEST_CODE_V2:
                    {
                        if (data?.GetParcelableExtra(IO.Scanbot.Sdk.Ui_v2.Common.Activity.ActivityConstants.ExtraKeyRtuResult) is BarcodeScannerResult barcodeV2)
                        {
                            var imagePath = data.GetStringExtra(
                                IO.Scanbot.Sdk.Ui_v2.Barcode.BarcodeScannerActivity.ScannedBarcodeImagePathExtra);
                            var previewPath = data.GetStringExtra(
                                IO.Scanbot.Sdk.Ui_v2.Barcode.BarcodeScannerActivity.ScannedBarcodePreviewFramePathExtra);

                            var intent = new Intent(this, typeof(Activities.V2.BarcodeResultActivity));
                            var bundle = new BaseBarcodeResult<BarcodeScannerResult>(barcodeV2, imagePath, previewPath).ToBundle();
                            intent.PutExtra("BarcodeResult", bundle);

                            StartActivity(intent);
                        }
                        return;
                    }
            }
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

        private bool CheckLicense()
        {
            if (scanbotSDK.LicenseInfo.IsValid)
            {
                licenseIndicator.Visibility = ViewStates.Gone;
            }
            else
            {
                licenseIndicator.Visibility = ViewStates.Visible;
                licenseIndicator.Text = scanbotSDK.LicenseInfo.LicenseStatusMessage;
                Alert.Toast(this, "Invalid or missing license");
            }

            return scanbotSDK.LicenseInfo.IsValid;
        }
    }
}