using Android.Content;
using IO.Scanbot.Sdk.Check;
using IO.Scanbot.Sdk.Creditcard;
using IO.Scanbot.Sdk.Document;
using ScanbotSdkExample.Droid.Model;
using IO.Scanbot.Sdk.Documentdata;
using IO.Scanbot.Sdk.Ehicscanner;
using IO.Scanbot.Sdk.MC;
using ScanbotSdkExample.Droid.Fragments;
using ScanbotSdkExample.Droid.Utils;
using ScanbotSdkExample.Droid.Views;

namespace ScanbotSdkExample.Droid;

public partial class MainActivity
{
    private Dictionary<int, Action<Intent>> DetectOnImageActions => new Dictionary<int, Action<Intent>>
    {
        { DetectMrzFromImageCode, RecognizeMrzFromImage },
        { DetectEhicFromImageCode, RecognizeEhicFromImage },
        { DetectCheckFromImageCode, RecognizeCheckFromImage },
        { DetectMedicalCertificateFromImageCode, RecognizeMedicalCertificateFromImage },
        { ExtractDocumentDataFromImageCode, ExtractDocumentDataFromImage },
        { DetectCreditCardFromImageCode, RecognizeCreditCardFromImage },
    };

    private void LaunchImagePicker(int activityRequestCode)
    {
        var intent = new Intent();
        intent.SetType("image/*");
        intent.SetAction(Intent.ActionGetContent);
        intent.PutExtra(Intent.ExtraLocalOnly, false);
        intent.PutExtra(Intent.ExtraAllowMultiple, false);

        var chooser = Intent.CreateChooser(intent, Texts.ShareTitle);
        StartActivityForResult(chooser, activityRequestCode);
    }

    private void RecognizeMrzFromImage(Intent data)
    {
        var bitmap = ImageUtils.ProcessGalleryResult(this, data);
        var recognizer = _scanbotSdk.CreateMrzScanner();
        var result = recognizer.ScanFromBitmap(bitmap, 0);

        if (result?.Document == null || !result.Success)
        {
            Alert.Show(this, "Error", "Unable to detect the MRZ.");
            return;
        }

        var fragment = MRZDialogFragment.CreateInstance(result.Document);
        ShowFragment(fragment, MRZDialogFragment.Name);
    }

    private void RecognizeCheckFromImage(Intent data)
    {
        var bitmap = ImageUtils.ProcessGalleryResult(this, data);
        var recognizer = _scanbotSdk.CreateCheckScanner();
        var result = recognizer.ScanFromBitmap(bitmap, 0);
        
        if (result?.Check == null || result.Status != CheckMagneticInkStripScanningStatus.Success)
        {
            Alert.Show(this, "Error", "Unable to detect the Check.");
            return;
        }
        
        var description = result.Check.ToFormattedString();
        Alert.Show(this, "Result", description);
    }
    
    private void RecognizeMedicalCertificateFromImage(Intent data)
    {
        var bitmap = ImageUtils.ProcessGalleryResult(this, data);
        var recognizer = _scanbotSdk.CreateMedicalCertificateScanner();
        
        var parameters = new MedicalCertificateScanningParameters(
            shouldCropDocument: true,
            recognizePatientInfoBox: true,
            recognizeBarcode: true,
            extractCroppedImage: true,
            preprocessInput: true);
        
        var result = recognizer.ScanFromBitmap(image: bitmap,
            orientation: 0,
            parameters: parameters);

        if (result == null || !result.ScanningSuccessful || result.DocumentDetectionResult.Status != DocumentDetectionStatus.Ok)
        {
            Alert.Show(this, "Error", "Unable to detect the Medical certificate.");
            return;
        }

        var fragment = MedicalCertificateResultDialogFragment.CreateInstance(result);
        ShowFragment(fragment, MedicalCertificateResultDialogFragment.Name);
    }

    private void RecognizeEhicFromImage(Intent data)
    {
        var bitmap = ImageUtils.ProcessGalleryResult(this, data);
        var recognizer = _scanbotSdk.CreateHealthInsuranceCardScanner();
        var result = recognizer.RecognizeBitmap(bitmap, 0);

        if (result == null || result.Status != EuropeanHealthInsuranceCardRecognitionResult.RecognitionStatus.Success)
        {
            Alert.Show(this, "Error", "Unable to detect the EHIC.");
            return;
        }

        var fragment = HealthInsuranceCardFragment.CreateInstance(result);
        ShowFragment(fragment, HealthInsuranceCardFragment.Name);
    }

    private void ExtractDocumentDataFromImage(Intent data)
    {
        var bitmap = ImageUtils.ProcessGalleryResult(this, data);
        var recognizer = _scanbotSdk.CreateDocumentDataExtractor();
        var result = recognizer.ExtractFromBitmap(bitmap,  0, DocumentDataExtractionMode.SingleShot);
        
        if (result?.Document == null || result.DocumentDetectionResult.Status != DocumentDetectionStatus.Ok) 
        {
            Alert.Show(this, "Error", "Unable to extract the Document data.");
            return;
        }
        
        var description = result.Document.ToFormattedString();
        Alert.Show(this, "Result", description);
    }
    
    private void RecognizeCreditCardFromImage(Intent data)
    {
        var bitmap = ImageUtils.ProcessGalleryResult(this, data);
        var recognizer = _scanbotSdk.CreateCreditCardScanner();
        var result = recognizer.ScanFromBitmap(bitmap, 0);
    
        if (result?.CreditCard == null || result.ScanningStatus != CreditCardScanningStatus.Success) 
        {
            Alert.Show(this, "Error", "Unable to detect the Credit card.");
            return;
        }
        
        var description = result.CreditCard.ToFormattedString();
        Alert.Show(this, "Result", description);
    }

    private void ShowFragment(BaseDialogFragment fragment, string name)
    {
        fragment?.Show(FragmentManager, name);
    }
}