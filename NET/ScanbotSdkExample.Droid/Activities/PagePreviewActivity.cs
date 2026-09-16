using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;
using ScanbotSdkExample.Droid.Fragments;
using ScanbotSdkExample.Droid.Listeners;
using ScanbotSdkExample.Droid.Utils;
using IO.Scanbot.Sdk.Docprocessing;
using IO.Scanbot.Sdk.Documentqualityanalyzer;
using IO.Scanbot.Sdk.Image;
using IO.Scanbot.Sdk.Imageprocessing;
using IO.Scanbot.Sdk.Ui_v2.Common;
using IO.Scanbot.Sdk.Ui_v2.Document;
using IO.Scanbot.Sdk.Ui_v2.Document.Configuration;
using ScanbotSDK.Droid.Helpers;
using ScanbotSdkExample.Droid.Model;
using R = _Microsoft.Android.Resource.Designer.ResourceConstant;
using Result = Android.App.Result;

namespace ScanbotSdkExample.Droid.Activities;
[Activity]
public partial class PagePreviewActivity : AppCompatActivity, IFiltersListener
{
    private const int CameraActivity = 8888;
    private const int DocumentCleanUpdRequestCode = 001;
    private const string FiltersMenuTag = "FILTERS_MENU_TAG";
    private const string SaveMenuTag = "SAVE_MENU_TAG";

    private IO.Scanbot.Sdk.ScanbotSDK _scanbotSdk;
    private Document _document;

    private PageAdapter _adapter;
    private RecyclerView _recycleView;

    private FilterListFragment _filterFragment;
    private SaveBottomSheetMenuFragment _saveFragment;

    private TextView _crop, _filter, _quality, _cleanUp;
    private Button _export;

    internal static Intent CreateIntent(Context context, string documentId)
    {
        var intent = new Intent(context, typeof(PagePreviewActivity));
        intent.PutExtra(nameof(documentId), documentId);
        return intent;
    }

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        _scanbotSdk = new IO.Scanbot.Sdk.ScanbotSDK(this);
        SetContentView(R.Layout.activity_page_preview);
        var documentId = Intent?.GetStringExtra("documentId");
        _document = _scanbotSdk.DocumentApi.LoadDocument(documentId).GetOrThrow<Document>();;
        SetupToolbar();

        _filterFragment = new FilterListFragment();
        _saveFragment = new SaveBottomSheetMenuFragment();

        var fragmentFilterMenu = SupportFragmentManager.FindFragmentByTag(FiltersMenuTag);
        if (fragmentFilterMenu != null)
        {
            SupportFragmentManager.BeginTransaction().Remove(fragmentFilterMenu).CommitNow();
        }

        var fragmentSaveMenu = SupportFragmentManager.FindFragmentByTag(SaveMenuTag);
        if (fragmentSaveMenu != null)
        {
            SupportFragmentManager.BeginTransaction().Remove(fragmentSaveMenu).CommitNow();
        }

        _adapter = new PageAdapter(_scanbotSdk.FileIOProcessor(), _document);
        _adapter.HasStableIds = true;
        _adapter.Context = this;

        _recycleView = (RecyclerView)FindViewById(R.Id.pages_preview)!;
        _recycleView.HasFixedSize = true;
        _recycleView.SetAdapter(_adapter);
        _recycleView.SetLayoutManager(new GridLayoutManager(this, 3));

        // Bottom Toolbar
            
        _crop = (TextView)FindViewById(R.Id.action_crop)!;
        _crop.Text = Texts.Crop;
        _crop.Click += delegate
        {
            var document = _scanbotSdk.DocumentApi.LoadDocument(documentId)?.GetOrThrow<Document>();
            var pageId = document.PageAtIndex(0)?.Uuid;
            var configurations = CroppingActivityConfiguration.Init(documentId, pageId);
                
            configurations.Appearance.TopBarBackgroundColor = new ScanbotColor(Color.Red);
            configurations.Cropping.TopBarConfirmButton.Foreground.Color = new ScanbotColor(Color.Red);
                
            // e.g. enable/disable the rotation feature.
            configurations.Cropping.Toolbar.RotateButton.Visible = true;

            // e.g. configure various colors.
            configurations.Appearance.TopBarBackgroundColor = new ScanbotColor(Color.Red);
            configurations.Cropping.TopBarConfirmButton.Foreground.Color = new ScanbotColor(Color.White);

            // e.g. customize a UI element's text.
            configurations.Localization.CroppingTopBarCancelButtonTitle = "Cancel";
                
            var intent = CroppingActivity.NewIntent(this, configurations);
            StartActivityForResult(intent, CameraActivity);
        };

