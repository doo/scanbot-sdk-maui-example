using System.Text;
using Android.Views;
using IO.Scanbot.Mcscanner.Model;
using IO.Scanbot.Sdk.Mcrecognizer.Entity;
using ReadyToUseUI.Droid.Views;

namespace ReadyToUseUI.Droid.Fragments
{
    public class MedicalCertificateResultDialogFragment : BaseDialogFragment
    {
        public const string NAME = "MedicalCertificateResultDialogFragment";
        public const string MEDICAL_CERTIFICATE_RESULT_EXTRA = "MEDICAL_CERTIFICATE_RESULT_EXTRA";

        private MedicalCertificateRecognizerResult _result;

        public static MedicalCertificateResultDialogFragment CreateInstance(MedicalCertificateRecognizerResult result)
        {
            var fragment = new MedicalCertificateResultDialogFragment();

            var args = new Bundle();
            args.PutParcelable(MEDICAL_CERTIFICATE_RESULT_EXTRA, result);
            fragment.Arguments = args;

            return fragment;
        }

        public override View AddContentView(LayoutInflater inflater, ViewGroup container)
        {
            _result = (MedicalCertificateRecognizerResult)Arguments.GetParcelable(MEDICAL_CERTIFICATE_RESULT_EXTRA);
            var view = inflater.Inflate(Resource.Layout.fragment_medical_certificate_result_dialog, container);

            CopyText = ParseData(_result);
            view.FindViewById<TextView>(Resource.Id.tv_data).Text = CopyText;
            view.FindViewById<ImageView>(Resource.Id.front_snap_result).SetImageBitmap(_result?.CroppedImage);

            return view;
        }

        private string ParseData(MedicalCertificateRecognizerResult result)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("Type: ").Append(result.Checkboxes
                   ?.FirstOrDefault(medicalCertificateInfoBox => medicalCertificateInfoBox.Type == CheckBoxType.McBoxInitialCertificate)
                   ?.HasContents == true ? "Initial" :
                   result.Checkboxes
                       ?.FirstOrDefault(medicalCertificateInfoBox => medicalCertificateInfoBox.Type == CheckBoxType.McBoxRenewedCertificate)
                       ?.HasContents == true ? "Renewed" : "Unknown").Append("\n");

            stringBuilder.Append("Work Accident: ").Append(result.Checkboxes
                ?.FirstOrDefault(medicalCertificateInfoBox => medicalCertificateInfoBox.Type == CheckBoxType.McBoxWorkAccident)
                ?.HasContents == true ? "Yes" : "No").Append("\n");

            stringBuilder.Append("Accident Consultant: ").Append(result.Checkboxes
                ?.FirstOrDefault(medicalCertificateInfoBox => medicalCertificateInfoBox.Type == CheckBoxType.McBoxAssignedToAccidentInsuranceDoctor)
                ?.HasContents == true ? "Yes" : "No").Append("\n");

            stringBuilder.Append("Start Date: ").Append(
                result.Dates?.FirstOrDefault(dateRecord => dateRecord.Type == DateRecordType.DateRecordIncapableOfWorkSince)?.DateString).Append("\n");

            stringBuilder.Append("End Date: ").Append(
                result.Dates?.FirstOrDefault(dateRecord => dateRecord.Type == DateRecordType.DateRecordIncapableOfWorkUntil)?.DateString).Append("\n");

            stringBuilder.Append("Issue Date: ").Append(
                result.Dates?.FirstOrDefault(dateRecord => dateRecord.Type == DateRecordType.DateRecordDiagnosedOn)?.DateString).Append("\n");

            stringBuilder.Append($"Form type: {result.McFormType.Name()}").Append("\n");

            stringBuilder.Append(string.Join("\n", result.PatientInfoBox.Fields.ToList().ConvertAll(field => $"{field.PatientInfoFieldType.Name()}: {field.Value}")));

            return stringBuilder.ToString();
        }
    }

}
