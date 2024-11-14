using Android.Views;
using ReadyToUseUI.Droid.Fragments;
using Android.Graphics;
using Android.Content;
using Android.Runtime;
using AndroidX.Activity.Result.Contract;
using IO.Scanbot.Sdk.Persistence.Page.Legacy;
using ReadyToUseUI.Droid.Activities;
using ReadyToUseUI.Droid.Utils;
using IO.Scanbot.Sdk.Process;
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
using IO.Scanbot.Sdk.Vin;
using IO.Scanbot.Sdk.UI.View.Licenseplate.Entity;

namespace ReadyToUseUI.Droid
{
    [Activity(Label = "NET RTU UI", MainLauncher = true, Icon = "@mipmap/icon")]
    public partial class MainActivity : AndroidX.AppCompat.App.AppCompatActivity
    {
        private const int SCAN_DOCUMENT_REQUEST_CODE = 1000;

        private const int IMPORT_IMAGE_REQUEST = 2001;

        private const int SCAN_MRZ_REQUEST = 4001;
        private const int GENERIC_DOCUMENT_REQUEST = 4002;
        private const int SCAN_DATA_REQUEST = 4003;
        private const int SCAN_VIN_REQUEST = 4004;
        private const int SCAN_EU_LICENSE_REQUEST = 4005;
        private const int SCAN_EHIC_REQUEST = 4006;
        private const int SCAN_MEDICAL_CERTIFICATE_REQUEST = 4007;
        private const int CHECK_RECOGNIZER_REQUEST = 5001;
        
        private const int DETECT_MRZ_FROM_IMAGE = 6001;
        private const int DETECT_EHIC_FROM_IMAGE = 6002;
        private const int DETECT_MEDICAL_CERTIFICATE_FROM_IMAGE = 6003;
        private const int DETECT_CHECK_FROM_IMAGE = 6004;
        private const int DETECT_GDR_FROM_IMAGE = 6005;

        private readonly List<ListItemButton> buttons = new List<ListItemButton>();
        private ProgressBar progress;

        private IO.Scanbot.Sdk.ScanbotSDK scanbotSDK;
        // private PageFileStorage pageFileStorage;
        private TextView licenseIndicator;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            scanbotSDK = new IO.Scanbot.Sdk.ScanbotSDK(this);
            // pageFileStorage = scanbotSDK.CreatePageFileStorage();

            var container = (LinearLayout)FindViewById(Resource.Id.container);
            var title = container.FindViewById<TextView>(Resource.Id.textView);
            title.Text = Texts.scanbot_sdk_demo;
          
            var scanner = (LinearLayout)container.FindViewById(Resource.Id.document_scanner);
            var scannerTitle = (TextView)scanner.FindViewById(Resource.Id.textView);
            scannerTitle.Text = "Document Scanner";
            scanner.AddChildren(buttons, new[]
            {
                new ListItemButton(this, "Single Document Scanning", SingleDocumentScanning),
                new ListItemButton(this, "Single Finder Document Scanning", SingleFinderDocumentScanning),
                new ListItemButton(this, "Multiple Document Scanning", MultipleDocumentScanning),
                new ListItemButton(this, "Import Image", ImportImage)
            });

            var detectors = (LinearLayout)container.FindViewById(Resource.Id.data_detectors);
            var detectorsTitle = (TextView)detectors.FindViewById(Resource.Id.textView);
            detectorsTitle.Text = "Data Detectors";
            detectors.AddChildren(buttons, new[]
            {
                new ListItemButton(this, "Scan MRZ", ScanMrz),
                new ListItemButton(this, "Scan Health Insurance card", ScanEhic),
                new ListItemButton(this, "Generic Document Recognizer", RecongnizeGenericDocument),
                new ListItemButton(this, "Check Recognizer", RecognizeCheck),
                new ListItemButton(this, "Text Data Recognizer", TextDataRecognizerTapped),
                new ListItemButton(this, "VIN Recognizer", VinRecognizerTapped),
                new ListItemButton(this, "License Plate Recognizer", LicensePlateRecognizerTapped),
                new ListItemButton(this, "Medical Certificate Recognizer", ScanMedicalCertificate),
            });

