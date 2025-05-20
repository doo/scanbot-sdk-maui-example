using IO.Scanbot.Sdk.UI.View.Textpattern.Entity;

namespace ReadyToUseUI.Droid.Utils
{
    public class ValidationCallback : Java.Lang.Object, TextPatternScannerStep.ITextPatternScanningValidationCallback
    {
        public bool Validate(string text)
        {
            Console.WriteLine("ValidationCallback.Validate: " + text);
            return true;
        }
    }

    public class RecognitionCallback : Java.Lang.Object, TextPatternScannerStep.ICleanScanningResultCallback
    {
        public string Process(string rawText)
        {
            Console.WriteLine("RecognitionCallback.Process: " + rawText);
            return rawText;
        }
    }
}
