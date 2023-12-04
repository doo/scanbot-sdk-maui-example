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
using Android.Graphics.Pdf;
using IO.Scanbot.Sdk.Tiff.Model;
using static Android.Graphics.BitmapFactory;
using IO.Scanbot.Sdk.Entity;
using Android.Views;
using IO.Scanbot.Sdk;
using IO.Scanbot.Sdk.Camera;
using IO.Scanbot.Sdk.Check;
using IO.Scanbot.Sdk.Contourdetector;
using IO.Scanbot.Sdk.UI.Camera;
using IO.Scanbot.Sdk.Check.Entity;

//using ScanbotSDK.MAUI.Constants;
//using SBSDK = ScanbotSDK.MAUI.Native.Droid.ScanbotSDK;
//using ScanbotSDK.MAUI.Models;

namespace ClassicComponent.Droid
{
    [Activity(Label = "NET Classic Component", MainLauncher = true, Icon = "@mipmap/icon",
              ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : Activity
    {
        static readonly string LOG_TAG = typeof(MainActivity).Name;

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

            SetContentView(Resource.Layout.Main);

            imageView = FindViewById<ImageView>(Resource.Id.imageView);

            AssignCopyrightText();
            AssignStartCameraXButtonHandler();
            AssignStartGdrButtonHandler();
            AssingCroppingUIButtonHandler();
            AssignApplyImageFilterButtonHandler();
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
            copyrightTextView.Text = "Copyright (c) " + DateTime.Now.Year.ToString() + " doo GmbH. All rights reserved.";
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

        void AssignApplyImageFilterButtonHandler()
        {
            var applyImageFilterButton = FindViewById<Button>(Resource.Id.applyImageFilterButton);
            applyImageFilterButton.Click += delegate
            {
                if (!CheckScanbotSDKLicense()) { return; }
                if (!CheckDocumentImage()) { return; }

                var transaction = FragmentManager.BeginTransaction();
                var dialogFragment = new ImageFilterDialog(ApplyImageFilter);
                dialogFragment.Show(transaction, "ImageFilterDialog");
            };
        }

        void ApplyImageFilter(ImageFilterType filter)
        {
            DebugLog("Applying image filter " + filter + " on image: " + documentImageUri);
            try
            {
                Task.Run(() =>
                {
                    // The SDK call is sync!
                    // TODO load uri
                    Bitmap documentImage = null; // documentImageUri
                    var resultImage = scanbotSDK.ImageProcessor().ProcessBitmap(documentImage, new FilterOperation(filter), false);
                   // var resultImage = SBSDK.ApplyImageFilter(documentImageUri, filter);
                    documentImageUri = TempImageStorage.Instance.AddImage(resultImage);
                    ShowImageView(resultImage);
                });
            }
            catch (Exception e)
            {
                ErrorLog("Error applying image filter", e);
            }
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
                intent.PutExtra(CroppingImageDemoActivity.EXTRAS_ARG_IMAGE_FILE_URI, originalImageUri.ToString());
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
                        var tempPdfFile = scanbotSDK.CreatePdfRenderer().RenderDocumentFromImages(images, false, PDFPageSize.FixedA4);
                        //  SBSDK.CreatePDF(images, pdfOutputUri, PDFPageSize.FixedA4);
                        File.Move(tempPdfFile.AbsolutePath, new Java.IO.File(pdfOutputUri.Path).AbsolutePath);
                        DebugLog("PDF file created: " + pdfOutputUri);
                        ShowAlertDialog("PDF file created: " + pdfOutputUri, onDismiss: () =>
                        {
                            OpenSharingDialog(pdfOutputUri, "application/pdf");
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
                        scanbotSDK.CreateTiffWriter().WriteTIFFFromFiles(
                            new[] {new Java.IO.File(documentImageUri.Path) }.ToList(),
                            false,
                            new Java.IO.File(tiffOutputUri.Path),
                            TIFFImageWriterParameters.DefaultParameters());
                        DebugLog("TIFF file created: " + tiffOutputUri);
                        ShowAlertDialog("TIFF file created: " + tiffOutputUri, onDismiss: () =>
                        {
                            OpenSharingDialog(AndroidNetUri.FromFile(tiffOutputUri), "image/tiff");
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

                        var ocrResult = scanbotSDK.CreateOcrRecognizer().RecognizeTextWithPdfFromUris(
                            images.ToList(),
                            false, PDFPageSize.FromImage,
                            new[] { Language.LanguageByLanguageTag("en"), Language.LanguageByLanguageTag("de") } );


                        var tempPdfFile = ocrResult.SandwichedPdfDocumentFile;
                        File.Move(tempPdfFile.AbsolutePath, new Java.IO.File(pdfOutputUri.Path).AbsolutePath);
                        DebugLog("Recognized OCR text: " + ocrResult.RecognizedText);
                        DebugLog("Sandwiched PDF file created: " + pdfOutputUri);
                        ShowAlertDialog(ocrResult.RecognizedText, "OCR Result", () =>
                        {
                            OpenSharingDialog(pdfOutputUri, "application/pdf");
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
                ShowImageView(ImageLoader.Instance.Load(documentImageUri));
                return;
            }

            if (requestCode == REQUEST_SB_CROPPING_UI && resultCode == Result.Ok)
            {
                documentImageUri = AndroidNetUri.Parse(data.GetStringExtra(CroppingImageDemoActivity.EXTRAS_ARG_IMAGE_FILE_URI));
                ShowImageView(ImageLoader.Instance.Load(documentImageUri));
                return;
            }

            if (requestCode == REQUEST_SYSTEM_GALLERY && resultCode == Result.Ok)
            {
                // An image was imported from the Photo Library. Run document detection on it and show the cropped document image result.
                originalImageUri = data.Data;
                RunDocumentDetection(originalImageUri);
                return;
            }

            if (requestCode == REQUEST_SB_MRZ_SCANNER && resultCode == Result.Ok)
            {
                //TODO: Have to FIX MRZ Result
                //var mrzRecognitionResult = data.GetParcelableExtra(RtuConstants.ExtraKeyRtuResult) as MRZRecognitionResult;
                //Toast.MakeText(this, ExtractMrzResultData(mrzRecognitionResult), ToastLength.Long).Show();
                //return;
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

        // TODO Have to fix MRZResult
        //string ExtractMrzResultData(MRZRecognitionResult result)
        //{
        //    return new System.Text.StringBuilder()
        //            .Append("documentCode: ").Append(result.DocumentCodeField().Value).Append("\n")
        //            .Append("First name: ").Append(result.FirstNameField().Value).Append("\n")
        //            .Append("Last name: ").Append(result.LastNameField().Value).Append("\n")
        //            .Append("issuingStateOrOrganization: ").Append(result.IssuingStateOrOrganizationField().Value).Append("\n")
        //            .Append("departmentOfIssuance: ").Append(result.DepartmentOfIssuanceField().Value).Append("\n")
        //            .Append("nationality: ").Append(result.NationalityField().Value).Append("\n")
        //            .Append("dateOfBirth: ").Append(result.DateOfBirthField().Value).Append("\n")
        //            .Append("gender: ").Append(result.GenderField().Value).Append("\n")
        //            .Append("dateOfExpiry: ").Append(result.DateOfExpiryField().Value).Append("\n")
        //            .Append("personalNumber: ").Append(result.PersonalNumberField().Value).Append("\n")
        //            .Append("optional1: ").Append(result.Optional1Field().Value).Append("\n")
        //            .Append("optional2: ").Append(result.Optional2Field().Value).Append("\n")
        //            .Append("discreetIssuingStateOrOrganization: ").Append(result.DiscreetIssuingStateOrOrganizationField().Value).Append("\n")
        //            .Append("validCheckDigitsCount: ").Append(result.ValidCheckDigitsCount).Append("\n")
        //            .Append("checkDigitsCount: ").Append(result.CheckDigitsCount).Append("\n")
        //            .Append("travelDocType: ").Append(result.TravelDocTypeField().Value).Append("\n")
        //            .ToString();
        //}

        void RunDocumentDetection(AndroidNetUri imageUri)
        {
            DebugLog("Running document detection on image: " + imageUri);

            Task.Run(() =>
            {
                try
                {
                    // The SDK call is sync!
                    var image = ImageLoader.Instance.LoadFromMedia(imageUri);

                    var detectionResult = scanbotSDK.CreateContourDetector().Detect(image);

                    DebugLog("Document detection result: " + detectionResult.Status);
                    if (detectionResult.Status == IO.Scanbot.Sdk.Core.Contourdetector.DetectionStatus.Ok)
                    {
                        var documentImage = image;

                        if (detectionResult.PolygonF != null)
                        {
                            documentImage = scanbotSDK.ImageProcessor().ProcessBitmap(image, new CropOperation(detectionResult.PolygonF), false);
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


        void OpenSharingDialog(AndroidNetUri publicFileUri, string mimeType)
        {
            // Please note: To be able to share a file on Android it must be in a public folder. 
            // If you need a secure place to store PDF, TIFF, etc, do NOT use this sharing solution!
            // Also see the initialization of the TempImageStorage in the MainApplication class.

            Intent shareIntent = new Intent(Intent.ActionSend);
            shareIntent.SetType(mimeType);
            shareIntent.SetFlags(ActivityFlags.ClearWhenTaskReset | ActivityFlags.NewTask);
            shareIntent.AddFlags(ActivityFlags.GrantReadUriPermission | ActivityFlags.GrantWriteUriPermission);

            var authority = ApplicationContext.PackageName + ".provider";
            var uri = FileProvider.GetUriForFile(this, authority, new Java.IO.File(publicFileUri.Path));

            shareIntent.PutExtra(Intent.ExtraStream, uri);
            StartActivity(shareIntent);
        }


        void OpenPDFFile(AndroidNetUri publicPdfFileUri)
        {
            Intent openIntent = new Intent();
            openIntent.SetAction(Intent.ActionView);
            openIntent.SetDataAndType(publicPdfFileUri, "application/pdf");
            openIntent.SetFlags(ActivityFlags.ClearWhenTaskReset | ActivityFlags.NewTask);

            if (openIntent.ResolveActivity(this.PackageManager) != null)
            {
                StartActivity(openIntent);
            }
            else
            {
                RunOnUiThread(() =>
                {
                    Toast.MakeText(this, "Error opening PDF document", ToastLength.Long).Show();
                });
            }
        }


        void DebugLog(string msg)
        {
            Log.Debug(LOG_TAG, msg);
        }

        void ErrorLog(string msg)
        {
            Log.Error(LOG_TAG, msg);
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

            Action<ImageFilterType> ApplyFilterAction;

            internal ImageFilterDialog(Action<ImageFilterType> applyFilterAction)
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
                    ApplyFilterAction?.Invoke(filter);
                });

                return builder.Create();
            }
        }
    }
}