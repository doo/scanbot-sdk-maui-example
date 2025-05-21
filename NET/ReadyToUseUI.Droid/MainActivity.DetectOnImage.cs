using Android.Content;
using DocumentSDK.NET.Model;
using IO.Scanbot.Sdk.Documentdata;
using IO.Scanbot.Sdk.MC;
using ReadyToUseUI.Droid.Fragments;
using ReadyToUseUI.Droid.Utils;

namespace ReadyToUseUI.Droid;

public partial class MainActivity
{
    private Dictionary<int, Action<Intent>> detectOnImageActions => new Dictionary<int, Action<Intent>>
    {
        { DETECT_MRZ_FROM_IMAGE, DetectMrzFromImage },
        { DETECT_EHIC_FROM_IMAGE, DetectEHICFromImage },
        { DETECT_CHECK_FROM_IMAGE, DetectCheckFromImage },
        { DETECT_MEDICAL_CERTIFICATE_FROM_IMAGE, DetectMedicalCertificateFromImage },
        { DETECT_DOCUMENT_DATA_FROM_IMAGE, DetectDocumentDataFromImage },
        { DETECT_CREDIT_CARD_FROM_IMAGE, DetectCreditCardFromImage },
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

    private void DetectMrzFromImage(Intent data)
    {
        var bitmap = ImageUtils.ProcessGalleryResult(this, data);
        var recognizer = scanbotSDK.CreateMrzScanner();
        var result = recognizer.ScanFromBitmap(bitmap, 0);

        if (result?.Document == null) return;

        var fragment = MRZDialogFragment.CreateInstance(result.Document);
        fragment.Show(FragmentManager, MRZDialogFragment.NAME);
    }

    private void DetectCheckFromImage(Intent data)
    {
        var bitmap = ImageUtils.ProcessGalleryResult(this, data);
        var recognizer = scanbotSDK.CreateCheckScanner();
        var result = recognizer.ScanFromBitmap(bitmap, 0);
        var description = result.Check.ToFormattedString();
        Alert.ShowAlert(this, "Result", description);
    }

    private void DetectMedicalCertificateFromImage(Intent data)
    {
        var bitmap = ImageUtils.ProcessGalleryResult(this, data);
        var recognizer = scanbotSDK.CreateMedicalCertificateScanner();
        
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

    private void DetectEHICFromImage(Intent data)
    {
        var bitmap = ImageUtils.ProcessGalleryResult(this, data);
        var recognizer = scanbotSDK.CreateHealthInsuranceCardScanner();
        var result = recognizer.RecognizeBitmap(bitmap, 0);

        if (result == null) return;

        var fragment = HealthInsuranceCardFragment.CreateInstance(result);
        fragment.Show(FragmentManager, HealthInsuranceCardFragment.NAME);
    }

    private void DetectDocumentDataFromImage(Intent data)
    {
        var bitmap = ImageUtils.ProcessGalleryResult(this, data);
        var recognizer = scanbotSDK.CreateDocumentDataExtractor();
        var result = recognizer.ExtractFromBitmap(bitmap,  0, DocumentDataExtractionMode.Live);
    
        if (result?.Document == null) return;
        var description = result.Document.ToFormattedString();
        Alert.ShowAlert(this, "Result", description);
    }
    
    private void DetectCreditCardFromImage(Intent data)
    {
        var bitmap = ImageUtils.ProcessGalleryResult(this, data);
        var recognizer = scanbotSDK.CreateCreditCardScanner();
        var result = recognizer.ScanFromBitmap(bitmap, 0);
    
        if (result?.CreditCard == null) return;
        var description = result.CreditCard.ToFormattedString();
        Alert.ShowAlert(this, "Result", description);
    }
}