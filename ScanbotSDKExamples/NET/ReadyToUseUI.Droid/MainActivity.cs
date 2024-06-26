using Android.Views;
using ReadyToUseUI.Droid.Fragments;
using Android.Graphics;
using Android.Content;
using Android.Runtime;
using IO.Scanbot.Sdk.Persistence;
using ReadyToUseUI.Droid.Activities;
using ReadyToUseUI.Droid.Utils;
using IO.Scanbot.Sdk.Process;
using IO.Scanbot.Sdk.Barcode.Entity;
using IO.Scanbot.Ehicscanner.Model;
using IO.Scanbot.Sdk.Core.Contourdetector;
using IO.Scanbot.Genericdocument.Entity;
using IO.Scanbot.Sdk.UI.Result;
using IO.Scanbot.Sdk.UI.View.Base;
using IO.Scanbot.Sdk.Check.Entity;
using DocumentSDK.NET.Model;
using IO.Scanbot.Mrzscanner.Model;
using IO.Scanbot.Sdk.UI.View.Generictext.Entity;
using IO.Scanbot.Sdk.Mcrecognizer.Entity;
using IO.Scanbot.Sdk.Ui_v2.Barcode.Common.Mappers;
using IO.Scanbot.Sdk.Vin;
using IO.Scanbot.Sdk.Ui_v2.Barcode.Configuration;

namespace ReadyToUseUI.Droid
{
    [Activity(Label = "NET RTU UI", MainLauncher = true, Icon = "@mipmap/icon")]
    public partial class MainActivity : AndroidX.AppCompat.App.AppCompatActivity, IBarcodeItemMapper
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
            var barcodeScannerTitle = "Barcode Scanner";
            
#if LEGACY_EXAMPLES
            barcodeDetectorsTitle.Text = barcodeScannerTitle + " V1";
            barcodeDetectors.AddChildren(buttons, new[]
            {
                new ListItemButton(this, "Scan Barcodes", ScanBarcode),
                new ListItemButton(this, "Scan Batch Barcodes", ScanBarcodesInBatch),
                new ListItemButton(this, "Import and Detect Barcodes", ImportAndDetectBarcode),
            });
            barcodeScannerTitle += " V2";
#else
            barcodeDetectorsTitle.Visibility = ViewStates.Gone;
            barcodeDetectors.Visibility = ViewStates.Gone;
#endif

            var barcodeDetectorV2 = (LinearLayout)container.FindViewById(Resource.Id.barcode_data_scanner_v2);
            var barcodeDetectorV2Title = (TextView)barcodeDetectorV2.FindViewById(Resource.Id.textView);
            barcodeDetectorV2Title.Text = barcodeScannerTitle;
            barcodeDetectorV2.AddChildren(buttons, new[]
            {
                new ListItemButton(this, "Single Barcode Scanning", SingleScanning),
                new ListItemButton(this, "Single Barcode Scanning - AR Overlay", SingleScanningWithArOverlay),
                new ListItemButton(this, "Multiple Unique Barcode Scanning", BatchBarcodeScanning),
                new ListItemButton(this, "Find and Pick Barcode Scanning", FindAndPickScanning),
            });

            var scanner = (LinearLayout)container.FindViewById(Resource.Id.document_scanner);
            var scannerTitle = (TextView)scanner.FindViewById(Resource.Id.textView);
            scannerTitle.Text = "Document Scanner";
            scanner.AddChildren(buttons, new[]
            {
                new ListItemButton(this, "Scan Document", ScanDocument),
                new ListItemButton(this, "Scan Document with Finder", ScanDocumentWithFinder),
                new ListItemButton(this, "Import Image", ImportImage),
                new ListItemButton(this, "View Images", ViewImages)
            });

            var detectors = (LinearLayout)container.FindViewById(Resource.Id.data_detectors);
            var detectorsTitle = (TextView)detectors.FindViewById(Resource.Id.textView);
            detectorsTitle.Text = "Data Detectors";
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
        
        public void MapBarcodeItem(IO.Scanbot.Sdk.Ui_v2.Barcode.Configuration.BarcodeItem barcodeItem, IBarcodeMappingResult result)
        {
            result.OnResult(new BarcodeMappedData(
                title: barcodeItem.TextWithExtension,
                subtitle: barcodeItem.Type.Name(),
                barcodeImage: BarcodeMappedDataExtension.BarcodeFormatKey));
        }
    }
}