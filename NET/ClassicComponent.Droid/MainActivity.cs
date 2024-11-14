using Android.Graphics;
using Android.Content;
using Java.Util;
using AndroidNetUri = Android.Net.Uri;
using Android.Util;
using IO.Scanbot.Sdk.Barcode.Entity;
using ClassicComponent.Droid.Activities;
using ClassicComponent.Droid.Utils;

using AndroidX.Core.Content;
using IO.Scanbot.Sdk.UI.View.Genericdocument;
using IO.Scanbot.Sdk.UI.Result;
using IO.Scanbot.Sdk.UI.View.Genericdocument.Configuration;
using IO.Scanbot.Genericdocument.Entity;
using IO.Scanbot.Sdk.UI.View.Base;
using IO.Scanbot.Sdk.Process;
using IO.Scanbot.Sdk.Tiff.Model;
using IO.Scanbot.Pdf.Model;
using IO.Scanbot.Sdk.Imagefilters;
using IO.Scanbot.Sdk.Persistence.Fileio;
using IO.Scanbot.Sdk.Util.Thread;
using IO.Scanbot.Sdk.Ocr;
using static IO.Scanbot.Sdk.Ocr.IOpticalCharacterRecognizer;

namespace ClassicComponent.Droid
{
    [Activity(Label = "NET Classic Component", MainLauncher = true, Icon = "@mipmap/icon",
              ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : Activity
    {
        static readonly string LOG_TAG = nameof(MainActivity);

        const int REQUEST_SB_SCANNING_UI = 4711;
        const int REQUEST_SB_CROPPING_UI = 4712;
        const int REQUEST_SYSTEM_GALLERY = 4713;
        const int REQUEST_SB_MRZ_SCANNER = 4714;
        const int REQUEST_SB_BARCODE_SCANNER = 4715;
        const int REQUEST_SB_GDR_SCANNING_UI = 4716;

        const int BIG_THUMB_MAX_W = 800, BIG_THUMB_MAX_H = 800;

        AndroidNetUri documentImageUri, originalImageUri;

        ImageView imageView;
        Button performOcrButton;

        private IO.Scanbot.Sdk.ScanbotSDK scanbotSDK;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            scanbotSDK = new IO.Scanbot.Sdk.ScanbotSDK(this);

            TempImageStorage.Init(MainApplication.USE_ENCRYPTION ? MainApplication.EncryptionFileIOProcessor : new DefaultFileIOProcessor(this));

            SetContentView(Resource.Layout.Main);

            imageView = FindViewById<ImageView>(Resource.Id.imageView);

            AssignCopyrightText();
            AssignStartCameraXButtonHandler();
            AssignStartGdrButtonHandler();
            AssingCroppingUIButtonHandler();
            AssignImportImageButtonHandler();
            AssignCreatePdfButtonHandler();
            AssignCreateTiffButtonHandler();
            AssignOcrButtonsHandler();
            AssignCheckRecognizerUiButtonHandler();

            PermissionUtils.Request(this, FindViewById(Resource.Layout.Main));
        }

        void AssignCopyrightText()
        {
            var copyrightTextView = FindViewById<TextView>(Resource.Id.copyrightTextView);
            copyrightTextView.Text = "Copyright (c) " + DateTime.Now.Year.ToString() + " Scanbot SDK GmbH. All rights reserved.";
        }

        void AssignStartCameraXButtonHandler()
        {
            var scanningCameraXUIButton = FindViewById<Button>(Resource.Id.scanningCameraXUIButton);
            scanningCameraXUIButton.Click += delegate
            {
                if (!CheckScanbotSDKLicense()) { return; }

                Intent intent = new Intent(this, typeof(CameraXViewDemoActivity));
                StartActivityForResult(intent, REQUEST_SB_SCANNING_UI);
            };
        }

        void AssignStartGdrButtonHandler()
        {
            var gdrUIButton = FindViewById<Button>(Resource.Id.gdrUiButton);
            gdrUIButton.Click += delegate
            {
                if (!CheckScanbotSDKLicense()) { return; }

                var configuration = new GenericDocumentRecognizerConfiguration();

                configuration.SetAcceptedDocumentTypes(new List<RootDocumentType>
                {
                    RootDocumentType.DeIdCardFront,
                    RootDocumentType.DeIdCardBack
                });

                Intent intent = GenericDocumentRecognizerActivity.NewIntent(this, configuration);

                StartActivityForResult(intent, REQUEST_SB_GDR_SCANNING_UI);
            };
        }

        void AssignImportImageButtonHandler()
        {
            var importImageButton = FindViewById<Button>(Resource.Id.importImageButton);
            importImageButton.Click += delegate
            {
                if (!CheckScanbotSDKLicense()) { return; }

                // Select an image from the Photo Library and run document detection on it (also see OnActivityResult)
                var imageIntent = new Intent();
                imageIntent.SetType("image/*");
                imageIntent.SetAction(Intent.ActionGetContent);
                imageIntent.PutExtra(Intent.ExtraLocalOnly, true);
                StartActivityForResult(Intent.CreateChooser(imageIntent, "Select photo"), REQUEST_SYSTEM_GALLERY);
            };
        }

        void AssingCroppingUIButtonHandler()
        {
            var croppingUIButton = FindViewById<Button>(Resource.Id.croppingUIButton);
            croppingUIButton.Click += delegate
            {
                if (!CheckScanbotSDKLicense()) { return; }
                if (!CheckOriginalImage()) { return; }

                Intent intent = new Intent(this, typeof(CroppingImageDemoActivity));
                intent.PutExtra(CroppingImageDemoActivity.EXTRAS_ARG_IMAGE_FILE_URI, documentImageUri?.ToString() ?? originalImageUri?.ToString());
                StartActivityForResult(intent, REQUEST_SB_CROPPING_UI);
            };
        }

        void AssignCreatePdfButtonHandler()
        {
            var createPdfButton = FindViewById<Button>(Resource.Id.createPdfButton);
            createPdfButton.Click += delegate
            {
                if (!CheckScanbotSDKLicense()) { return; }
                if (!CheckDocumentImage()) { return; }

                DebugLog("Starting PDF creation...");

                Task.Run(() =>
                {
                    try
                    {
                        var pdfOutputUri = GenerateRandomFileUrlInDemoTempStorage(".pdf");
                        var images = new AndroidNetUri[] { documentImageUri }; // add more images for PDF pages here
                        // The SDK call is sync!
                        var tempPdfFile = scanbotSDK.CreatePdfRenderer().Render(images, false, PdfConfig.DefaultConfig());
                        //  SBSDK.CreatePDF(images, pdfOutputUri, PDFPageSize.FixedA4);
                        File.Move(tempPdfFile.AbsolutePath, new Java.IO.File(pdfOutputUri.Path).AbsolutePath);
                        DebugLog("PDF file created: " + pdfOutputUri);
                        ShowAlertDialog("PDF file created: " + pdfOutputUri, onDismiss: () =>
                        {
                            OpenSharingDialog(pdfOutputUri);
                        });
                    }
                    catch (Exception e)
                    {
                        ErrorLog("Error creating PDF", e);
                    }
                });
            };
        }

        void AssignCreateTiffButtonHandler()
        {
            var createTiffButton = FindViewById<Button>(Resource.Id.createTiffButton);
            createTiffButton.Click += delegate
            {
                if (!CheckScanbotSDKLicense()) { return; }
                if (!CheckDocumentImage()) { return; }

                DebugLog("Starting TIFF creation...");

                Task.Run(() =>
                {
                    try
                    {
                        var tiffOutputUri = GenerateRandomFileInDemoTempStorage(".tiff");

                        // The SDK call is sync!
                        scanbotSDK.CreateTiffWriter().WriteTIFF(
                            new[] {  documentImageUri },
                            false,
                            new Java.IO.File(tiffOutputUri.Path),
                            TIFFImageWriterParameters.DefaultParameters());
                        DebugLog("TIFF file created: " + tiffOutputUri);
                        ShowAlertDialog("TIFF file created: " + tiffOutputUri, onDismiss: () =>
                        {
                            OpenSharingDialog(AndroidNetUri.FromFile(tiffOutputUri));
                        });
                    }
                    catch (Exception e)
                    {
                        ErrorLog("Error creating TIFF", e);
                    }
                });
            };
        }

        void AssignOcrButtonsHandler()
        {
            performOcrButton = FindViewById<Button>(Resource.Id.performOcrButton);
            performOcrButton.Click += delegate
            {
                if (!CheckScanbotSDKLicense()) { return; }
                if (!CheckDocumentImage()) { return; }

                performOcrButton.Post(() =>
                {
                    performOcrButton.Text = "Running OCR ... Please wait ...";
                    performOcrButton.Enabled = false;
                });

                Task.Run(() =>
                {
                    try
                    {
                        var pdfOutputUri = GenerateRandomFileUrlInDemoTempStorage(".pdf");
                        var images = new AndroidNetUri[] { documentImageUri }; // add more images for OCR here
                        
                        // This is the new OCR configuration with ML which doesn't require the langauges.

                        var recognitionMode = IOpticalCharacterRecognizer.EngineMode.ScanbotOcr;
                        var recognizer = scanbotSDK.CreateOcrRecognizer();

                        // to use legacy configuration we have to pass the installed languages.
                        if (recognitionMode == IOpticalCharacterRecognizer.EngineMode.Tesseract)
                        { 
                            var ocrConfig = new OcrConfig(IOpticalCharacterRecognizer.EngineMode.Tesseract, recognizer.InstalledLanguages);
                            recognizer.SetOcrConfig(ocrConfig);
                        }
                        else
                        {
                            recognizer.SetOcrConfig(new OcrConfig(IOpticalCharacterRecognizer.EngineMode.ScanbotOcr));
                        }

                        var ocrResult = recognizer.RecognizeTextWithPdfFromUris(
                            images.ToList(),
                            MainApplication.USE_ENCRYPTION,
                            PdfConfig.DefaultConfig());

                        File.Move(ocrResult.SandwichedPdfDocumentFile.AbsolutePath, new Java.IO.File(pdfOutputUri.Path).AbsolutePath);
                        DebugLog("Recognized OCR text: " + ocrResult.RecognizedText);
                        DebugLog("Sandwiched PDF file created: " + pdfOutputUri);
                        ShowAlertDialog(ocrResult.RecognizedText, "OCR Result", () =>
                        {
                            OpenSharingDialog(pdfOutputUri);
                        });
                    }
                    catch (Exception e)
                    {
                        ErrorLog("Error performing OCR", e);
                    }
                    finally
                    {
                        performOcrButton.Post(() =>
                        {
                            performOcrButton.Text = "Perform OCR";
                            performOcrButton.Enabled = true;
                        });
                    }
                });
            };
        }

        void AssignCheckRecognizerUiButtonHandler()
        {
            FindViewById<Button>(Resource.Id.checkUiButton).Click += delegate
            {
                StartActivity(CheckRecognizerDemoActivity.NewIntent(this));
            };
        }

        bool CheckDocumentImage()
        {
            if (documentImageUri == null)
            {
                Toast.MakeText(this, "Please snap a document image via Scanning UI or run Document Detection on an image file from the gallery", ToastLength.Long).Show();
                return false;
            }
            return true;
        }

        bool CheckOriginalImage()
        {
            if (originalImageUri == null)
            {
                Toast.MakeText(this, "Please snap a document image via Scanning UI or run Document Detection on an image file from the gallery", ToastLength.Long).Show();
                return false;
            }
            return true;
        }

        bool CheckScanbotSDKLicense()
        {
            if (scanbotSDK.LicenseInfo.IsValid)
            {
                // Trial period, valid trial license or valid production license.
                return true;
            }

            Toast.MakeText(this, "Scanbot SDK (trial) license has expired!", ToastLength.Long).Show();
            return false;
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == REQUEST_SB_SCANNING_UI && resultCode == Result.Ok)
            {
                documentImageUri = AndroidNetUri.Parse(data.GetStringExtra(CameraXViewDemoActivity.EXTRAS_ARG_DOC_IMAGE_FILE_URI));
                originalImageUri = AndroidNetUri.Parse(data.GetStringExtra(CameraXViewDemoActivity.EXTRAS_ARG_ORIGINAL_IMAGE_FILE_URI));
                ShowImageView(new ImageLoader(this).Load(documentImageUri));
                return;
            }

            if (requestCode == REQUEST_SB_CROPPING_UI && resultCode == Result.Ok)
            {
                documentImageUri = AndroidNetUri.Parse(data.GetStringExtra(CroppingImageDemoActivity.EXTRAS_ARG_IMAGE_FILE_URI));
                ShowImageView(new ImageLoader(this).Load(documentImageUri));
                return;
            }

            if (requestCode == REQUEST_SYSTEM_GALLERY && resultCode == Result.Ok)
            {
                // An image was imported from the Photo Library. Run document detection on it and show the cropped document image result.
                originalImageUri = data.Data;
                RunDocumentDetection(originalImageUri);
                return;
            }

            if (requestCode == REQUEST_SB_BARCODE_SCANNER && resultCode == Result.Ok)
            {
                var barcodeResult = data.GetParcelableExtra(RtuConstants.ExtraKeyRtuResult) as BarcodeScanningResult;
                var barcode = barcodeResult.BarcodeItems[0];
                Toast.MakeText(this, barcode.BarcodeFormat + "\n" + barcode.Text, ToastLength.Long).Show();
                return;
            }

            if (requestCode == REQUEST_SB_GDR_SCANNING_UI && resultCode == Result.Ok)
            {
                var resultsArray = data.GetParcelableArrayListExtra(RtuConstants.ExtraKeyRtuResult);
                if (resultsArray.Count == 0)
                {
                    return;
                }

                var resultWrapper = (ResultWrapper)resultsArray[0];
                var resultRepository = new IO.Scanbot.Sdk.ScanbotSDK(this).ResultRepositoryForClass(resultWrapper.Clazz);
                var genericDocument = (IO.Scanbot.Genericdocument.Entity.GenericDocument)resultRepository.GetResultAndErase(resultWrapper.ResultId);
                var fields = genericDocument.Fields.Cast<IO.Scanbot.Genericdocument.Entity.Field>().ToList();
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

                ShowAlert("Result", description);

                Console.WriteLine("GDR Result: ", description);
            }

        }

        void RunDocumentDetection(AndroidNetUri imageUri)
        {
            DebugLog("Running document detection on image: " + imageUri);

            Task.Run(() =>
            {
                try
                {
                    // The SDK call is sync!
                    var image = new ImageLoader(this).LoadFromMedia(imageUri);

                    var detectionResult = scanbotSDK.CreateContourDetector().Detect(image);

                    DebugLog("Document detection result: " + detectionResult.Status);
                    if (detectionResult.Status == IO.Scanbot.Sdk.Core.Contourdetector.DocumentDetectionStatus.Ok ||
                        detectionResult.Status == IO.Scanbot.Sdk.Core.Contourdetector.DocumentDetectionStatus.OkButTooSmall)
                    {
                        var documentImage = image;

                        if (detectionResult.PolygonF != null)
                        {
                            documentImage = new ImageProcessor(image).Crop(detectionResult.PolygonF).ProcessedBitmap();
                        }

                        documentImageUri = TempImageStorage.Instance.AddImage(documentImage);
                        ShowImageView(documentImage);

                        DebugLog("Detected polygon: ");
                        foreach (var p in detectionResult.Polygon)
                        {
                            DebugLog(p.ToString());
                        }
                    }
                    else
                    {
                        DebugLog("No document detected!");
                        RunOnUiThread(() =>
                        {
                            Toast.MakeText(this, "No document detected! (Detection result: " + detectionResult.Status + ")", ToastLength.Long).Show();
                        });
                    }
                }
                catch (Exception e)
                {
                    ErrorLog("Error while document detection", e);
                }
            });
        }


        void ShowImageView(Bitmap bitmap)
        {
            imageView.Post(() =>
            {
                var thumb = ImageUtils.GetThumbnail(bitmap, BIG_THUMB_MAX_W, BIG_THUMB_MAX_H);
                imageView.SetImageBitmap(thumb);
            });
        }


        Java.IO.File GenerateRandomFileInDemoTempStorage(string fileExtension)
        {
            var targetFile = System.IO.Path.Combine(
                TempImageStorage.Instance.TempDir, UUID.RandomUUID() + fileExtension);
            return new Java.IO.File(targetFile);
        }

        AndroidNetUri GenerateRandomFileUrlInDemoTempStorage(string fileExtension)
        {
            return AndroidNetUri.FromFile(GenerateRandomFileInDemoTempStorage(fileExtension));
        }

        void ShowAlertDialog(string message, string title = "Info", Action onDismiss = null)
        {
            RunOnUiThread(() =>
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle(title);
                builder.SetMessage(message);
                var alert = builder.Create();
                alert.SetButton("OK", (c, ev) =>
                {
                    alert.Dismiss();
                    onDismiss?.Invoke();
                });
                alert.Show();
            });
        }
        private const string SNAPPING_DOCUMENTS_DIR_NAME = "snapping_documents";

