using Android.Content;
using ScanbotSdkExample.Droid.Model;
using IO.Scanbot.Sdk.Documentdata;
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

        if (result?.Document == null)
        {
            Alert.ShowAlert(this, "Info", "No MRZ found");
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
        
        if (result?.Check == null)
        {
            Alert.ShowAlert(this, "Info", "No Check found.");
            return;
        }
        
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

        if (result == null)
        {
            Alert.ShowAlert(this, "Info", "No Medical certificate found.");
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

        if (result == null)
        {
            Alert.ShowAlert(this, "Info", "No EHIC found.");
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
        
        if (result?.Document == null) 
        {
            Alert.ShowAlert(this, "Info", "No Document found.");
            return;
        }
        
        var description = result.Document.ToFormattedString();
        Alert.ShowAlert(this, "Result", description);
    }
    
    private void RecognizeCreditCardFromImage(Intent data)
    {
        var bitmap = ImageUtils.ProcessGalleryResult(this, data);
        var recognizer = _scanbotSdk.CreateCreditCardScanner();
        var result = recognizer.ScanFromBitmap(bitmap, 0);
    
        if (result?.CreditCard == null) 
        {
            Alert.ShowAlert(this, "Info", "No Credit card found.");
            return;
        }
        
        var description = result.CreditCard.ToFormattedString();
        Alert.ShowAlert(this, "Result", description);
    }

    private void ShowFragment(BaseDialogFragment fragment, string name)
    {
        fragment?.Show(FragmentManager, name);
    }
}