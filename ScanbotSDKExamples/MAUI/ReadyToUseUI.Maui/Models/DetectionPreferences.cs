using BarcodeSDK.MAUI.Configurations;
using BarcodeSDK.MAUI.Constants;

namespace ReadyToUseUI.Maui.Models
{
    public class DetectionPreferences
    {
        public static DetectionPreferences Instance { get; private set; } = new DetectionPreferences();

        public BarcodeScannerAdditionalParameters BarcodeAdditionalParameters { get; set; }

        private DetectionPreferences() {
            BarcodeAdditionalParameters = new BarcodeScannerAdditionalParameters();
        }

        public void Update(string key, bool newValue) {
            switch (key) {
                case "gs1DecodingEnabled":
                    BarcodeAdditionalParameters.Gs1DecodingEnabled = newValue;
                    break;
                case "stripCheckDigits":
                    BarcodeAdditionalParameters.StripCheckDigits = newValue;
                    break;
                default:
                    break;
            }
        }

        public void Update(string key, string value) {
            switch (key) {
                case "msiPlesseyChecksumAlgorithm":
                    if (value == "Default") {
                        BarcodeAdditionalParameters.MsiPlesseyChecksumAlgorithm = null;
                        return;
                    }
                    BarcodeAdditionalParameters.MsiPlesseyChecksumAlgorithm =
                        (MSIPlesseyChecksumAlgorithm?)Enum.Parse(typeof(MSIPlesseyChecksumAlgorithm), value);
                    break;
                default:
                    break;
            }
        }
    }
}
