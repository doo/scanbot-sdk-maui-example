using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;
using ScanbotSdkExample.Droid.Fragments;
using ScanbotSdkExample.Droid.Listeners;
using ScanbotSdkExample.Droid.Utils;
using ScanbotSdkExample.Droid.Model;
using IO.Scanbot.Sdk.Docprocessing;
using IO.Scanbot.Sdk.Imagefilters;
using IO.Scanbot.Sdk.Ui_v2.Common;
using IO.Scanbot.Sdk.Ui_v2.Document;
using IO.Scanbot.Sdk.Ui_v2.Document.Configuration;

namespace ScanbotSdkExample.Droid.Activities
{
    internal class PageModel
    {
        public string DocumentId { get; set; }
        
        public string PageId { get; set; }
        
        public Android.Net.Uri OriginalPagePreviewUri { get; set; }
        
        public Android.Net.Uri OriginalPageUri { get; set; }
        
        public Android.Net.Uri ScannedPageUri { get; set; }
        
        public Android.Net.Uri ScannedPagePreviewUri { get; set; }
    }
    
    [Activity]
    public partial class PagePreviewActivity : AppCompatActivity, IFiltersListener
    {
        const int FILTER_UI_REQUEST_CODE = 7777;
        const int CAMERA_ACTIVITY = 8888;

        const string FILTERS_MENU_TAG = "FILTERS_MENU_TAG";
        const string SAVE_MENU_TAG = "SAVE_MENU_TAG";

        private IO.Scanbot.Sdk.ScanbotSDK scanbotSDK;
        private Document document;
        
        PageAdapter adapter;
        RecyclerView recycleView;

        FilterListFragment filterFragment;
        SaveBottomSheetMenuFragment saveFragment;

        ProgressBar progress;
        private TextView crop, filter, quality; 
        Button export;

        internal static Intent CreateIntent(Context context, string documentId)
        {
            var intent = new Intent(context, typeof(PagePreviewActivity));
            intent.PutExtra(nameof(documentId), documentId);
            return intent;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            scanbotSDK = new IO.Scanbot.Sdk.ScanbotSDK(this);
            SetContentView(Resource.Layout.activity_page_preview);
            var documentId = Intent.GetStringExtra("documentId");
            document = scanbotSDK.DocumentApi.LoadDocument(documentId);
            SetupToolbar();

            filterFragment = new FilterListFragment();
            saveFragment = new SaveBottomSheetMenuFragment();

            var fragmentFilterMenu = SupportFragmentManager.FindFragmentByTag(FILTERS_MENU_TAG);
            if (fragmentFilterMenu != null)
            {
                SupportFragmentManager.BeginTransaction().Remove(fragmentFilterMenu).CommitNow();
            }

            var fragmentSaveMenu = SupportFragmentManager.FindFragmentByTag(SAVE_MENU_TAG);
            if (fragmentSaveMenu != null)
            {
                SupportFragmentManager.BeginTransaction().Remove(fragmentSaveMenu).CommitNow();
            }

            progress = FindViewById<ProgressBar>(Resource.Id.progressBar);

            adapter = new PageAdapter(scanbotSDK.FileIOProcessor(), document);
            adapter.HasStableIds = true;
            adapter.Context = this;

            recycleView = FindViewById<RecyclerView>(Resource.Id.pages_preview);
            recycleView.HasFixedSize = true;
            recycleView.SetAdapter(adapter);
            recycleView.SetLayoutManager(new GridLayoutManager(this, 3));

            // Bottom Toolbar
            
            crop = FindViewById<TextView>(Resource.Id.action_crop);
            crop.Text = Texts.crop;
            crop.Click += delegate
            {
                var pageId = scanbotSDK.DocumentApi.LoadDocument(documentId)?.PageAtIndex(0)?.Uuid;
                var configurations = CroppingActivityConfiguration.Init(documentId, pageId);
                
                configurations.Appearance.TopBarBackgroundColor = new ScanbotColor(Android.Graphics.Color.Red);
                configurations.Cropping.TopBarConfirmButton.Foreground.Color = new ScanbotColor(Android.Graphics.Color.Red);
                
                // e.g. disable the rotation feature.
                configurations.Cropping.BottomBar.RotateButton.Visible = false;

                // e.g. configure various colors.
                configurations.Appearance.TopBarBackgroundColor = new ScanbotColor(Color.Red);
                configurations.Cropping.TopBarConfirmButton.Foreground.Color = new ScanbotColor(Color.White);

                // e.g. customize a UI element's text.
                configurations.Localization.CroppingTopBarCancelButtonTitle = "Cancel";
                
                var intent = CroppingActivity.NewIntent(this, configurations);
                StartActivityForResult(intent, CAMERA_ACTIVITY);
            };

            quality = FindViewById<TextView>(Resource.Id.action_document_quality);
            quality.Text = Texts.check_document_quality;
            quality.Click += delegate
             {
                 var bitmap = scanbotSDK.DocumentApi.LoadDocument(documentId)?.PageAtIndex(0)?.DocumentImage;
                 if (bitmap == null)
                     return;

                 var qualityAnalyzer = scanbotSDK.CreateDocumentQualityAnalyzer();
                 var documentQualityResult = qualityAnalyzer.AnalyzeOnBitmap(bitmap, 0);
                 Alert.ShowAlert(this, "Document Quality", documentQualityResult.Quality.Name());
             };

            filter = FindViewById<TextView>(Resource.Id.action_filter);
            filter.Text = Texts.filter;
            filter.Click += delegate
            {
                filterFragment.Show(SupportFragmentManager, FILTERS_MENU_TAG);
            };

            // Top Toolbar
            export = FindViewById<Button>(Resource.Id.action_export_document);
            export.Text = Texts.export;
            export.Click += delegate
            {
                saveFragment.Show(SupportFragmentManager, SAVE_MENU_TAG);
            };
        }

