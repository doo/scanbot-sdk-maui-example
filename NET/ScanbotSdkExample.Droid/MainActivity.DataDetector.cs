using Android.Content;
using Android.Graphics;
using IO.Scanbot.Sdk.Check;
using IO.Scanbot.Sdk.Documentdata;
using IO.Scanbot.Sdk.Ehicscanner;
using IO.Scanbot.Sdk.UI.View.Base;
using IO.Scanbot.Sdk.UI.View.Check;
using IO.Scanbot.Sdk.MC;
using IO.Scanbot.Sdk.Ui_v2.Creditcard.Configuration;
using IO.Scanbot.Sdk.Ui_v2.Mrz;
using IO.Scanbot.Sdk.Ui_v2.Mrz.Configuration;
using IO.Scanbot.Sdk.Ui_v2.Textpattern;
using IO.Scanbot.Sdk.Ui_v2.Textpattern.Configuration;
using IO.Scanbot.Sdk.UI.View.Documentdata;
using IO.Scanbot.Sdk.UI.View.Hic;
using IO.Scanbot.Sdk.UI.View.Hic.Configuration;
using IO.Scanbot.Sdk.UI.View.MC;
using IO.Scanbot.Sdk.UI.View.MC.Configuration;
using IO.Scanbot.Sdk.UI.View.Vin;
using IO.Scanbot.Sdk.Vin;
using ScanbotSdkExample.Droid.Fragments;
using ScanbotSdkExample.Droid.Utils;
using CheckScannerConfiguration = IO.Scanbot.Sdk.UI.View.Check.Configuration.CheckScannerConfiguration;
using DocumentDataExtractorConfiguration = IO.Scanbot.Sdk.UI.View.Documentdata.Configuration.DocumentDataExtractorConfiguration;
using RootDocumentType = IO.Scanbot.Sdk.Check.Entity.RootDocumentType;
using VinScannerConfiguration = IO.Scanbot.Sdk.UI.View.Vin.Configuration.VinScannerConfiguration;

namespace ScanbotSdkExample.Droid;

public partial class MainActivity
{
    private Dictionary<int, Action<Intent>> dataDetectorActions => new Dictionary<int, Action<Intent>>
    {
        { ScanMrzRequestCode, HandleMrzScanResult },
        { ExtractDocumentDataRequestCode, HandleDocumentDataExtractorResult },
        { ScanEhicRequestCode, HandleEhicResult },
        { ScanVinRequestCode, HandleVinResult },
        { ScanDataRequestCode, HandleTextDataResult },
        { ScanMedicalCertificateRequestCode, HandleMedicaCertificateResult },
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
        fragment.Show(FragmentManager, MRZDialogFragment.NAME);
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
        fragment.Show(FragmentManager, HealthInsuranceCardFragment.Name);
    }

    private void ExtractDocumentData()
    {
        var configuration = new DocumentDataExtractorConfiguration();
        configuration.SetCancelButtonTitle("Done");

        // Specify accepted types if needed
        configuration.SetAcceptedDocumentTypes([
            IO.Scanbot.Sdk.Documentdata.Entity.RootDocumentType.DeIdCardFront,
            IO.Scanbot.Sdk.Documentdata.Entity.RootDocumentType.DeIdCardBack,
            IO.Scanbot.Sdk.Documentdata.Entity.RootDocumentType.DePassport,
        ]);

        // Apply the parameters for fields
        // Use constants from NormalizedFieldNames objects from the corresponding document type

        // configuration.SetFieldsDisplayConfiguration
        // (
        //     new Dictionary<string, FieldProperties>()
        //     {
        //         { DeIdCardFront.NormalizedFieldNames.Photo,  new FieldProperties("My Id card photo", FieldProperties.DisplayState.AlwaysVisible) },
        //         { DePassport.NormalizedFieldNames.Photo,  new FieldProperties("My passport photo", FieldProperties.DisplayState.AlwaysVisible) },
        //         { MRZ.NormalizedFieldNames.CheckDigitGeneral,  new FieldProperties("Check digit general", FieldProperties.DisplayState.AlwaysVisible) },
        //     }
        // );

        var intent = DocumentDataExtractorActivity.NewIntent(this, configuration);
        StartActivityForResult(intent, ExtractDocumentDataRequestCode);
    }

