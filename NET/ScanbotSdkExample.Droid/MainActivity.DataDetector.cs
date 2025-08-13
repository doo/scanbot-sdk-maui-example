using Android.Content;
using Android.Graphics;
using IO.Scanbot.Sdk.Documentdata;
using IO.Scanbot.Sdk.Ehicscanner;
using IO.Scanbot.Sdk.UI.View.Base;
using IO.Scanbot.Sdk.MC;
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
using IO.Scanbot.Sdk.UI.View.Hic;
using IO.Scanbot.Sdk.UI.View.Hic.Configuration;
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
        { ScanEhicRequestCode, HandleEhicResult },
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
        var result = (MrzScannerUiResult)data.GetParcelableExtra(RtuConstants.ExtraKeyRtuResult);
        var fragment = MRZDialogFragment.CreateInstance(result.MrzDocument);
        ShowFragment(fragment, MRZDialogFragment.Name);
    }

    private void ScanEhic()
    {
        var configuration = new HealthInsuranceCardScannerConfiguration();
        configuration.SetCancelButtonTitle("Done");

        var intent = HealthInsuranceCardScannerActivity.NewIntent(this, configuration);
        StartActivityForResult(intent, ScanEhicRequestCode);
    }

    private void HandleEhicResult(Intent data)
    {
        var result = (EuropeanHealthInsuranceCardRecognitionResult)data.GetParcelableExtra(RtuConstants.ExtraKeyRtuResult);
        var fragment = HealthInsuranceCardFragment.CreateInstance(result);
        ShowFragment(fragment, HealthInsuranceCardFragment.Name);
        fragment.Show(FragmentManager, HealthInsuranceCardFragment.Name);
    }

    private void ExtractDocumentData()
    {
        var configuration = new DocumentDataExtractorScreenConfiguration();
      
        var intent = DocumentDataExtractorActivity.NewIntent(this, configuration);
        StartActivityForResult(intent, ExtractDocumentDataRequestCode);
    }

    private void HandleDocumentDataExtractorResult(Intent data)
    {
        var outputArray = data.GetParcelableArrayListExtra(RtuConstants.ExtraKeyRtuResult);
        
        // List of Generic documents
        if (outputArray == null || outputArray.Count == 0)
        {
            Alert.Show(this, "Error", "Unable to scan the provided input.");
            return;
        }
        
        // For this example we only refer to the first document from the result.
        if (outputArray[0] is DocumentDataExtractionResult result)
        {
            Alert.Show(this, "Document Data Result", result?.Document?.ToFormattedString());
        }
    }

    private void ScanCheck()
    {
        var config = new CheckScannerScreenConfiguration();
       
        var intent = IO.Scanbot.Sdk.Ui_v2.Check.CheckScannerActivity.NewIntent(this, config);
        StartActivityForResult(intent, ScanCheckRequestCode);
    }

    private void HandleCheckResult(Intent data)
    {
        var checkResult = (CheckScannerUiResult)data.GetParcelableExtra(RtuConstants.ExtraKeyRtuResult);
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
        var results = (TextPatternScannerUiResult)data.GetParcelableExtra(RtuConstants.ExtraKeyRtuResult);
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
        var result = (VinScannerUiResult)data.GetParcelableExtra(RtuConstants.ExtraKeyRtuResult);

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
        var result = (MedicalCertificateScanningResult)data.GetParcelableExtra(RtuConstants.ExtraKeyRtuResult);

        var fragment = MedicalCertificateResultDialogFragment.CreateInstance(result);
        fragment.Show(FragmentManager, MedicalCertificateResultDialogFragment.Name);
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
        var resultCreditCard = (CreditCardScannerUiResult)data.GetParcelableExtra(RtuConstants.ExtraKeyRtuResult);
        if (resultCreditCard?.CreditCard?.Fields == null)
        {
            Alert.Show(this, "Error", "Unable to scan the provided input.");
            return;
        }

        Alert.Show(this, "Credit Card Scanner Result", resultCreditCard.CreditCard.ToFormattedString());
    }
}