            var detectionOnImage = (LinearLayout)container.FindViewById(Resource.Id.data_detection_on_image);
            var detectionOnImageTitle = (TextView)detectionOnImage.FindViewById(Resource.Id.textView);
            detectionOnImageTitle.Text = "Data Detection On Image";
            detectionOnImage.AddChildren(buttons, new[]
            {
                new ListItemButton(this, "Detect MRZ from Image", () => LaunchImagePicker(DETECT_MRZ_FROM_IMAGE)),
                new ListItemButton(this, "Detect EHIC from Image", () => LaunchImagePicker(DETECT_EHIC_FROM_IMAGE)),
                new ListItemButton(this, "Detect Generic Document from Image", () => LaunchImagePicker(DETECT_GDR_FROM_IMAGE)),
                new ListItemButton(this, "Detect Check from Image", () => LaunchImagePicker(DETECT_CHECK_FROM_IMAGE)),
                new ListItemButton(this, "Detect Medical Certificate from Image", () => LaunchImagePicker(DETECT_MEDICAL_CERTIFICATE_FROM_IMAGE))
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
                        var documentId = data?.GetStringExtra(IO.Scanbot.Sdk.Ui_v2.Common.Activity.ActivityConstants.ExtraKeyRtuResult) as string;
                        var intent = PagePreviewActivity.CreateIntent(this, documentId);
                        StartActivity(intent);
                        return;
                }
                case IMPORT_IMAGE_REQUEST:
                {
                    progress.Visibility = ViewStates.Visible;

                    Alert.Toast(this, Texts.importing_and_processing);

                    var bitmap = ImageUtils.ProcessGalleryResult(this, data);

                    var detector = scanbotSDK.CreateContourDetector();
                    var detectionResult = detector.Detect(bitmap);
                    
                    var defaultDocumentSizeLimit = 0;
                    var document = scanbotSDK.DocumentApi.CreateDocument(defaultDocumentSizeLimit);
                    document.AddPage(bitmap);

                    if (detectionResult != null)
                    {
                        document.PageAtIndex(0).Polygon = detectionResult.PolygonF;
                    }

                    progress.Visibility = ViewStates.Gone;

                    var intent = PagePreviewActivity.CreateIntent(this, document.Uuid);
                    StartActivity(intent);
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

                    Alert.Toast(this, $"VIN Scanned: {result?.RawText}");
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
                    var result = (data.GetParcelableExtra(RtuConstants.ExtraKeyRtuResult) as LicensePlateScannerResult);

                    if (result == null)
                    {
                        return;
                    }

                    Alert.Toast(this, $"EU_LICENSE Scanned: {result.RawText}");
                    return;
                }
                case SCAN_MEDICAL_CERTIFICATE_REQUEST:
                {
                    var resultWrapper = (ResultWrapper)data.GetParcelableExtra(RtuConstants.ExtraKeyRtuResult);
                    var resultRepository = scanbotSDK.ResultRepositoryForClass(resultWrapper.Clazz);
                    var result = (MedicalCertificateRecognizerResult)resultRepository.GetResultAndErase(resultWrapper.ResultId);

                    var fragment = MedicalCertificateResultDialogFragment.CreateInstance(result);
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
                case DETECT_MRZ_FROM_IMAGE:
                    DetectMrzFromImage(data);
                    return;
                case DETECT_EHIC_FROM_IMAGE:
                    DetectEHICFromImage(data);
                    return;
                case DETECT_CHECK_FROM_IMAGE:
                    DetectCheckFromImage(data);
                    return;
                case DETECT_MEDICAL_CERTIFICATE_FROM_IMAGE:
                    DetectMedicalCertificateFromImage(data);
                    return;
                case DETECT_GDR_FROM_IMAGE:
                    DetectGenericDocumentFromImage(data);
                    return;
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

        private void LaunchImagePicker(int activityRequestCode)
        {
                var intent = new Intent();
                intent.SetType("image/*");
                intent.SetAction(Intent.ActionGetContent);
                intent.PutExtra(Intent.ExtraLocalOnly, false);
                intent.PutExtra(Intent.ExtraAllowMultiple, false);

                var chooser = Intent.CreateChooser(intent, Texts.share_title);
                StartActivityForResult(chooser, activityRequestCode);
        }
    }
}