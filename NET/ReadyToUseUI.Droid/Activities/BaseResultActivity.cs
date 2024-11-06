// using Android.Graphics;
// using Android.Views;
// using AndroidX.AppCompat.App;
//
// namespace ReadyToUseUI.Droid.Activities;
//
// public class BaseResultActivity<TNativeBarcodeResult> : AppCompatActivity where TNativeBarcodeResult : global::Java.Lang.Object, global::Android.OS.IParcelable
// {
//     protected override void OnCreate(Bundle savedInstanceState)
//     {
//         base.OnCreate(savedInstanceState);
//         SetContentView(Resource.Layout.barcode_result);
//         SetupToolbar();
//         DisplayBarcodeResult();
//     }
//
//     protected void SetupToolbar()
//     {
//         var toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
//         SetSupportActionBar(toolbar);
//     }
//
//     protected virtual BaseBarcodeResult<TNativeBarcodeResult> DisplayBarcodeResult()
//     {
//         var barcodeResult = new BaseBarcodeResult<TNativeBarcodeResult>().FromBundle(Intent?.GetBundleExtra("BarcodeResult"));
//         string imagePath = barcodeResult.PreviewPath ?? barcodeResult.ImagePath;
//
//         if (!string.IsNullOrEmpty(imagePath))
//         {
//             ShowSnapImage(imagePath);
//         }
//         else if (barcodeResult.ResultBitmap != null)
//         {
//             ShowSnapImage(barcodeResult);
//         }
//
//         return barcodeResult;
//     }
//
//     protected void ShowSnapImage(string path) => AddImageView().SetImageURI(Android.Net.Uri.Parse(path));
//
//     protected void ShowSnapImage(BaseBarcodeResult<TNativeBarcodeResult> barcodeResult)
//     {
//         Bitmap scaled = Bitmap.CreateScaledBitmap(barcodeResult.ResultBitmap, 200, 200, true);
//         AddImageView().SetImageBitmap(scaled);
//     }
//
//     protected ImageView AddImageView()
//     {
//         var items = FindViewById<LinearLayout>(Resource.Id.recognizedItems);
//         var view = LayoutInflater.Inflate(Resource.Layout.snap_image_item, items, false);
//         items.AddView(view);
//         return view.FindViewById<ImageView>(Resource.Id.snapImage);
//     }
//     
//     protected FrameLayout GetSeparatorView()
//     {
//         var separatorView = new FrameLayout(this);
//         separatorView.LayoutParameters = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, 2);
//         separatorView.SetPadding(20, 0, 10, 0);
//         separatorView.SetBackgroundColor(Color.Gray);
//         return separatorView;
//     }
// }