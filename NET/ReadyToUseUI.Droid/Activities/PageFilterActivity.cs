using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.App;
using IO.Scanbot.Sdk.Persistence;
using IO.Scanbot.Sdk.Process;
using IO.Scanbot.Sdk.UI.View.Edit;
using IO.Scanbot.Sdk.UI.View.Edit.Configuration;
using ReadyToUseUI.Droid.Fragments;
using ReadyToUseUI.Droid.Listeners;
using ReadyToUseUI.Droid.Utils;
using DocumentSDK.NET.Model;
using IO.Scanbot.Imagefilters;

namespace ReadyToUseUI.Droid.Activities
{
    [Activity]
    public class PageFilterActivity : AppCompatActivity, IFiltersListener
    {
        const string FILTERS_MENU_TAG = "FILTERS_MENU_TAG";
        const string CHOOSE_FILTERS_DIALOG_TAG = "CHOOSE_FILTERS_DIALOG_TAG";
        const int CROP_DEFAULT_UI_REQUEST_CODE = 9999;

        TextView crop, delete, checkQuality, filter;

        public static Intent CreateIntent(Context context, string pageId)
        {
            var intent = new Intent(context, typeof(PageFilterActivity));
            intent.PutExtra(nameof(selectedPageId), pageId);
            return intent;
        }

        private string selectedPageId;
        private LegacyFilter selectedFilter;
        // private FilterBottomSheetMenuFragment filterFragment;
        private FilterListFragment filterFragment;
        private ProgressBar progress;
        private IO.Scanbot.Sdk.ScanbotSDK scanbotSDK;
        private IO.Scanbot.Sdk.Persistence.PageFileStorage pageStorage;
        private IO.Scanbot.Sdk.Docprocessing.PageProcessor pageProcessor;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            scanbotSDK = new IO.Scanbot.Sdk.ScanbotSDK(this);
            pageStorage = scanbotSDK.CreatePageFileStorage();
            pageProcessor = scanbotSDK.CreatePageProcessor();

            SetContentView(Resource.Layout.activity_filters);

            SetupToolbar();

            progress = FindViewById<ProgressBar>(Resource.Id.progress);

            selectedPageId = Intent.GetStringExtra(nameof(selectedPageId));

            crop = FindViewById<TextView>(Resource.Id.action_crop_and_rotate);
            crop.Text = Texts.crop_amp_rotate;

            filter = FindViewById<TextView>(Resource.Id.action_filter);
            filter.Text = Texts.filter;
            filter.Click += delegate
            {
               filterFragment.Show(SupportFragmentManager, CHOOSE_FILTERS_DIALOG_TAG);
            };

            delete = FindViewById<TextView>(Resource.Id.action_delete);
            delete.Text = Texts.delete;
            delete.Click += delegate
            {
                pageStorage.Remove(selectedPageId);
                Finish();
            };

            checkQuality = FindViewById<TextView>(Resource.Id.action_check_quality);
            checkQuality.Text = Texts.check_document_quality;
            checkQuality.Click += delegate
            {
                var bitmap = pageStorage.GetPreviewImage(selectedPageId, PageFileStorage.PageFileType.Document, null);
                var qualityAnalyzer = scanbotSDK.CreateDocumentQualityAnalyzer();
                var documentQualityResult = qualityAnalyzer.AnalyzeInBitmap(bitmap, 0);
                Alert.ShowAlert(this, "Document Quality", documentQualityResult.Name());
            };

            FindViewById(Resource.Id.action_crop_and_rotate).Click += delegate
            {
                var configuration = new CroppingConfiguration(new Page().Copy(pageId: selectedPageId));
                configuration.SetPolygonColor(Color.Red);
                configuration.SetPolygonColorMagnetic(Color.Blue);

                var intent = CroppingActivity.NewIntent(this, configuration);
                StartActivityForResult(intent, CROP_DEFAULT_UI_REQUEST_CODE);
            };

            var fragmentFilterMenu = SupportFragmentManager.FindFragmentByTag(FILTERS_MENU_TAG);
            if (fragmentFilterMenu != null)
            {
                SupportFragmentManager.BeginTransaction().Remove(fragmentFilterMenu).CommitNow();
            }

            filterFragment = new FilterListFragment();//FilterBottomSheetMenuFragment();

            if (!scanbotSDK.LicenseInfo.IsValid)
            {
                Alert.ShowLicenseDialog(this);
            }
            else
            {
                GeneratePreviewImage();
            }
        }

        private void SetupToolbar()
        {
            var toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            SupportActionBar.Title = Texts.page_title;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);
        }

        private void GeneratePreviewImage()
        {
            progress.Visibility = ViewStates.Visible;

            Task.Run(delegate
            {
                var uri = pageStorage.GetFilteredPreviewImageURI(selectedPageId, new LegacyFilter(ImageFilterType.None.Code)); 

                if (!File.Exists(uri.Path))
                {
                    pageProcessor.GenerateFilteredPreview(new Page().Copy(pageId: selectedPageId),  new LegacyFilter(ImageFilterType.None.Code));
                }

                UpdateImage(uri);
            });
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode != Result.Ok)
            {
                return;
            }

            if (requestCode == CROP_DEFAULT_UI_REQUEST_CODE)
            {
                selectedFilter = selectedFilter ?? new LegacyFilter(ImageFilterType.None.Code);

                var uri = pageStorage.GetFilteredPreviewImageURI(selectedPageId, selectedFilter);

                if (!File.Exists(uri.Path))
                {
                    scanbotSDK.CreatePageProcessor().GenerateFilteredPreview(new Page().Copy(pageId: selectedPageId), selectedFilter);
                }

                UpdateImage(uri);
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
            {
                base.OnBackPressed();
                return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        public void ApplyFilter(ImageFilterType type)
        {
            progress.Visibility = ViewStates.Visible;
            selectedFilter = new LegacyFilter(type.Code);
            Task.Run(delegate
            {
                var pageToFilter = new Page().Copy(pageId: selectedPageId);
                pageProcessor.ApplyFilter(pageToFilter, selectedFilter);
                pageProcessor.GenerateFilteredPreview(pageToFilter, selectedFilter);
                var uri = pageStorage.GetFilteredPreviewImageURI(selectedPageId, selectedFilter);
                UpdateImage(uri);
            });
        }

        private void UpdateImage(Android.Net.Uri uri)
        {
            RunOnUiThread(delegate
            {
                var image = FindViewById<ImageView>(Resource.Id.image);
                image.SetImageBitmap(null);
                image.SetImageBitmap(scanbotSDK.FileIOProcessor().ReadImage(uri, new BitmapFactory.Options()));
                progress.Visibility = ViewStates.Gone;
            });
        }
    }
}
