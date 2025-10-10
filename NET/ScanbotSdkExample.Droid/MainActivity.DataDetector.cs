using Android.Content;
using Android.Graphics;
using IO.Scanbot.Sdk.Documentdata;
using IO.Scanbot.Sdk.Medicalcertificate;
using IO.Scanbot.Sdk.Ui_v2.Check.Configuration;
using IO.Scanbot.Sdk.Ui_v2.Creditcard;
using IO.Scanbot.Sdk.Ui_v2.Creditcard.Configuration;
using IO.Scanbot.Sdk.Ui_v2.Documentdata;
using IO.Scanbot.Sdk.Ui_v2.Documentdataextractor.Configuration;
using IO.Scanbot.Sdk.Ui_v2.Mrz;
using IO.Scanbot.Sdk.Ui_v2.Mrz.Configuration;
using IO.Scanbot.Sdk.Ui_v2.Textpattern;
using IO.Scanbot.Sdk.Ui_v2.Textpattern.Configuration;
using IO.Scanbot.Sdk.Ui_v2.Vin.Configuration;
using IO.Scanbot.Sdk.UI.View.MC;
using IO.Scanbot.Sdk.UI.View.MC.Configuration;
using ScanbotSdkExample.Droid.Fragments;
using ScanbotSdkExample.Droid.Utils;

namespace ScanbotSdkExample.Droid;

public partial class MainActivity
{
    private Dictionary<int, Action<Intent>> DataDetectorActions => new Dictionary<int, Action<Intent>>
    {
        { ScanMrzRequestCode, HandleMrzScanResult },
        { ExtractDocumentDataRequestCode, HandleDocumentDataExtractorResult },
        { ScanVinRequestCode, HandleVinResult },
        { ScanDataRequestCode, HandleTextDataResult },
        { ScanMedicalCertificateRequestCode, HandleMedicalCertificateResult },
        { ScanCheckRequestCode, HandleCheckResult },
        { ScanCreditCardRequestCode, HandleCreditCard },
    };
    
    private void ScanMrz()
    {
        var configuration = new MrzScannerScreenConfiguration();
        var intent = MrzScannerActivity.NewIntent(this, configuration);
        StartActivityForResult(intent, ScanMrzRequestCode);
    }

    private void HandleMrzScanResult(Intent data)
    {
        var result = GetParcelableExtra<MrzScannerUiResult>(data);
        var fragment = MRZDialogFragment.CreateInstance(result?.MrzDocument);
        ShowFragment(fragment, MRZDialogFragment.Name);
    }

    private void ExtractDocumentData()
    {
        var configuration = new DocumentDataExtractorScreenConfiguration();
        var intent = DocumentDataExtractorActivity.NewIntent(this, configuration);
        StartActivityForResult(intent, ExtractDocumentDataRequestCode);
    }

    private void HandleDocumentDataExtractorResult(Intent data)
    {
        var result = GetParcelableExtra<DocumentDataExtractorUiResult>(data);

        // List of Generic documents
        if (result?.Document == null || (result.RecognitionStatus != DocumentDataExtractionStatus.Ok &&
                                         result.RecognitionStatus != DocumentDataExtractionStatus.OkButInvalidDocument))
        {
            Alert.Show(this, "Error", "Unable to scan the provided input.");
            return;
        }

        // For this example we only refer to the first document from the result.
        Alert.Show(this, "Document Data Result", result.Document.ToFormattedString());
    }

    private void ScanCheck()
    {
        var config = new CheckScannerScreenConfiguration();
       
        var intent = IO.Scanbot.Sdk.Ui_v2.Check.CheckScannerActivity.NewIntent(this, config);
        StartActivityForResult(intent, ScanCheckRequestCode);
    }

    private void HandleCheckResult(Intent data)
    {
        var checkResult = GetParcelableExtra<CheckScannerUiResult>(data);
        if (checkResult?.Check?.Fields == null)
        {
            Alert.Show(this, "Error", "Unable to scan the provided input.");
            return;
        }

        Alert.Show(this, "Check Scanner Result", checkResult.Check.ToFormattedString());
    }

    private void ScanTextPattern()
    {
        var config = new TextPatternScannerScreenConfiguration();
        config.Localization.TopBarCancelButton = "Done";

        StartActivityForResult(TextPatternScannerActivity.NewIntent(this, config), ScanDataRequestCode);
    }

    private void HandleTextDataResult(Intent data)
    {
        var results = GetParcelableExtra<TextPatternScannerUiResult>(data);
        if (string.IsNullOrEmpty(results?.RawText))
        {
            return;
        }

        Alert.Show(this, "TextPatternScanner Result", results.RawText);
    }

    private void ScanVin()
    {
        var configuration = new VinScannerScreenConfiguration();

        var intent = IO.Scanbot.Sdk.Ui_v2.Vin.VinScannerActivity.NewIntent(this, configuration);
        StartActivityForResult(intent, ScanVinRequestCode);
    }

    private void HandleVinResult(Intent data)
    {
        var result = GetParcelableExtra<VinScannerUiResult>(data);

        Alert.Toast(this, $"VIN Scanned: {result.TextResult.RawText}");
    }

    private void ScanMedicalCertificate()
    {
        var configuration = new MedicalCertificateScannerConfiguration();
        configuration.SetTopBarBackgroundColor(Color.Black);

        var intent = MedicalCertificateScannerActivity.NewIntent(this, configuration);
        StartActivityForResult(intent, ScanMedicalCertificateRequestCode);
    }

    private void HandleMedicalCertificateResult(Intent data)
    {
        var result = GetParcelableExtra<MedicalCertificateScanningResult>(data);

        var fragment = MedicalCertificateResultDialogFragment.CreateInstance(result);
        ShowFragment(fragment, MedicalCertificateResultDialogFragment.Name);
    }

    private void ScanCreditCard()
    {
        var configuration = new CreditCardScannerScreenConfiguration();
        configuration.TopBar.CancelButton.Text = "Done";

        var intent = CreditCardScannerActivity.NewIntent(this, configuration);
        StartActivityForResult(intent, ScanCreditCardRequestCode);
    }

    private void HandleCreditCard(Intent data)
    {
        var resultCreditCard = GetParcelableExtra<CreditCardScannerUiResult>(data);
        if (resultCreditCard?.CreditCard?.Fields == null)
        {
            Alert.Show(this, "Error", "Unable to scan the provided input.");
            return;
        }

        Alert.Show(this, "Credit Card Scanner Result", resultCreditCard.CreditCard.ToFormattedString());
    }
}