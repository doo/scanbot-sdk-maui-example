using ScanbotSDK.iOS;

namespace ReadyToUseUI.iOS.Utils;

public static class BarcodeUtilities
{
    internal static string ToBarcodeName(this SBSDKUI2BarcodeFormat source)
    {
        if (source.Equals(SBSDKUI2BarcodeFormat.Aztec))
        {
            return nameof(SBSDKUI2BarcodeFormat.Aztec);
        }

        if (source.Equals(ScanbotSDK.iOS.SBSDKUI2BarcodeFormat.Codabar))
        {
            return nameof(SBSDKUI2BarcodeFormat.Codabar);
        }

        if (source.Equals(ScanbotSDK.iOS.SBSDKUI2BarcodeFormat.Code25))
        {
            return nameof(SBSDKUI2BarcodeFormat.Code25);
        }

        if (source.Equals(ScanbotSDK.iOS.SBSDKUI2BarcodeFormat.Code39))
        {
            return nameof(SBSDKUI2BarcodeFormat.Code39);
        }

        if (source.Equals(ScanbotSDK.iOS.SBSDKUI2BarcodeFormat.Code93))
        {
            return nameof(SBSDKUI2BarcodeFormat.Code93);
        }

        if (source.Equals(ScanbotSDK.iOS.SBSDKUI2BarcodeFormat.Code128))
        {
            return nameof(SBSDKUI2BarcodeFormat.Code128);
        }

        if (source.Equals(ScanbotSDK.iOS.SBSDKUI2BarcodeFormat.DataMatrix))
        {
            return nameof(SBSDKUI2BarcodeFormat.DataMatrix);
        }

        if (source.Equals(ScanbotSDK.iOS.SBSDKUI2BarcodeFormat.Ean8))
        {
            return nameof(SBSDKUI2BarcodeFormat.Ean8);
        }

        if (source.Equals(ScanbotSDK.iOS.SBSDKUI2BarcodeFormat.Ean13))
        {
            return nameof(SBSDKUI2BarcodeFormat.Ean13);
        }

        if (source.Equals(ScanbotSDK.iOS.SBSDKUI2BarcodeFormat.Itf))
        {
            return nameof(SBSDKUI2BarcodeFormat.Itf);
        }

        if (source.Equals(ScanbotSDK.iOS.SBSDKUI2BarcodeFormat.Pdf417))
        {
            return nameof(SBSDKUI2BarcodeFormat.Pdf417);
        }

        if (source.Equals(ScanbotSDK.iOS.SBSDKUI2BarcodeFormat.QrCode))
        {
            return nameof(SBSDKUI2BarcodeFormat.QrCode);
        }

        if (source.Equals(ScanbotSDK.iOS.SBSDKUI2BarcodeFormat.MicroQrCode))
        {
            return nameof(SBSDKUI2BarcodeFormat.MicroQrCode);
        }

        if (source.Equals(ScanbotSDK.iOS.SBSDKUI2BarcodeFormat.Databar))
        {
            return nameof(SBSDKUI2BarcodeFormat.Databar);
        }

        if (source.Equals(ScanbotSDK.iOS.SBSDKUI2BarcodeFormat.DatabarExpanded))
        {
            return nameof(SBSDKUI2BarcodeFormat.DatabarExpanded);
        }

        if (source.Equals(ScanbotSDK.iOS.SBSDKUI2BarcodeFormat.UpcA))
        {
            return nameof(SBSDKUI2BarcodeFormat.UpcA);
        }

        if (source.Equals(ScanbotSDK.iOS.SBSDKUI2BarcodeFormat.UpcE))
        {
            return nameof(SBSDKUI2BarcodeFormat.UpcE);
        }

        if (source.Equals(ScanbotSDK.iOS.SBSDKUI2BarcodeFormat.MsiPlessey))
        {
            return nameof(SBSDKUI2BarcodeFormat.MsiPlessey);
        }

        if (source.Equals(ScanbotSDK.iOS.SBSDKUI2BarcodeFormat.Iata2Of5))
        {
            return nameof(SBSDKUI2BarcodeFormat.Iata2Of5);
        }

        if (source.Equals(ScanbotSDK.iOS.SBSDKUI2BarcodeFormat.Industrial2Of5))
        {
            return nameof(SBSDKUI2BarcodeFormat.Industrial2Of5);
        }

        if (source.Equals(ScanbotSDK.iOS.SBSDKUI2BarcodeFormat.UspsIntelligentMail))
        {
            return nameof(SBSDKUI2BarcodeFormat.UspsIntelligentMail);
        }

        if (source.Equals(ScanbotSDK.iOS.SBSDKUI2BarcodeFormat.RoyalMail))
        {
            return nameof(SBSDKUI2BarcodeFormat.RoyalMail);
        }

        if (source.Equals(ScanbotSDK.iOS.SBSDKUI2BarcodeFormat.JapanPost))
        {
            return nameof(SBSDKUI2BarcodeFormat.JapanPost);
        }

        if (source.Equals(ScanbotSDK.iOS.SBSDKUI2BarcodeFormat.RoyalTntPost))
        {
            return nameof(SBSDKUI2BarcodeFormat.RoyalTntPost);
        }

        if (source.Equals(ScanbotSDK.iOS.SBSDKUI2BarcodeFormat.AustraliaPost))
        {
            return nameof(SBSDKUI2BarcodeFormat.AustraliaPost);
        }

        if (source.Equals(ScanbotSDK.iOS.SBSDKUI2BarcodeFormat.DatabarLimited))
        {
            return nameof(SBSDKUI2BarcodeFormat.DatabarLimited);
        }

        if (source.Equals(ScanbotSDK.iOS.SBSDKUI2BarcodeFormat.Gs1Composite))
        {
            return nameof(SBSDKUI2BarcodeFormat.Gs1Composite);
        }

        if (source.Equals(ScanbotSDK.iOS.SBSDKUI2BarcodeFormat.MicroPdf417))
        {
            return nameof(SBSDKUI2BarcodeFormat.MicroPdf417);
        }

        throw new ArgumentException("Invalid format");
    }
}