        public static Java.IO.File Copy(Context context, Android.Net.Uri uri)
        {
            var path = System.IO.Path.Combine(context.GetExternalFilesDir(null).AbsolutePath, SNAPPING_DOCUMENTS_DIR_NAME);
            var file = System.IO.Path.Combine(path, uri.LastPathSegment);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            File.Copy(uri.Path, file);

            return new Java.IO.File(file);
        }

        void OpenSharingDialog(AndroidNetUri publicFileUri)
        {
            // Please note: To be able to share a file on Android it must be in a public folder. 
            // If you need a secure place to store PDF, TIFF, etc, do NOT use this sharing solution!
            // Also see the initialization of the TempImageStorage in the MainApplication class.

            Java.IO.File file = Copy(this, publicFileUri);

            var shareIntent = new Intent(Intent.ActionSend);

            var authority = ApplicationContext.PackageName + ".provider";
            var uri = FileProvider.GetUriForFile(this, authority, file);

            shareIntent.SetDataAndType(uri, MimeUtils.GetMimeByName(file.Name));
            shareIntent.SetFlags(ActivityFlags.ClearWhenTaskReset | ActivityFlags.NewTask);
            shareIntent.AddFlags(ActivityFlags.GrantReadUriPermission | ActivityFlags.GrantWriteUriPermission);

            shareIntent.PutExtra(Intent.ExtraStream, uri);
            StartActivity(shareIntent);
        }

