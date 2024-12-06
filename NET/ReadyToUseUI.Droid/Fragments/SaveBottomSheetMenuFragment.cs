using Android.Views;
using Google.Android.Material.BottomSheet;
using ReadyToUseUI.Droid.Activities;
using DocumentSDK.NET.Model;

namespace ReadyToUseUI.Droid.Fragments
{
    public class SaveBottomSheetMenuFragment : BottomSheetDialogFragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.save_bottom_sheet, container, false);

            var savePdf = view.FindViewById<Button>(Resource.Id.save_pdf);
            savePdf.Text = Texts.save_pdf;
            savePdf.Click += delegate
             {
                 (Activity as PagePreviewActivity)?.SavePdf();
                 DismissAllowingStateLoss();
             };
            
            var saveSandwichPdf = view.FindViewById<Button>(Resource.Id.save_sandwich_pdf);
            saveSandwichPdf.Text = Texts.save_sandwich_pdf;
            saveSandwichPdf.Click += delegate
            {
                (Activity as PagePreviewActivity)?.SaveSandwichPdf();
                DismissAllowingStateLoss();
            };

            var performOcr = view.FindViewById<Button>(Resource.Id.save_ocr);
            performOcr.Text = Texts.perform_ocr;
            performOcr.Click += delegate
            {
                (Activity as PagePreviewActivity)?.SaveWithOcr();
                DismissAllowingStateLoss();
            };

            var saveTiff = view.FindViewById<Button>(Resource.Id.save_tiff);
            saveTiff.Text = Texts.Tiff;
            saveTiff.Click += delegate
            {
                (Activity as PagePreviewActivity).SaveTiff();
                DismissAllowingStateLoss();
            };

            return view;
        }
    }
}
