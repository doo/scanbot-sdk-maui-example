using ScanbotSdkExample.iOS.Utils;
using Scanbot.ImagePicker.iOS;
using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Controller;

public partial class MainViewController
{
    private async void RecognizeMrz()
    {
        try
        {
            NSError error;
            
            // pick the image from photo library.
            var image = await ImagePicker.Instance.PickImageAsync();
            var imageRef = SBSDKImageRef.FromUIImageWithImage(image, new SBSDKRawImageLoadOptions());

            // create the configuration
            var config = new SBSDKMRZScannerConfiguration
            {
                EnableDetection = true,
                IncompleteResultHandling = SBSDKMRZIncompleteResultHandling.Accept
            };

            // create the scanner
            var scanner = new SBSDKMRZScanner(config, out error).GetOrThrow(error);
            
            // run the scanner on image
            var result = scanner.RunWithImage(imageRef, out error).GetOrThrow(error);
            if (result.Document == null || !result.Success)
            {
                Alert.Show("Error", "Unable to detect the MRZ.");
                return;
            }

            // show the result
            ShowPopup(this, result.Document?.ToFormattedString());
        }
        catch (Exception ex)
        {
            // display error
            Alert.ValidateAndShowError(ex);
        }
    }
	
    private async void RecognizeDocumentData()
    {
        try
        {
            NSError error;
            
            // pick the image from photo library.
            var image = await ImagePicker.Instance.PickImageAsync();
            
            // convert UIImage to SBSDKImageRef
            var imageRef = SBSDKImageRef.FromUIImageWithImage(image, new SBSDKRawImageLoadOptions());
            
            // create the configuration
            var configuration = new SBSDKDocumentDataExtractorConfiguration();
            var detectionTypes = SBSDKDocumentsModelRootType.AllDocumentTypes.Select(_ => ToString()).ToArray();
            
            // set the accepted document types
            configuration.Configurations =
            [
                new SBSDKDocumentDataExtractorCommonConfiguration
                {
                    AcceptedDocumentTypes = detectionTypes
                }
            ];

            // returns the cropped image in the result
            configuration.ReturnCrops = true;

            // create the extractor
            var extractor = new SBSDKDocumentDataExtractor(configuration, out error).GetOrThrow(error);

            // run the extractor on image
            var result = extractor.RunWithImage(imageRef, out error).GetOrThrow(error);
            if (result.Document == null)
            {
                Alert.Show("Error", "Unable to extract the Document data.");
                return;
            }

            // display the result
            ShowPopup(this, result.Document?.ToFormattedString());
        }
        catch (Exception ex)
        {
            // display error
            Alert.ValidateAndShowError(ex);
        }
    }
	
    private async void RecognizeCheck()
    {
        try
        {
            NSError error;
            
            // pick the image from photo library.
            var image = await ImagePicker.Instance.PickImageAsync();
            
            // convert UIImage to SBSDKImageRef
            var imageRef = SBSDKImageRef.FromUIImageWithImage(image, new SBSDKRawImageLoadOptions());

            // create the configuration
            var config = new SBSDKCheckScannerConfiguration();
            config.DocumentDetectionMode = SBSDKCheckDocumentDetectionMode.DetectDocument;
            
            // create the scanner
            var scanner = new SBSDKCheckScanner(new SBSDKCheckScannerConfiguration(), out error).GetOrThrow(error);

            // run the scanner on image
            var result = scanner.RunWithImage(imageRef, out error).GetOrThrow(error);
            if (result.Check == null)
            {
                Alert.Show("Error", "Unable to detect the Check.");
                return;
            }
            
            // show the result
            ShowPopup(this, result.Check?.ToFormattedString());
        }
        catch (Exception ex)
        {
            // display error
            Alert.ValidateAndShowError(ex);
        }
    }

    private async void RecognizeCreditCard()
    {
        try
        {
            NSError error;
            
            // pick the image from photo library.
            var image = await ImagePicker.Instance.PickImageAsync();
            
            // convert UIImage to SBSDKImageRef
            var imageRef = SBSDKImageRef.FromUIImageWithImage(image, new SBSDKRawImageLoadOptions());
            
            // create the configuration
            var configuration = new SBSDKCreditCardScannerConfiguration();
            configuration.ProcessingMode = SBSDKProcessingMode.SingleShot;

            // create the scanner
            var scanner = new SBSDKCreditCardScanner(configuration, out error).GetOrThrow(error);

            // run the scanner on image
            var result = scanner.RunWithImage(imageRef, out error).GetOrThrow(error);
            if (result.CreditCard == null)
            {
                Alert.Show("Error", "Unable to detect the Credit card.");
                return;
            }

            // show the result
            ShowPopup(this, result.CreditCard.ToFormattedString());
        }
        catch (Exception ex)
        {
            // display error
            Alert.ValidateAndShowError(ex);
        }
    }
	
    private async void RecognizeMedicalCertificate()
    {
        try
        {
            NSError error;
            
            // pick the image from photo library.
            var image = await ImagePicker.Instance.PickImageAsync();
            
            // convert UIImage to SBSDKImageRef
            var imageRef = SBSDKImageRef.FromUIImageWithImage(image, new SBSDKRawImageLoadOptions());
            
            // create the scanner
            var scanner = SBSDKMedicalCertificateScanner.CreateAndReturnError(out error).GetOrThrow(error);
            
            // run the scanner on image
            var result = scanner.RunWithImage(imageRef, new SBSDKMedicalCertificateScanningParameters(), out error).GetOrThrow(error);
            if (result == null || !result.ScanningSuccessful)
            {
                Alert.Show("Error", "Unable to detect the Medical certificate.");
                return;
            }

            // display the result
            ShowPopupWithAttributedText(this, result.ToFormattedAttributeString());
        }
        catch (Exception ex)
        {
            // display error
            Alert.ValidateAndShowError(ex);
        }
    }
}