        void DebugLog(string msg)
        {
            Log.Debug(LOG_TAG, msg);
        }

        void ErrorLog(string msg, Exception ex)
        {
            Log.Error(LOG_TAG, Java.Lang.Throwable.FromException(ex), msg);
        }

        void ShowAlert(string title, string message)
        {
            var dialog = new AlertDialog.Builder(this);
            AlertDialog alert = dialog.Create();
            alert.SetTitle(title);
            alert.SetMessage(message);
            alert.SetButton((int)DialogButtonType.Neutral, "OK", (c, ev) =>
            {
                alert.Dismiss();
            });
            alert.Show();
        }


        [Obsolete]
        class ImageFilterDialog : DialogFragment
        {
            static List<string> ImageFilterItems = new List<string>();

            static ImageFilterDialog()
            {
                foreach (var filter in ImageFilterType.Values())
                {
                    if (filter.ToString().ToLower() == "none") { continue; }
                    ImageFilterItems.Add(filter.ToString());
                }
            }

            Action<LegacyFilter> ApplyFilterAction;

            internal ImageFilterDialog(Action<LegacyFilter> applyFilterAction)
            {
                ApplyFilterAction = applyFilterAction;
            }

            public override Dialog OnCreateDialog(Bundle savedInstanceState)
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(Activity);
                builder.SetTitle("Pick an Image Filter");
                builder.SetItems(ImageFilterItems.ToArray(), (sender, args) =>
                {
                    var filterName = ImageFilterItems[args.Which];
                    var filter = ImageFilterType.Values().FirstOrDefault(f => f.ToString() == filterName);
                    ApplyFilterAction?.Invoke(new LegacyFilter(filter.Code));
                });

                return builder.Create();
            }
        }
    }
}