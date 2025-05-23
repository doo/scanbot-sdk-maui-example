using Android.Graphics;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using IO.Scanbot.Sdk.MC;
using ScanbotSdkExample.Droid.Views;

namespace ScanbotSdkExample.Droid.Fragments
{
    public class MedicalCertificateResultDialogFragment : BaseDialogFragment
    {
        public const string NAME = "MedicalCertificateResultDialogFragment";
        public const string MEDICAL_CERTIFICATE_RESULT_EXTRA = "MEDICAL_CERTIFICATE_RESULT_EXTRA";

        private MedicalCertificateScanningResult _result;

        public static MedicalCertificateResultDialogFragment CreateInstance(MedicalCertificateScanningResult result)
        {
            var fragment = new MedicalCertificateResultDialogFragment();

            var args = new Bundle();
            args.PutParcelable(MEDICAL_CERTIFICATE_RESULT_EXTRA, result);
            fragment.Arguments = args;

            return fragment;
        }

        public override View AddContentView(LayoutInflater inflater, ViewGroup container)
        {
            _result = (MedicalCertificateScanningResult)Arguments.GetParcelable(MEDICAL_CERTIFICATE_RESULT_EXTRA);
            var view = inflater.Inflate(Resource.Layout.fragment_medical_certificate_result_dialog, container);

            view.FindViewById<TextView>(Resource.Id.tv_data).TextFormatted = ParseData(_result);
            view.FindViewById<ImageView>(Resource.Id.front_snap_result).SetImageBitmap(_result?.CroppedImage.ToBitmap());

            return view;
        }

        private SpannableStringBuilder ParseData(MedicalCertificateScanningResult result)
        {
            var sb = new SpannableStringBuilder();

            sb.Append(GetBoldSpanString("FormType:"));
            sb.Append(GetRegularSpanString(result.FormType + "\n \n"));
            
            if (result.PatientInfoBox?.Fields != null && result.PatientInfoBox.Fields.Count > 0)
            {
                sb.Append(ExtractPatientInfo(result.PatientInfoBox.Fields));
            }

            if (result.Dates != null && result.Dates.Count > 0)
            {
                sb.Append(ExtractDatesInfo(result.Dates));
            }

            if (result.CheckBoxes != null && result.CheckBoxes.Count > 0)
            {
                sb.Append(ExtractCheckBoxInfo(result.CheckBoxes));
            }
           
            return sb;
        }

        private SpannableStringBuilder ExtractPatientInfo(IList<MedicalCertificatePatientInfoField> fields)
        {
            var sb = new SpannableStringBuilder();
            sb.Append(GetBoldSpanString("Patient Information:\n"));
            foreach (var field in fields)
            {
                sb.Append(GetBoldSpanString(field.Type.Name() + ":"));
                sb.Append(GetRegularSpanString(field.Value));
            }

            return sb;
        }
        
        private SpannableStringBuilder ExtractDatesInfo(IList<MedicalCertificateDateRecord> resultDates)
        {
            var sb = new SpannableStringBuilder();
            sb.Append(GetBoldSpanString("\nDate Information:"));
            foreach (var record in resultDates)
            {
                sb.Append(GetBoldSpanString(record.Type.Name() + ":"));
                sb.Append(GetRegularSpanString(record.Value));
            }

            return sb;
        }

        private SpannableStringBuilder ExtractCheckBoxInfo(IList<MedicalCertificateCheckBox> resultCheckBoxes)
        {
            var sb = new SpannableStringBuilder();
            sb.Append(GetBoldSpanString("\nCheckBox Fields:"));
            foreach (var checkBox in resultCheckBoxes)
            {
                sb.Append(GetBoldSpanString(checkBox.Type.Name() + ":"));
                sb.Append(GetRegularSpanString(checkBox.Checked ? "Yes" : "No"));
            }

            return sb;
        }

        private SpannableString GetBoldSpanString(string text)
        {
            var boldString = new SpannableString(text + "\n");
            boldString.SetSpan(new StyleSpan(TypefaceStyle.Bold), 0, text.Length, SpanTypes.ExclusiveExclusive);
            return boldString;
        }
        
        private SpannableString GetRegularSpanString(string text)
        {
            var regularString = new SpannableString(text + "\n");
            regularString.SetSpan(new StyleSpan(TypefaceStyle.Normal), 0, text.Length, SpanTypes.ExclusiveExclusive);
            return regularString;
        }
    }
}
