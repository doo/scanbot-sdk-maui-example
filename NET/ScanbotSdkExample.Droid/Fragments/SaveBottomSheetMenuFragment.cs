using Android.Views;
using Google.Android.Material.BottomSheet;
using ScanbotSdkExample.Droid.Activities;
using ScanbotSdkExample.Droid.Model;
using R = _Microsoft.Android.Resource.Designer.ResourceConstant;

namespace ScanbotSdkExample.Droid.Fragments
{
    public class SaveBottomSheetMenuFragment : BottomSheetDialogFragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(R.Layout.save_bottom_sheet, container, false)!;

            var savePdf = (Button)view.FindViewById(R.Id.save_pdf)!;
            savePdf.Text = Texts.SavePdf;
            savePdf.Click += delegate
             {
                 (Activity as PagePreviewActivity)?.SavePdf();
                 DismissAllowingStateLoss();
             };
            
            var saveSandwichPdf = (Button)view.FindViewById(R.Id.save_sandwich_pdf)!;
            saveSandwichPdf.Text = Texts.SaveSandwichPdf;
            saveSandwichPdf.Click += delegate
            {
                (Activity as PagePreviewActivity)?.SaveSandwichPdf();
                DismissAllowingStateLoss();
            };

            var performOcr = (Button)view.FindViewById(R.Id.save_ocr)!;
            performOcr.Text = Texts.PerformOcr;
            performOcr.Click += delegate
            {
                (Activity as PagePreviewActivity)?.SaveWithOcr();
                DismissAllowingStateLoss();
            };

            var saveTiff =  (Button)view.FindViewById(R.Id.save_tiff)!;
            saveTiff.Text = Texts.Tiff;
            saveTiff.Click += delegate
            {
                (Activity as PagePreviewActivity)?.SaveTiff();
                DismissAllowingStateLoss();
            };

            return view;
        }
    }
}
