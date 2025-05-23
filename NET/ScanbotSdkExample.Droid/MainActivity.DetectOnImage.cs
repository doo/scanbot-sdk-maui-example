using Android.Content;
using ScanbotSdkExample.Droid.Model;
using IO.Scanbot.Sdk.Documentdata;
using IO.Scanbot.Sdk.MC;
using ScanbotSdkExample.Droid.Fragments;
using ScanbotSdkExample.Droid.Utils;

namespace ScanbotSdkExample.Droid;

public partial class MainActivity
{
    private Dictionary<int, Action<Intent>> DetectOnImageActions => new Dictionary<int, Action<Intent>>
    {
        { DetectMrzFromImageCode, RecognizeMrzFromImage },
        { DetectEhicFromImageCode, RecognizeEHICFromImage },
        { DetectCheckFromImageCode, RecognizeCheckFromImage },
        { DetectMedicalCertificateFromImageCode, RecognizeMedicalCertificateFromImage },
        { DetectDocumentDataFromImageCode, RecognizeDocumentDataFromImage },
        { DetectCreditCardFromImageCode, RecognizeCreditCardFromImage },
    };

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

    private void RecognizeMrzFromImage(Intent data)
    {
        var bitmap = ImageUtils.ProcessGalleryResult(this, data);
        var recognizer = _scanbotSdk.CreateMrzScanner();
        var result = recognizer.ScanFromBitmap(bitmap, 0);

        if (result?.Document == null) return;

        var fragment = MRZDialogFragment.CreateInstance(result.Document);
        fragment.Show(FragmentManager, MRZDialogFragment.NAME);
    }

    private void RecognizeCheckFromImage(Intent data)
    {
        var bitmap = ImageUtils.ProcessGalleryResult(this, data);
        var recognizer = _scanbotSdk.CreateCheckScanner();
        var result = recognizer.ScanFromBitmap(bitmap, 0);
        var description = result.Check.ToFormattedString();
        Alert.ShowAlert(this, "Result", description);
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

        if (result == null) return;

        var fragment = MedicalCertificateResultDialogFragment.CreateInstance(result);
        fragment.Show(FragmentManager, MedicalCertificateResultDialogFragment.NAME);
    }

    private void RecognizeEHICFromImage(Intent data)
    {
        var bitmap = ImageUtils.ProcessGalleryResult(this, data);
        var recognizer = _scanbotSdk.CreateHealthInsuranceCardScanner();
        var result = recognizer.RecognizeBitmap(bitmap, 0);

        if (result == null) return;

        var fragment = HealthInsuranceCardFragment.CreateInstance(result);
        fragment.Show(FragmentManager, HealthInsuranceCardFragment.Name);
    }

    private void RecognizeDocumentDataFromImage(Intent data)
    {
        var bitmap = ImageUtils.ProcessGalleryResult(this, data);
        var recognizer = _scanbotSdk.CreateDocumentDataExtractor();
        var result = recognizer.ExtractFromBitmap(bitmap,  0, DocumentDataExtractionMode.Live);
    
        if (result?.Document == null) return;
        var description = result.Document.ToFormattedString();
        Alert.ShowAlert(this, "Result", description);
    }
    
    private void RecognizeCreditCardFromImage(Intent data)
    {
        var bitmap = ImageUtils.ProcessGalleryResult(this, data);
        var recognizer = _scanbotSdk.CreateCreditCardScanner();
        var result = recognizer.ScanFromBitmap(bitmap, 0);
    
        if (result?.CreditCard == null) return;
        var description = result.CreditCard.ToFormattedString();
        Alert.ShowAlert(this, "Result", description);
    }
}