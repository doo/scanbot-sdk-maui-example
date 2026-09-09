using ScanbotSdkExample.iOS.Utils;
using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Controller;

public partial class MainViewController
{
    private void ScanMrz()
    {
        var configuration = new SBSDKUI2MRZScannerScreenConfiguration();
        var vc = SBSDKUI2MRZScannerViewController.CreateWith(configuration, (controller, result, error) =>
        {
            if (error != null)
            {
                Alert.ValidateAndShowError(error);
                return;
            }

            if (result?.MrzDocument == null)
                return;

            ShowPopup(this, result.MrzDocument.ToFormattedString());
        }, PerformMrzDismissFunc); // performDismiss

        PresentViewController(vc, true, null);
    }

    private void PerformMrzDismissFunc(SBSDKUI2MRZScannerViewController controller, SBSDKUI2MRZScannerDismissHandler dismissVc)
    {
        // Dismiss on your own
        controller.DismissViewController(true, completionHandler: () =>
        {
            // Inform the SDK, so it initiates the result/error callback.
            dismissVc?.Invoke();
        });
    }

    private void ExtractDocumentData()
    {
        var configuration = new SBSDKUI2DocumentDataExtractorScreenConfiguration();
        configuration.TopBar.CancelButton.Text = "Done";
        var vc = SBSDKUI2DocumentDataExtractorViewController.CreateWith( configuration, (controller, result, error) =>
        {
            if (error != null)
            {
                Alert.ValidateAndShowError(error);
                return;
            }

            // Display the results.
            ShowPopup(this, result.Document?.ToFormattedString());
        }, PerformDdeDismissFunc); // performDismiss

        PresentViewController(vc, true, null);
    }

    private void PerformDdeDismissFunc(SBSDKUI2DocumentDataExtractorViewController controller, SBSDKUI2DocumentDataExtractorDismissHandler dismissVc)
    {

        // Dismiss on your own
        controller.DismissViewController(true, completionHandler: () =>
        {
            // Inform the SDK, so it initiates the result/error callback.
            dismissVc?.Invoke();
        });
    }

    private void ScanCheck()
    {
        var configuration = new SBSDKUI2CheckScannerScreenConfiguration();
        configuration.TopBar.CancelButton.Text = "Done";
        var vc = SBSDKUI2CheckScannerViewController.CreateWith(configuration, (controller, result, error) =>
        {
            if (error != null)
            {
                Alert.ValidateAndShowError(error);
                return;
            }

            // Display the results.
            ShowPopup(this, result.Check?.ToFormattedString());
        }, PerformCheckDismissFunc); // performDismiss

        PresentViewController(vc, true, null);
    }

    private void PerformCheckDismissFunc(SBSDKUI2CheckScannerViewController controller, SBSDKUI2CheckScannerDismissHandler dismissVc)
    {

        // Dismiss on your own
        controller.DismissViewController(true, completionHandler: () =>
        {
            // Inform the SDK, so it initiates the result/error callback.
            dismissVc?.Invoke();
        });
    }

    private void ScanTextPattern()
    {
        var configuration = new SBSDKUI2TextPatternScannerScreenConfiguration();
        configuration.TopBar.CancelButton.Text = "Done";
        configuration.ScannerConfiguration.Validator = new SBSDKCustomContentValidator("", new MyCallback());
        
        var vc = SBSDKUI2TextPatternScannerViewController.CreateWith(configuration, (controller, result, error) =>
         {
             if (error != null)
             {
                 Alert.ValidateAndShowError(error);
                 return;
             }
             
             if (string.IsNullOrWhiteSpace(result.RawText))
             {
                 Alert.Show("Alert", "Something went wrong while scanning the text.");
                 return;
             }
             Alert.Show("Result", result.RawText);
         }, PerformDismissFunc); // performDismiss

        PresentViewController(vc, true, null);
    }

    private void PerformDismissFunc(SBSDKUI2TextPatternScannerViewController controller, SBSDKUI2TextPatternScannerDismissHandler dismissVc)
    {
        // Dismiss on your own
        controller.DismissViewController(true, completionHandler: () =>
        {
            // Inform the SDK, so it initiates the result/error callback.
            dismissVc?.Invoke();
        });
    }

    private void ScanVin()
    {
        var configuration = new SBSDKUI2VINScannerScreenConfiguration();
        configuration.TopBar.CancelButton.Text = "Done";
        var vc = SBSDKUI2VINScannerViewController.CreateWith(configuration, (controller, result, error) =>
        {
            if (error != null)
            {
                Alert.ValidateAndShowError(error);
                return;
            }

            if (!result.TextResult.ValidationSuccessful)
            {
                Alert.Show("Alert", "Something went wrong while scanning the text.");
                return;
            }

            Alert.Show("Result", result.TextResult.RawText);
        }, PerformVinDismissFunc); // performDismiss

        PresentViewController(vc, true, null);
    }

    private void PerformVinDismissFunc(SBSDKUI2VINScannerViewController controller, SBSDKUI2VINScannerDismissHandler dismissVc)
    {
        // Dismiss on your own
        controller.DismissViewController(true, completionHandler: () =>
        {
            // Inform the SDK, so it initiates the result/error callback.
            dismissVc?.Invoke();
        });
    }

    private void ScanCreditCard()
    {
        var configuration = new SBSDKUI2CreditCardScannerScreenConfiguration();
        configuration.TopBar.CancelButton.Text = "Done";

       var vc = SBSDKUI2CreditCardScannerViewController.CreateWith(configuration, (controller, result, error) =>
        {
            if (error != null)
            {
                Alert.ValidateAndShowError(error);
                return;
            }

            if (result?.CreditCard == null)
                return;

            ShowPopup(this, result.CreditCard?.ToFormattedString());
        }, PerformCreditCardDismissFunc); // performDismiss

        PresentViewController(vc, true, null);
    }

    private void PerformCreditCardDismissFunc(SBSDKUI2CreditCardScannerViewController controller, SBSDKUI2CreditCardScannerDismissHandler dismissVc)
    {
        // Dismiss on your own
        controller.DismissViewController(true, completionHandler: () =>
        {
            // Inform the SDK, so it initiates the result/error callback.
            dismissVc?.Invoke();
        });
    }

    // (1) Subclassing the wrapper crashes at construction:
    public sealed class MyCallback : SBSDKContentValidationCallback
    {
        public override bool ValidateWithText(string text) => /* … */ true;
        public override string CleanWithRawText(string rawText) => rawText;
    }
}