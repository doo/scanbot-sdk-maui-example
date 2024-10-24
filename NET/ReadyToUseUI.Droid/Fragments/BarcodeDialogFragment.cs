﻿using Android.Views;
using IO.Scanbot.Sdk.Barcode.Entity;
using IO.Scanbot.Sdk.Process.Model;
using ReadyToUseUI.Droid.Views;

namespace ReadyToUseUI.Droid.Fragments
{
    public class BarcodeDialogFragment : BaseDialogFragment
    {
        private const string dataTag = "BarcodeDialogFragment";

        public static BarcodeDialogFragment CreateInstance(BarcodeScanningResult data)
        {
            var fragment = new BarcodeDialogFragment();
            var args = new Bundle();
            args.PutParcelable(dataTag, data);

            fragment.Arguments = args;
            return fragment;
        }

        public override View AddContentView(LayoutInflater inflater, ViewGroup container)
        {
            var data = (BarcodeScanningResult)Arguments.GetParcelable(dataTag);
            var view = inflater.Inflate(Resource.Layout.fragment_barcode_dialog, container);

            var content = view.FindViewById<TextView>(Resource.Id.barcode_result_values);

            if (data == null || data.BarcodeItems.Count == 0)
            {
                content.Text = "No barcodes found";
                return view;
            }

            var resultText = "";
            foreach (BarcodeItem barcode in data.BarcodeItems)
            {
                resultText += barcode.BarcodeFormat.Name() + ": " + barcode.Text + "\n";
            }
            
            CopyText = resultText;
            content.Text = resultText;


            return view;
        }

        public void Show(FragmentManager manager)
        {
            Show(manager, dataTag);
        }
    }
}
