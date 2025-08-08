using System.Text;
using ScanbotSDK.MAUI.Core.MedicalCertificate;

namespace ScanbotSdkExample.Maui.Utils;
public static class StringUtils
{
    public static string CopyrightLabel =>
        $"Copyright (c) {DateTime.Now.Year} Scanbot SDK GmbH. All rights reserved";
        
        
    public static string ToFormattedString(this MedicalCertificateScanningResult result)
    {
        StringBuilder stringBuilder = new StringBuilder();
            
        stringBuilder.Append("Type: ").Append(result.CheckBoxes?.FirstOrDefault(medicalCertificateInfoBox => medicalCertificateInfoBox.Type == MedicalCertificateCheckBoxType.InitialCertificate) != null ? "Initial" :
            result.CheckBoxes?.FirstOrDefault(medicalCertificateInfoBox => medicalCertificateInfoBox.Type == MedicalCertificateCheckBoxType.RenewedCertificate) != null ? "Renewed" : "Unknown").Append("\n");
            
        stringBuilder.Append("Work Accident: ").Append(result.CheckBoxes
            ?.FirstOrDefault(medicalCertificateInfoBox => medicalCertificateInfoBox.Type == MedicalCertificateCheckBoxType.WorkAccident) != null ? "Yes" : "No").Append("\n");
            
        stringBuilder.Append("Accident Consultant: ").Append(result.CheckBoxes
                                                                 ?.FirstOrDefault(medicalCertificateInfoBox => medicalCertificateInfoBox.Type == MedicalCertificateCheckBoxType.AssignedToAccidentInsuranceDoctor)
                                                             != null ? "Yes" : "No").Append("\n");
            
        stringBuilder.Append("Start Date: ").Append(result.Dates?.FirstOrDefault(dateRecord => dateRecord.Type == MedicalCertificateDateRecordType.IncapableOfWorkSince)?.RawString).Append("\n");
            
        stringBuilder.Append("End Date: ").Append(result.Dates?.FirstOrDefault(dateRecord => dateRecord.Type ==  MedicalCertificateDateRecordType.IncapableOfWorkUntil)?.RawString).Append("\n");
            
        stringBuilder.Append("Issue Date: ").Append(result.Dates?.FirstOrDefault(dateRecord => dateRecord.Type == MedicalCertificateDateRecordType.DiagnosedOn)?.RawString).Append("\n");
            
        stringBuilder.Append($"Form type: {result.FormType}").Append("\n");
            
        stringBuilder.Append(string.Join("\n", result.PatientInfoBox.Fields.ToList().ConvertAll(field => $"{field.Type}: {field.Value}")));
            
        return stringBuilder?.ToString();
    }
        
}