        private void SetupToolbar()
        {
            var toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            SupportActionBar.Title = Texts.scan_results;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            
            document = scanbotSDK.DocumentApi.LoadDocument(document.Uuid); // refresh from memory
            adapter.Refresh(document);
            UpdateVisibility();
        }

        protected override void OnResume()
        {
            base.OnResume();

            if (!scanbotSDK.LicenseInfo.IsValid)
            {
                Alert.ShowLicenseDialog(this);
            }

            UpdateVisibility();
        }

        private void UpdateVisibility()
        {
            quality.Visibility = adapter.ItemCount == 1 ? ViewStates.Visible : ViewStates.Invisible;
            crop.Visibility = adapter.ItemCount == 1 ? ViewStates.Visible : ViewStates.Invisible;
            filter.Enabled = !adapter.IsEmpty;
        }

        private Android.Net.Uri GetOutputUri(string extension)
        {
            var external = GetExternalFilesDir(null).AbsolutePath;
            var filename = Guid.NewGuid() + extension;
            var targetFile = System.IO.Path.Combine(external, filename);
            return Android.Net.Uri.FromFile(new Java.IO.File(targetFile));
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

        public void ApplyFilter(ParametricFilter selectedFilter)
        {
            foreach (var page in document.Pages)
            {
                page.Apply(page.Rotation, page.Polygon, new[] { selectedFilter });
            }
            adapter.Refresh(document);                                          
        }
    }

    class PageAdapter : RecyclerView.Adapter
    {
        private IO.Scanbot.Sdk.Persistence.Fileio.IFileIOProcessor fileProcessor;
        private List<PageModel> _pages = new List<PageModel>();
        public PageAdapter(IO.Scanbot.Sdk.Persistence.Fileio.IFileIOProcessor fileProcessor, Document document)
        {
            this.fileProcessor = fileProcessor;
            Refresh(document);
        }

        public void Refresh(Document document)
        {
            _pages.Clear();

            if (document?.Pages != null)
            {
                foreach (var page in document.Pages)
                {
                    _pages.Add(new PageModel
                    {
                        PageId = page.Uuid,
                        DocumentId = document.Uuid,
                        ScannedPageUri = page.DocumentFileUri,
                        ScannedPagePreviewUri = page.DocumentPreviewFileUri,
                        OriginalPageUri = page.OriginalFileUri
                    });
                }
            }

            NotifyDataSetChanged();
        }

        public PageModel PageIdForIndex(int index)
        {
            return _pages[index];
        }

        public Context Context;
        
        public override int ItemCount => _pages.Count;

        public bool IsEmpty { get => ItemCount == 0; }

        public override long GetItemId(int position)
        {
            return _pages[position].GetHashCode();
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = LayoutInflater.From(Context).Inflate(Resource.Layout.item_page, parent, false);
            return new PageViewHolder(view);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var page = _pages[position];
            (holder as PageViewHolder).image.SetImageResource(0);

            var options = new BitmapFactory.Options();
            if (File.Exists(page.ScannedPagePreviewUri.Path))
            {
                var bitmap = fileProcessor.ReadImage(page.ScannedPagePreviewUri, options);

                (holder as PageViewHolder).image.SetImageBitmap(bitmap);
            }
            else
            {
                var bitmap = fileProcessor.ReadImage(page.OriginalPagePreviewUri, options);
                (holder as PageViewHolder).image.SetImageBitmap(bitmap);
            }
        }
    }

    class PageViewHolder : RecyclerView.ViewHolder
    {
        public ImageView image;
        public PageViewHolder(View item) : base(item)
        {
            image = item.FindViewById<ImageView>(Resource.Id.page);
        }
    }
}

