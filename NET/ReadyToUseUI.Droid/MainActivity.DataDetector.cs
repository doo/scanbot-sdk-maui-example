using Android.Content;
using Android.Graphics;
using IO.Scanbot.Ehicscanner;
using IO.Scanbot.Genericdocument.Entity;
using IO.Scanbot.Mrz;
using IO.Scanbot.Sdk.Check;
using IO.Scanbot.Sdk.Check.Entity;
using IO.Scanbot.Sdk.Creditcard;
using IO.Scanbot.Sdk.Documentdata;
using IO.Scanbot.Sdk.Documentdata.Entity;
using IO.Scanbot.Sdk.Mcrecognizer;
using IO.Scanbot.Sdk.UI.Result;
using IO.Scanbot.Sdk.UI.View.Base;
using IO.Scanbot.Sdk.UI.View.Check;
using IO.Scanbot.Sdk.Genericdocument;
using IO.Scanbot.Sdk.Genericdocument.Entity;
using IO.Scanbot.Sdk.MC;
using IO.Scanbot.Sdk.Mrz;
using IO.Scanbot.Sdk.Ui_v2.Creditcard.Configuration;
using IO.Scanbot.Sdk.Ui_v2.Mrz;
using IO.Scanbot.Sdk.Ui_v2.Textpattern;
using IO.Scanbot.Sdk.Ui_v2.Textpattern.Configuration;
using IO.Scanbot.Sdk.UI.View.Documentdata;
using IO.Scanbot.Sdk.UI.View.Hic;
using IO.Scanbot.Sdk.UI.View.Hic.Configuration;
using IO.Scanbot.Sdk.UI.View.MC;
using IO.Scanbot.Sdk.UI.View.MC.Configuration;
using IO.Scanbot.Sdk.UI.View.Mrz;
using IO.Scanbot.Sdk.UI.View.Mrz.Configuration;
using IO.Scanbot.Sdk.UI.View.Textpattern.Configuration;
using IO.Scanbot.Sdk.UI.View.Textpattern.Entity;
using IO.Scanbot.Sdk.UI.View.Vin;
using IO.Scanbot.Sdk.Vin;
using ReadyToUseUI.Droid.Fragments;
using ReadyToUseUI.Droid.Utils;
using CheckScannerConfiguration = IO.Scanbot.Sdk.UI.View.Check.Configuration.CheckScannerConfiguration;
using DocumentDataExtractorConfiguration = IO.Scanbot.Sdk.UI.View.Documentdata.Configuration.DocumentDataExtractorConfiguration;
using RootDocumentType = IO.Scanbot.Sdk.Check.Entity.RootDocumentType;
using VinScannerConfiguration = IO.Scanbot.Sdk.UI.View.Vin.Configuration.VinScannerConfiguration;

namespace ReadyToUseUI.Droid;

public partial class MainActivity
{
    private Dictionary<int, Action<Intent>> dataDetectorActions => new Dictionary<int, Action<Intent>>
    {
        { SCAN_MRZ_REQUEST, HandleMrzScanResult },
        { EXTRACT_DOCUMENT_DATA_REQUEST, HandleDocumentDataExtractorResult },
        { SCAN_EHIC_REQUEST, HandleEhicResult },
        { SCAN_VIN_REQUEST, HandleVinResult },
        { SCAN_DATA_REQUEST, HandleTextDataResult },
        { SCAN_MEDICAL_CERTIFICATE_REQUEST, HandleMedicaCertificateResult },
        { SCAN_CHECK_REQUEST, HandleCheckResult },
        { SCAN_CREDIT_CARD_REQUEST, HandleCreditCard },
    };

    private void ScanMrz()
    {
        var configuration = new MRZScannerConfiguration();
        configuration.SetCancelButtonTitle("Done");

        var intent = MRZScannerActivity.NewIntent(this, configuration);
        StartActivityForResult(intent, SCAN_MRZ_REQUEST);
    }

    private void HandleMrzScanResult(Intent data)
    {
        var result = (MrzScannerResult)data.GetParcelableExtra(RtuConstants.ExtraKeyRtuResult);
        var fragment = MRZDialogFragment.CreateInstance(result.Document);
        fragment.Show(FragmentManager, MRZDialogFragment.NAME);
    }

    private void ScanEhic()
    {
        var configuration = new HealthInsuranceCardScannerConfiguration();
        configuration.SetCancelButtonTitle("Done");

        var intent = HealthInsuranceCardScannerActivity.NewIntent(this, configuration);
        StartActivityForResult(intent, SCAN_EHIC_REQUEST);
    }

    private void HandleEhicResult(Intent data)
    {
        var result = (HealthInsuranceCardScannerActivity.HealthInsuranceCardScannerActivityResult)data.GetParcelableExtra(RtuConstants.ExtraKeyRtuResult);
        var fragment = HealthInsuranceCardFragment.CreateInstance(result.Component2());
        fragment.Show(FragmentManager, HealthInsuranceCardFragment.NAME);
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
        StartActivityForResult(intent, EXTRACT_DOCUMENT_DATA_REQUEST);
    }

    private void HandleDocumentDataExtractorResult(Intent data)
    {
        var result = (DocumentDataExtractionResult)data.GetParcelableExtra(RtuConstants.ExtraKeyRtuResult);
        if (result?.Document == null)
        {
            Alert.ShowAlert(this, "Error", "Unable to scan the provided input.");
            return;
        }

        Alert.ShowAlert(this, "Document Data Result", result.Document.ToFormattedString());
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
        StartActivityForResult(intent, SCAN_CHECK_REQUEST);
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

        StartActivityForResult(TextPatternScannerActivity.NewIntent(this, config), SCAN_DATA_REQUEST);
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
        StartActivityForResult(intent, SCAN_VIN_REQUEST);
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
        StartActivityForResult(intent, SCAN_MEDICAL_CERTIFICATE_REQUEST);
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
        StartActivityForResult(intent, SCAN_CREDIT_CARD_REQUEST);
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