        _quality = (TextView)FindViewById(R.Id.action_document_quality)!;
        _quality.Text = Texts.CheckDocumentQuality;
        _quality.Click += delegate
        {
            var document = _scanbotSdk.DocumentApi.LoadDocument(documentId)?.GetOrThrow<Document>();
            var bitmap = document.PageAtIndex(0)?.DocumentImage;
                
            if (bitmap == null)
                return;

            var configuration = new DocumentQualityAnalyzerConfiguration();
            
            var qualityAnalyzer = _scanbotSdk.CreateDocumentQualityAnalyzer(configuration).GetOrThrow<IDocumentQualityAnalyzer>();
            var genericResult = qualityAnalyzer?.Run(ImageRef.FromBitmap(bitmap, new BasicImageLoadOptions()));
                var documentQualityResult = genericResult.GetOrThrow<DocumentQualityAnalyzerResult>();
            Alert.Show(this, "Document Quality", documentQualityResult?.Quality?.Name());
        };

        _filter = (TextView)FindViewById(R.Id.action_filter)!;
        _filter.Text = Texts.Filter;
        _filter.Click += delegate
        {
            _filterFragment.Show(SupportFragmentManager, FiltersMenuTag);
        };

        // Top Toolbar
        _export = (Button)FindViewById(R.Id.action_export_document)!;
        _export.Text = Texts.Export;
        _export.Click += delegate
        {
            _saveFragment.Show(SupportFragmentManager, SaveMenuTag);
        };
        