    private void HandleDocumentDataExtractorResult(Intent data)
    {
        var outputArray = data.GetParcelableArrayListExtra(RtuConstants.ExtraKeyRtuResult);
        
        // List of Generic documents
        if (outputArray == null || outputArray.Count == 0)
        {
            Alert.ShowAlert(this, "Error", "Unable to scan the provided input.");
            return;
        }
        
        // For this example we only refer to the first document from the result.
        if (outputArray[0] is DocumentDataExtractionResult result)
        {
            Alert.ShowAlert(this, "Document Data Result", result?.Document?.ToFormattedString());
        }
    }

    private void ScanCheck()
    {
        var config = new CheckScannerConfiguration();
        config.SetCancelButtonTitle("Done");
        config.SetAcceptedCheckStandards(
        [
            RootDocumentType.AUSCheck,
            RootDocumentType.FRACheck,
            RootDocumentType.INDCheck,
            RootDocumentType.KWTCheck,
            RootDocumentType.USACheck,
            RootDocumentType.UAECheck,
            RootDocumentType.ISRCheck,
            RootDocumentType.CANCheck,
        ]);
        var intent = CheckScannerActivity.NewIntent(this, config);
        StartActivityForResult(intent, ScanCheckRequestCode);
    }

    private void HandleCheckResult(Intent data)
    {
        var checkResult = (CheckScanningResult)data.GetParcelableExtra(RtuConstants.ExtraKeyRtuResult);
        if (checkResult?.Check?.Fields == null)
        {
            Alert.ShowAlert(this, "Error", "Unable to scan the provided input.");
            return;
        }

        Alert.ShowAlert(this, "Check Scanner Result", checkResult.Check.ToFormattedString());
    }

    private void TextDataRecognizerTapped()
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

        Alert.ShowAlert(this, "TextPatternScanner Result", results.RawText);
    }

    private void VinRecognizerTapped()
    {
        var configuration = new VinScannerConfiguration();
        configuration.SetCancelButtonTitle("Done");

        var intent = VinScannerActivity.NewIntent(this, configuration);
        StartActivityForResult(intent, ScanVinRequestCode);
    }

    private void HandleVinResult(Intent data)
    {
        var result = (VinScannerResult)data.GetParcelableExtra(RtuConstants.ExtraKeyRtuResult);

        Alert.Toast(this, $"VIN Scanned: {result.TextResult.RawText}");
    }

    private void ScanMedicalCertificate()
    {
        var configuration = new MedicalCertificateScannerConfiguration();
        configuration.SetTopBarBackgroundColor(Color.Black);

        var intent = MedicalCertificateScannerActivity.NewIntent(this, configuration);
        StartActivityForResult(intent, ScanMedicalCertificateRequestCode);
    }

    private void HandleMedicaCertificateResult(Intent data)
    {
        var result = (MedicalCertificateScanningResult)data.GetParcelableExtra(RtuConstants.ExtraKeyRtuResult);

        var fragment = MedicalCertificateResultDialogFragment.CreateInstance(result);
        fragment.Show(FragmentManager, MedicalCertificateResultDialogFragment.NAME);
    }

    private void ScanCreditCard()
    {
        var configuration = new IO.Scanbot.Sdk.Ui_v2.Creditcard.Configuration.CreditCardScannerScreenConfiguration();
        configuration.TopBar.CancelButton.Text = "Done";

        var intent = IO.Scanbot.Sdk.Ui_v2.Creditcard.CreditCardScannerActivity.NewIntent(this, configuration);
        StartActivityForResult(intent, ScanCreditCardRequestCode);
    }

    private void HandleCreditCard(Intent data)
    {
        var resultCreditCard = (CreditCardScannerUiResult)data.GetParcelableExtra(RtuConstants.ExtraKeyRtuResult);
        if (resultCreditCard?.CreditCard?.Fields == null)
        {
            Alert.ShowAlert(this, "Error", "Unable to scan the provided input.");
            return;
        }

        Alert.ShowAlert(this, "Credit Card Scanner Result", resultCreditCard.CreditCard.ToFormattedString());
    }
}