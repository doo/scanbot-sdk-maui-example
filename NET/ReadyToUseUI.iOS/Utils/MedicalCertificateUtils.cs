using ScanbotSDK.iOS;

namespace ReadyToUseUI.iOS.Utils;

public static class MedicalCertificateResultUtils
{
    internal static NSAttributedString ToFormattedAttributeString(this SBSDKMedicalCertificateScanningResult result)
    {
        var mutableString = new NSMutableAttributedString();

        mutableString.Append(GetBoldAttributedString("FormType:"));
        mutableString.Append(GetRegularAttributedString(result.FormType + "\n \n"));
            
        if (result.PatientInfoBox?.Fields != null && result.PatientInfoBox.Fields.Length > 0)
        {
            mutableString.Append(ExtractPatientInfo(result.PatientInfoBox.Fields));
        }

        if (result.Dates != null && result.Dates.Length > 0)
        {
            mutableString.Append(ExtractDatesInfo(result.Dates));
        }

        if (result.CheckBoxes != null && result.CheckBoxes.Length > 0)
        {
            mutableString.Append(ExtractCheckBoxInfo(result.CheckBoxes));
        }

        return mutableString;
    }
    
    private static NSAttributedString ExtractPatientInfo(SBSDKMedicalCertificatePatientInfoField[] fields)
    {
        var mutableString = new NSMutableAttributedString();
        mutableString.Append(GetBoldAttributedString("Patient Information:\n"));
        foreach (var field in fields)
        {
            mutableString.Append(GetBoldAttributedString(field.Type + ":"));
            mutableString.Append(GetRegularAttributedString(field.Value));
        }

        return mutableString;
    }
    
    private static NSAttributedString ExtractDatesInfo(SBSDKMedicalCertificateDateRecord[] resultDates)
    {
        var mutableString = new NSMutableAttributedString();
        mutableString.Append(GetBoldAttributedString("\nDate Information:"));
        foreach (var record in resultDates)
        {
            mutableString.Append(GetBoldAttributedString(record.Type + ":"));
            mutableString.Append(GetRegularAttributedString(record.Value));
        }

        return mutableString;
    }

    private static NSAttributedString ExtractCheckBoxInfo(SBSDKMedicalCertificateCheckBox[] resultCheckBoxes)
    {
        var mutableString = new NSMutableAttributedString();
        mutableString.Append(GetBoldAttributedString("\nCheckBox Fields:"));
        foreach (var checkBox in resultCheckBoxes)
        {
            mutableString.Append(GetBoldAttributedString(checkBox.Type + ":"));
            mutableString.Append(GetRegularAttributedString(checkBox.Checked ? "Yes" : "No"));
        }

        return mutableString;
    }

    private static NSAttributedString GetRegularAttributedString(string text)
    {
        return new NSAttributedString(text + "\n", new UIStringAttributes
        {
            Font = UIFont.SystemFontOfSize(16)
        });
    }
    
    private static NSAttributedString GetBoldAttributedString(string text)
    {
        return new NSAttributedString(text + "\n", new UIStringAttributes
        {
            Font = UIFont.BoldSystemFontOfSize(16)
        });
    }
}