

using static IO.Scanbot.Sdk.UI.View.Generictext.Entity.TextDataScannerStep;

namespace ReadyToUseUI.Droid.Utils
{
    public class ValidationCallback : Java.Lang.Object, IGenericTextValidationCallback
    {
        public bool Validate(string text)
        {
            Console.WriteLine("ValidationCallback.Validate: " + text);
            return true;
        }
    }

    public class RecognitionCallback : Java.Lang.Object, ICleanRecognitionResultCallback
    {
        public string Process(string rawText)
        {
            Console.WriteLine("RecognitionCallback.Process: " + rawText);
            return rawText;
        }
    }
}