        _cleanUp = (TextView)FindViewById(R.Id.action_clean_up)!;
        _cleanUp.Text = Texts.CleanUp;
        _cleanUp.Click += LaunchDocumentCleanUp;
    }

    private void LaunchDocumentCleanUp(object sender, EventArgs e)
    {
        if (_document.Pages.Count == 0) return; // returns if no page for clean up.
        
        // Create the default configuration object. You may access the IScannedDocument to get the unique identifiers
        var configuration = DocumentCleanupActivityConfiguration.Init(_document.Uuid, _document.Pages.First().Uuid);

        // If true, the cleanup tool will not allow erasing text. But it takes some time to process OCR on the image initially,
        configuration.Cleanup.EngineConfiguration.KeepText = false;

        // The maximum number of undo/redo operations that can be performed. Make it less to save up memory
        configuration.Cleanup.EngineConfiguration.MaxUndoRedoStackSize = 4;

        // Downscales stroke area to this value in pixels (width x height) to speed up the cleanup process. The smaller the value the faster but quality will be lower too.
        configuration.Cleanup.EngineConfiguration.MaxCleanupResolution = 1_200_000;

        // Customize the top bar.
        configuration.Cleanup.TopBarBackButton.Text = "Cancel";
        configuration.Cleanup.TopBarConfirmButton.Text = "Done";
        configuration.Cleanup.TopBarTitle.Text = "Clean up the page";

        // background color
        configuration.Cleanup.BackgroundColor = new ScanbotColor("#222222");

        // Configure the toolbar buttons (undo / redo / reset).
        configuration.Cleanup.Toolbar.UndoButton.Title.Text = "Undo";
        configuration.Cleanup.Toolbar.RedoButton.Title.Text = "Redo";
        configuration.Cleanup.Toolbar.ResetButton.Title.Text = "Reset";

        configuration.Cleanup.Toolbar.StrokeSizeSlider.Visible = true;
        configuration.Cleanup.Toolbar.StrokeSizeSlider.MinStrokeSize = 1;
        configuration.Cleanup.Toolbar.StrokeSizeSlider.MaxStrokeSize = 50;
        configuration.Cleanup.Toolbar.StrokeSizeSlider.Title.Text = "Brush size";

        // Optional: show an introduction screen the first time the user opens cleanup.
        configuration.Cleanup.Introduction.ShowAutomatically = true;

        // Customize the alert dialogs shown for Reset and for cancelling with unsaved changes.
        configuration.Cleanup.ResetAllEditsAlertDialog.Title.Text = "Reset all edits?";
        configuration.Cleanup.ResetAllEditsAlertDialog.Subtitle.Text = "This will revert all cleanup operations on this page.";

        configuration.Cleanup.DiscardChangesAlertDialog.Title.Text = "Discard changes?";
        configuration.Cleanup.DiscardChangesAlertDialog.Subtitle.Text = "Your cleanup edits on this page will be lost.";

        // Launch the scanner
        var intent = DocumentCleanupActivity.NewIntent(this, configuration);
        StartActivityForResult(intent, DocumentCleanUpdRequestCode);
    }

    private void SetupToolbar()
    {
        var toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(R.Id.toolbar);
        SetSupportActionBar(toolbar);
            
        if (SupportActionBar == null)
            return;

        SupportActionBar.Title = Texts.ScanResults;
        SupportActionBar.SetDisplayHomeAsUpEnabled(true);
        SupportActionBar.SetDisplayShowHomeEnabled(true);
    }

    protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
    {
        base.OnActivityResult(requestCode, resultCode, data);
            
        _document = _scanbotSdk.DocumentApi.LoadDocument(_document.Uuid).GetOrThrow<Document>(); // refresh from memory
        _adapter.Refresh(_document);
        UpdateVisibility();
    }

    protected override void OnResume()
    {
        base.OnResume();

        if (!_scanbotSdk.LicenseInfo.IsValid)
        {
            Alert.ShowLicenseDialog(this);
        }

        UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        _quality.Visibility = _adapter.ItemCount == 1 ? ViewStates.Visible : ViewStates.Invisible;
        _crop.Visibility = _adapter.ItemCount == 1 ? ViewStates.Visible : ViewStates.Invisible;
        _filter.Enabled = !_adapter.IsEmpty;
    }

    private Android.Net.Uri GetOutputUri(string extension)
    {
        var external = GetExternalFilesDir(null)?.AbsolutePath ?? string.Empty;
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
        foreach (var page in _document.Pages)
        {
            page.Apply(page.Rotation, page.Polygon, [ selectedFilter ], null);
        }
        _adapter.Refresh(_document);                                          
    }
}

class PageAdapter : RecyclerView.Adapter
{
    private readonly IO.Scanbot.Sdk.Persistence.Fileio.IFileIOProcessor _fileProcessor;
    private List<PageModel> _pages = new List<PageModel>();
    public PageAdapter(IO.Scanbot.Sdk.Persistence.Fileio.IFileIOProcessor fileProcessor, Document document)
    {
        this._fileProcessor = fileProcessor;
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

    public bool IsEmpty => ItemCount == 0;

    public override long GetItemId(int position)
    {
        return _pages[position].GetHashCode();
    }

    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
    {
        var view = LayoutInflater.From(Context)?.Inflate(R.Layout.item_page, parent, false);
        return new PageViewHolder(view);
    }

    public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
    {
        var page = _pages[position];
        (holder as PageViewHolder)?.Image.SetImageResource(0);

        var options = new BitmapFactory.Options();
        if (File.Exists(page.ScannedPagePreviewUri.Path))
        {
            var bitmap = _fileProcessor.ReadImage(page.ScannedPagePreviewUri, options);

            (holder as PageViewHolder)?.Image.SetImageBitmap(bitmap);
        }
        else
        {
            var bitmap = _fileProcessor.ReadImage(page.OriginalPagePreviewUri, options);
            (holder as PageViewHolder)?.Image.SetImageBitmap(bitmap);
        }
    }
}

class PageViewHolder : RecyclerView.ViewHolder
{
    public readonly ImageView Image;
    public PageViewHolder(View item) : base(item)
    {
        Image = item.FindViewById<ImageView>(R.Id.page);
    }
}