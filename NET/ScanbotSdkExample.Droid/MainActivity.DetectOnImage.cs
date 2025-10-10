using Android.Content;
using Android.Graphics;
using IO.Scanbot.Sdk.Check;
using IO.Scanbot.Sdk.Creditcard;
using IO.Scanbot.Sdk.Documentdata;
using IO.Scanbot.Sdk.Image;
using IO.Scanbot.Sdk.Medicalcertificate;
using IO.Scanbot.Sdk.Mrz;
using ScanbotSDK.Droid.Helpers;
using ScanbotSdkExample.Droid.Model;
using ScanbotSdkExample.Droid.Fragments;
using ScanbotSdkExample.Droid.Utils;
using ScanbotSdkExample.Droid.Views;

namespace ScanbotSdkExample.Droid;

public partial class MainActivity
{
    private Dictionary<int, Action<Intent>> DetectOnImageActions => new Dictionary<int, Action<Intent>>
    {
        { DetectMrzFromImageCode, ScanMrzFromImage },
        { DetectCheckFromImageCode, ScanCheckFromImage },
        { DetectMedicalCertificateFromImageCode, ScanMedicalCertificateFromImage },
        { ExtractDocumentDataFromImageCode, ExtractDocumentDataFromImage },
        { DetectCreditCardFromImageCode, ScanCreditCardFromImage },
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

    private void ScanMrzFromImage(Intent data)
    {
        var bitmap = ImageUtils.ProcessGalleryResult(this, data);
        var recognizer = _scanbotSdk.CreateMrzScanner(new MrzScannerConfiguration()).Get<IMrzScanner>();
        var result = recognizer?.Run(ImageRef.FromBitmap(bitmap, new BasicImageLoadOptions())).Get<MrzScannerResult>();

        if (result?.Document == null || !result.Success)
        {
            Alert.Show(this, "Error", "Unable to detect the MRZ.");
            return;
        }

        var fragment = MRZDialogFragment.CreateInstance(result.Document);
        ShowFragment(fragment, MRZDialogFragment.Name);
    }

    private void ScanCheckFromImage(Intent data)
    {
        var bitmap = ImageUtils.ProcessGalleryResult(this, data);
        var recognizer = _scanbotSdk.CreateCheckScanner(new CheckScannerConfiguration()).Get<ICheckScanner>();
        var result = recognizer?.Run(ImageRef.FromBitmap(bitmap, new BasicImageLoadOptions())).Get<CheckScanningResult>();
        
        if (result?.Check == null)
        {
            Alert.Show(this, "Error", "Unable to detect the Check.");
            return;
        }
        
        var description = result.Check.ToFormattedString();
        Alert.Show(this, "Result", description);
    }
    
    private void ScanMedicalCertificateFromImage(Intent data)
    {
        var bitmap = ImageUtils.ProcessGalleryResult(this, data);
        var recognizer = _scanbotSdk.CreateMedicalCertificateScanner().Get<IMedicalCertificateScanner>();;
        
        var parameters = new MedicalCertificateScanningParameters(
            shouldCropDocument: true,
            recognizePatientInfoBox: true,
            recognizeBarcode: true,
            extractCroppedImage: true,
            preprocessInput: true);
        
        var result = recognizer?.Run(image: ImageRef.FromBitmap(bitmap, new BasicImageLoadOptions()), parameters: parameters).Get<MedicalCertificateScanningResult>();

        if (result == null || !result.ScanningSuccessful)
        {
            Alert.Show(this, "Error", "Unable to detect the Medical certificate.");
            return;
        }

        var fragment = MedicalCertificateResultDialogFragment.CreateInstance(result);
        ShowFragment(fragment, MedicalCertificateResultDialogFragment.Name);
    }

    private void ExtractDocumentDataFromImage(Intent data)
    {
        var bitmap = ImageUtils.ProcessGalleryResult(this, data);
        var recognizer = _scanbotSdk.CreateDocumentDataExtractor(new DocumentDataExtractorConfiguration()).Get<IDocumentDataExtractor>();
        var result = recognizer.Run(ImageRef.FromBitmap(bitmap, new BasicImageLoadOptions())).Get<DocumentDataExtractionResult>();
        
        if (result?.Document == null) 
        {
            Alert.Show(this, "Error", "Unable to extract the Document data.");
            return;
        }
        
        var description = result.Document.ToFormattedString();
        Alert.Show(this, "Result", description);
    }
    
    private void ScanCreditCardFromImage(Intent data)
    {
        var bitmap = ImageUtils.ProcessGalleryResult(this, data);
        var recognizer = _scanbotSdk.CreateCreditCardScanner(new CreditCardScannerConfiguration()).Get<ICreditCardScanner>();
        var result = recognizer?.Run(ImageRef.FromBitmap(bitmap,new BasicImageLoadOptions())).Get<CreditCardScanningResult>();
    
        if (result?.CreditCard == null) 
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