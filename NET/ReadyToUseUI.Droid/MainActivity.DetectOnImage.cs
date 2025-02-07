using Android.Content;
using DocumentSDK.NET.Model;
using ReadyToUseUI.Droid.Fragments;
using ReadyToUseUI.Droid.Utils;

namespace ReadyToUseUI.Droid;

public partial class MainActivity
{
    private Dictionary<int, Action<Intent>> detectOnImageActions => new Dictionary<int, Action<Intent>>
    {
        { DETECT_MRZ_FROM_IMAGE, DetectMrzFromImage },
        { DETECT_EHIC_FROM_IMAGE, DetectEHICFromImage },
        { DETECT_CHECK_FROM_IMAGE, DetectCheckFromImage },
        { DETECT_MEDICAL_CERTIFICATE_FROM_IMAGE, DetectMedicalCertificateFromImage },
        { DETECT_GDR_FROM_IMAGE, DetectGenericDocumentFromImage },
    };

    private void LaunchImagePicker(int activityRequestCode)
    {
        var intent = new Intent();
        intent.SetType("image/*");
        intent.SetAction(Intent.ActionGetContent);
        intent.PutExtra(Intent.ExtraLocalOnly, false);
        intent.PutExtra(Intent.ExtraAllowMultiple, false);

        var chooser = Intent.CreateChooser(intent, Texts.share_title);
        StartActivityForResult(chooser, activityRequestCode);
    }

    private void DetectMrzFromImage(Intent data)
    {
        var bitmap = ImageUtils.ProcessGalleryResult(this, data);
        var recognizer = scanbotSDK.CreateMrzScanner();
        var result = recognizer.RecognizeMRZBitmap(bitmap, 0);

        if (result?.Document == null) return;

        var fragment = MRZDialogFragment.CreateInstance(result);
        fragment.Show(FragmentManager, MRZDialogFragment.NAME);
    }

    private void DetectCheckFromImage(Intent data)
    {
        var bitmap = ImageUtils.ProcessGalleryResult(this, data);
        var recognizer = scanbotSDK.CreateCheckRecognizer();
        var result = recognizer.RecognizeBitmap(bitmap, 0);
        var fields = result.Check.Fields;
        var description = string.Join(";\n", fields
                            .Where(field => field != null)
                            .Select((field) =>
                                    {
                                        string outStr = "";
                                        if (field.GetType() != null && field.GetType().Name != null)
                                        {
                                            outStr += field.GetType().Name + " = ";
                                        }
                                        if (field.Value != null && field.Value.Text != null)
                                        {
                                            outStr += field.Value.Text;
                                        }
                                        return outStr;
                                    })
                            .ToList());

        Alert.ShowAlert(this, "Result", description);
    }

    private void DetectMedicalCertificateFromImage(Intent data)
    {
        var bitmap = ImageUtils.ProcessGalleryResult(this, data);
        var recognizer = scanbotSDK.CreateMedicalCertificateRecognizer();
        var result = recognizer.RecognizeMcBitmap(image: bitmap,
            orientation: 0,
            shouldCropDocument: true,
            returnCroppedDocument: true,
            recognizePatientInfo: true,
            recognizeBarcode: true);

        if (result == null) return;

        var fragment = MedicalCertificateResultDialogFragment.CreateInstance(result);
        fragment.Show(FragmentManager, MedicalCertificateResultDialogFragment.NAME);
    }

    private void DetectEHICFromImage(Intent data)
    {
        var bitmap = ImageUtils.ProcessGalleryResult(this, data);
        var recognizer = scanbotSDK.CreateHealthInsuranceCardScanner();
        var result = recognizer.RecognizeBitmap(bitmap, 0);

        if (result == null) return;

        var fragment = HealthInsuranceCardFragment.CreateInstance(result);
        fragment.Show(FragmentManager, HealthInsuranceCardFragment.NAME);
    }

    private void DetectGenericDocumentFromImage(Intent data)
    {
        var bitmap = ImageUtils.ProcessGalleryResult(this, data);
        var recognizer = scanbotSDK.CreateGenericDocumentRecognizer();
        var result = recognizer.ScanBitmap(bitmap, true, 0);

        if (result?.Document == null) return;

        var description = string.Join(";\n", result.Document.Fields
                            .Where(field => field != null)
                            .Select(field =>
                                    {
                                        string typeName = field.GetType().Name;
                                        string valueText = field.Value?.Text;
                                        return !string.IsNullOrEmpty(typeName) && !string.IsNullOrEmpty(valueText)
                                                            ? $"{typeName} = {valueText}"
                                                            : null;
                                    })
                            .Where(outStr => outStr != null)
                            .ToList()
        );

        Alert.ShowAlert(this, "Result", description);
    }
}