using Android.Content;
using IO.Scanbot.Sdk.Check;
using IO.Scanbot.Sdk.Creditcard;
using IO.Scanbot.Sdk.Documentdata;
using IO.Scanbot.Sdk.Image;
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
        try
        {
            var bitmap = ImageUtils.ProcessGalleryResult(this, data);
            var recognizer = _scanbotSdk.CreateMrzScanner(new MrzScannerConfiguration()).GetOrThrow<IMrzScanner>();
            var result = recognizer.Run(ImageRef.FromBitmap(bitmap, new BasicImageLoadOptions())).GetOrThrow<MrzScannerResult>();
            if (result.Document == null || !result.Success)
            {
                Alert.Show(this, "Error", "Unable to detect the MRZ.");
                return;
            }

            var fragment = MRZDialogFragment.CreateInstance(result.Document);
            ShowFragment(fragment, MRZDialogFragment.Name);
        }
        catch (Exception ex)
        {
            Alert.Show(this, "Error", ex.Message);
        }
    }

    private void ScanCheckFromImage(Intent data)
    {
        try
        {
            var bitmap = ImageUtils.ProcessGalleryResult(this, data);
            var recognizer = _scanbotSdk.CreateCheckScanner(new CheckScannerConfiguration()).GetOrThrow<ICheckScanner>();
            var result = recognizer.Run(ImageRef.FromBitmap(bitmap, new BasicImageLoadOptions())).GetOrThrow<CheckScanningResult>();
            if (result.Check == null)
            {
                Alert.Show(this, "Error", "Unable to detect the Check.");
                return;
            }

            var description = result.Check.ToFormattedString();
            Alert.Show(this, "Result", description);
        }
        catch (Exception ex)
        {
            Alert.Show(this, "Error", ex.Message);
        }
    }
    
    private void ExtractDocumentDataFromImage(Intent data)
    {
        try
        {
            var bitmap = ImageUtils.ProcessGalleryResult(this, data);
            var recognizer = _scanbotSdk.CreateDocumentDataExtractor(new DocumentDataExtractorConfiguration()).GetOrThrow<IDocumentDataExtractor>();
            var result = recognizer.Run(ImageRef.FromBitmap(bitmap, new BasicImageLoadOptions())).GetOrThrow<DocumentDataExtractionResult>();
            if (result.Document == null)
            {
                Alert.Show(this, "Error", "Unable to extract the Document data.");
                return;
            }

            var description = result.Document.ToFormattedString();
            Alert.Show(this, "Result", description);
        }
        catch (Exception ex)
        {
            Alert.Show(this, "Error", ex.Message);
        }
    }

    private void ScanCreditCardFromImage(Intent data)
    {
        try
        {
            var bitmap = ImageUtils.ProcessGalleryResult(this, data);
            var recognizer = _scanbotSdk.CreateCreditCardScanner(new CreditCardScannerConfiguration()).GetOrThrow<ICreditCardScanner>();
            var result = recognizer.Run(ImageRef.FromBitmap(bitmap, new BasicImageLoadOptions())).GetOrThrow<CreditCardScanningResult>();
            if (result.CreditCard == null)
            {
                Alert.Show(this, "Error", "Unable to detect the Credit card.");
                return;
            }

            var description = result.CreditCard.ToFormattedString();
            Alert.Show(this, "Result", description);
        }
        catch (Exception ex)
        {
            Alert.Show(this, "Error", ex.Message);
        }
    }

    private void ShowFragment(BaseDialogFragment fragment, string name)
    {
        fragment?.Show(FragmentManager, name);
    }
}