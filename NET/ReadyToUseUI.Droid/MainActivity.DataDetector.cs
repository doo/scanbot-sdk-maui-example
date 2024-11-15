using Android.Content;
using Android.Graphics;
using IO.Scanbot.Ehicscanner.Model;
using IO.Scanbot.Genericdocument.Entity;
using IO.Scanbot.Mrzscanner.Model;
using IO.Scanbot.Sdk.Check.Entity;
using IO.Scanbot.Sdk.Mcrecognizer.Entity;
using IO.Scanbot.Sdk.UI.Result;
using IO.Scanbot.Sdk.UI.View.Base;
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
using IO.Scanbot.Sdk.UI.View.Licenseplate.Entity;
using IO.Scanbot.Sdk.UI.View.MC;
using IO.Scanbot.Sdk.UI.View.MC.Configuration;
using IO.Scanbot.Sdk.UI.View.Mrz;
using IO.Scanbot.Sdk.UI.View.Mrz.Configuration;
using IO.Scanbot.Sdk.UI.View.Vin;
using IO.Scanbot.Sdk.UI.View.Vin.Configuration;
using IO.Scanbot.Sdk.Vin;
using ReadyToUseUI.Droid.Fragments;
using ReadyToUseUI.Droid.Utils;

namespace ReadyToUseUI.Droid;

public partial class MainActivity
{
    private Dictionary<int, Action<Intent>> dataDetectorActions => new Dictionary<int, Action<Intent>>
    {
        { SCAN_MRZ_REQUEST, HandleMrzScanResult },
        { GENERIC_DOCUMENT_REQUEST, HandleGenericDocumentResult },
        { SCAN_EHIC_REQUEST, HandleEhicResult },
        { SCAN_VIN_REQUEST, HandleVinResult },
        { SCAN_DATA_REQUEST, HandleTextDataResult },
        { SCAN_EU_LICENSE_REQUEST, HandleEuLicenseResult },
        { SCAN_MEDICAL_CERTIFICATE_REQUEST, HandleMedicaCertificateResult },
        { CHECK_RECOGNIZER_REQUEST, HandleCheckResult },
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
        var result = (MRZGenericDocument)data.GetParcelableExtra(RtuConstants.ExtraKeyRtuResult);
        var fragment = MRZDialogFragment.CreateInstance(result);
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
        var result = (EhicRecognitionResult)data.GetParcelableExtra(RtuConstants.ExtraKeyRtuResult);
        var fragment = HealthInsuranceCardFragment.CreateInstance(result);
        fragment.Show(FragmentManager, HealthInsuranceCardFragment.NAME);
    }

    private void RecongnizeGenericDocument()
    {
        var configuration = new GenericDocumentRecognizerConfiguration();
        configuration.SetCancelButtonTitle("Done");

        // Specify accepted types if needed
        // configuration.SetAcceptedDocumentTypes(new List<RootDocumentType>
        // {
        //     RootDocumentType.DeIdCardFront,
        //     //RootDocumentType.DeIdCardBack,
        //     //RootDocumentType.DePassport,
        // });

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

    private void HandleGenericDocumentResult(Intent data)
    {
        var resultsArray = data.GetParcelableArrayListExtra(RtuConstants.ExtraKeyRtuResult);
        if (resultsArray?.Count == 0)
        {
            return;
        }

        var resultWrapper = (ResultWrapper)resultsArray[0];
        var resultRepository = scanbotSDK.ResultRepositoryForClass(resultWrapper.Clazz);
        var genericDocument = (GenericDocument)resultRepository.GetResultAndErase(resultWrapper.ResultId);
        var fields = genericDocument.Fields.Cast<Field>().ToList();

        var description = string.Join(";\n", fields
            .Where(field => field != null)
            .Select(field =>
            {
                string typeName = field.GetType().Name;
                string valueText = field.Value?.Text;
                return !string.IsNullOrEmpty(typeName) && !string.IsNullOrEmpty(valueText)
                    ? $"{typeName} = {valueText}"
                    : null;
            })
            .Where(outStr => outStr != null)
            .ToList()
        );

        Alert.ShowAlert(this, "Result", description);
    }

    private void RecognizeCheck()
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
            IO.Scanbot.Check.Entity.RootDocumentType.UAECheck,
            IO.Scanbot.Check.Entity.RootDocumentType.ISRCheck,
            IO.Scanbot.Check.Entity.RootDocumentType.CANCheck,
        });
        var intent = CheckRecognizerActivity.NewIntent(this, config);
        StartActivityForResult(intent, CHECK_RECOGNIZER_REQUEST);
    }

    private void HandleCheckResult(Intent data)
    {
        var resultWrapper = (ResultWrapper)data.GetParcelableExtra(RtuConstants.ExtraKeyRtuResult);
        var resultRepository = scanbotSDK.ResultRepositoryForClass(resultWrapper.Clazz);
        var checkResult = (CheckRecognizerResult)resultRepository.GetResultAndErase(resultWrapper.ResultId);
        var fields = checkResult.Check.Fields;
        var description = string.Join(";\n", fields
            .Where(field => field != null)
            .Select((field) =>
            {
                string outStr = "";
                if (field.GetType() != null && field.GetType().Name != null)
                {
                    outStr += field.GetType().Name + " = ";
                }
                if (field.Value != null && field.Value.Text != null)
                {
                    outStr += field.Value.Text;
                }
                return outStr;
            })
            .ToList());

        Alert.ShowAlert(this, "Result", description);
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

    private void HandleTextDataResult(Intent data)
    {
        var results = data.GetParcelableArrayExtra(RtuConstants.ExtraKeyRtuResult);
        if (results == null || results.Length == 0)
        {
            return;
        }
        var textDataScannerStepResult = results.First() as TextDataScannerStepResult;
        Alert.Toast(this, "Text Recognizer Result: " + textDataScannerStepResult.Text);
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
        var result = (VinScanResult)data.GetParcelableExtra(RtuConstants.ExtraKeyRtuResult);

        Alert.Toast(this, $"VIN Scanned: {result?.RawText}");
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

    private void HandleEuLicenseResult(Intent data)
    {
        var result = data.GetParcelableExtra(RtuConstants.ExtraKeyRtuResult) as LicensePlateScannerResult;

        if (result == null)
        {
            return;
        }

        Alert.Toast(this, $"EU_LICENSE Scanned: {result.RawText}");
    }

    private void ScanMedicalCertificate()
    {
        var configuration = new MedicalCertificateRecognizerConfiguration();
        configuration.SetTopBarBackgroundColor(Color.Black);

        var intent = MedicalCertificateRecognizerActivity.NewIntent(this, configuration);
        StartActivityForResult(intent, SCAN_MEDICAL_CERTIFICATE_REQUEST);
    }

    private void HandleMedicaCertificateResult(Intent data)
    {
        var resultWrapper = (ResultWrapper)data.GetParcelableExtra(RtuConstants.ExtraKeyRtuResult);
        var resultRepository = scanbotSDK.ResultRepositoryForClass(resultWrapper.Clazz);
        var result = (MedicalCertificateRecognizerResult)resultRepository.GetResultAndErase(resultWrapper.ResultId);

        var fragment = MedicalCertificateResultDialogFragment.CreateInstance(result);
        fragment.Show(FragmentManager, MedicalCertificateResultDialogFragment.NAME);
    }
}