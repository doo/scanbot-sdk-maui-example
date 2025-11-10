using Android.Content;
using AndroidX.AppCompat.App;
using IO.Scanbot.Sdk.Textpattern;
using IO.Scanbot.Sdk.Ui_v2.Common.Activity;
using IO.Scanbot.Sdk.Ui_v2.Textpattern;
using IO.Scanbot.Sdk.Ui_v2.Textpattern.Configuration;
using IO.Scanbot.Sdk.UI.View.Base;
using Java.Util.Regex;

namespace ScanbotSdkExample.Droid.Snippets.TextPatternScanner;

public class LaunchSnippet : AppCompatActivity
{
    private IO.Scanbot.Sdk.ScanbotSDK _scanbotSdk;
    private const int ScanTextPatternRequestCode = 001;

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
		
        // Returns the singleton instance of the Sdk.
        _scanbotSdk = new IO.Scanbot.Sdk.ScanbotSDK(this);
		
        if (_scanbotSdk.LicenseInfo.IsValid)
        {
            LaunchTextPatternScanner();
        }
    }
    private void LaunchTextPatternScanner()
    {
        var configuration = new TextPatternScannerScreenConfiguration();

        // Set a text pattern e.g. 4 digits
        var pattern = Pattern.Compile("^[0-9]{4}$"); 
		
        // Create and assign a custom validator
        configuration.ScannerConfiguration.Validator = new CustomContentValidator
        {
            Callback = new CustomValidationCallback(pattern: pattern),
            AllowedCharacters = "^[0-9]{4}$"
        };

        // Start the activity
        var intent = TextPatternScannerActivity.NewIntent(this, configuration);
        StartActivityForResult(intent, ScanTextPatternRequestCode);
    }

    // Custom TextPattern Validation implementation
    public class CustomValidationCallback(Pattern pattern) : Java.Lang.Object, IContentValidationCallback
    {
        public string Clean(string rawText)
        {
            return rawText.Replace(" ", "");
        }

        public bool Validate(string text)
        {
            var matcher = pattern.Matcher(text);
            return matcher.Find();
        }
    }

    public override void StartActivityForResult(Intent intent, int requestCode, Bundle options)
    {
        base.StartActivityForResult(intent, requestCode, options);
        var resultEntity = (TextPatternScannerUiResult)intent.GetParcelableExtra(ActivityConstants.ExtraKeyRtuResult);
        if (resultEntity?.RawText == null)
        {
            return;
        }

        Toast.MakeText(this, resultEntity.RawText, ToastLength.Long)?.Show();
    }
}