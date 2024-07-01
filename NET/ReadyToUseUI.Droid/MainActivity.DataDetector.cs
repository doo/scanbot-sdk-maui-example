using Android.Graphics;
using IO.Scanbot.Genericdocument.Entity;
using IO.Scanbot.Sdk.UI.View.Check;
using IO.Scanbot.Sdk.UI.View.Check.Configuration;
using IO.Scanbot.Sdk.UI.View.Genericdocument;
using IO.Scanbot.Sdk.UI.View.Genericdocument.Configuration;
using IO.Scanbot.Sdk.UI.View.Generictext;
using IO.Scanbot.Sdk.UI.View.Generictext.Configuration;
using IO.Scanbot.Sdk.UI.View.Generictext.Entity;
using IO.Scanbot.Sdk.UI.View.Hic;
using IO.Scanbot.Sdk.UI.View.Hic.Configuration;
using IO.Scanbot.Sdk.UI.View.Licenseplate;
using IO.Scanbot.Sdk.UI.View.Licenseplate.Configuration;
using IO.Scanbot.Sdk.UI.View.MC;
using IO.Scanbot.Sdk.UI.View.MC.Configuration;
using IO.Scanbot.Sdk.UI.View.Mrz;
using IO.Scanbot.Sdk.UI.View.Mrz.Configuration;
using IO.Scanbot.Sdk.UI.View.Vin;
using IO.Scanbot.Sdk.UI.View.Vin.Configuration;
using ReadyToUseUI.Droid.Utils;

namespace ReadyToUseUI.Droid;

public partial class MainActivity
{
    
    private void ScanMrz()
    {
        var configuration = new MRZScannerConfiguration();
        configuration.SetCancelButtonTitle("Done");

        var intent = MRZScannerActivity.NewIntent(this, configuration);
        StartActivityForResult(intent, SCAN_MRZ_REQUEST);
    }

    private void ScanEhic()
    {
        var configuration = new HealthInsuranceCardScannerConfiguration();
        configuration.SetCancelButtonTitle("Done");

        var intent = HealthInsuranceCardScannerActivity.NewIntent(this, configuration);
        StartActivityForResult(intent, SCAN_EHIC_REQUEST);
    }

    private void RecongnizeGenericDocument()
    {
        var configuration = new GenericDocumentRecognizerConfiguration();
        configuration.SetCancelButtonTitle("Done");

        // Specify accepted types if needed
        configuration.SetAcceptedDocumentTypes(new List<RootDocumentType>
        {
            RootDocumentType.DeIdCardFront,
            //RootDocumentType.DeIdCardBack,
            //RootDocumentType.DePassport,
        });

        // Apply the parameters for fields
        // Use constants from NormalizedFieldNames objects from the corresponding document type

        //configuration.SetFieldsDisplayConfiguration
        //(
        //    new Dictionary<string, FieldProperties>()
        //    {
        //        { DeIdCardFront.NormalizedFieldNames.Photo,  new FieldProperties("My Id card photo", FieldProperties.DisplayState.AlwaysVisible) },
        //        { DePassport.NormalizedFieldNames.Photo,  new FieldProperties("My passport photo", FieldProperties.DisplayState.AlwaysVisible) },
        //        { MRZ.NormalizedFieldNames.CheckDigitGeneral,  new FieldProperties("Check digit general", FieldProperties.DisplayState.AlwaysVisible) },
        //    }
        //);

        var intent = GenericDocumentRecognizerActivity.NewIntent(this, configuration);
        StartActivityForResult(intent, GENERIC_DOCUMENT_REQUEST);
    }

    private void RecogniseCheck()
    {
        var config = new CheckRecognizerConfiguration();
        config.SetCancelButtonTitle("Done");
        config.SetAcceptedCheckStandards(new List<IO.Scanbot.Check.Entity.RootDocumentType>
        {
            IO.Scanbot.Check.Entity.RootDocumentType.AUSCheck,
            IO.Scanbot.Check.Entity.RootDocumentType.FRACheck,
            IO.Scanbot.Check.Entity.RootDocumentType.INDCheck,
            IO.Scanbot.Check.Entity.RootDocumentType.KWTCheck,
            IO.Scanbot.Check.Entity.RootDocumentType.USACheck,
        });
        var intent = CheckRecognizerActivity.NewIntent(this, config);
        StartActivityForResult(intent, CHECK_RECOGNIZER_REQUEST);
    }

    private void TextDataRecognizerTapped()
    {
        // Launch the TextDataScanner UI
        var dataScannerStep = new TextDataScannerStep(
            stepTag: "tag",
            title: string.Empty,
            guidanceText: string.Empty,
            pattern: string.Empty,
            shouldMatchSubstring: true,
            validationCallback: new ValidationCallback(),
            cleanRecognitionResultCallback: new RecognitionCallback(),
            preferredZoom: 1.6f,
            aspectRatio: new IO.Scanbot.Sdk.AspectRatio(4.0, 1.0),
            unzoomedFinderHeight: 40f,
            allowedSymbols: new List<Java.Lang.Character>(),
            significantShakeDelay: 0);

        var config = new TextDataScannerConfiguration(dataScannerStep);
        config.SetCancelButtonTitle("Done");

        StartActivityForResult(TextDataScannerActivity.NewIntent(this, config), SCAN_DATA_REQUEST);
    }

    private void VinRecognizerTapped()
    {
        var configuration = new VinScannerConfiguration();
        configuration.SetCancelButtonTitle("Done");

        var intent = VinScannerActivity.NewIntent(this, configuration);
        StartActivityForResult(intent, SCAN_VIN_REQUEST);
    }

    private void LicensePlateRecognizerTapped()
    {
        var configuration = new LicensePlateScannerConfiguration();
        configuration.SetCancelButtonTitle("Done");
        configuration.SetTopBarButtonsColor(Color.Gray);
        configuration.SetTopBarBackgroundColor(Color.Black);

        var intent = LicensePlateScannerActivity.NewIntent(this, configuration);
        StartActivityForResult(intent, SCAN_EU_LICENSE_REQUEST);
    }

    private void ScanMedicalCertificate()
    {
        var configuration = new MedicalCertificateRecognizerConfiguration();
        configuration.SetTopBarBackgroundColor(Color.Black);

        var intent = MedicalCertificateRecognizerActivity.NewIntent(this, configuration);
        StartActivityForResult(intent, SCAN_MEDICAL_CERTIFICATE_REQUEST);
    }
}