using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using IO.Scanbot.Sdk.Persistence;
using IO.Scanbot.Sdk.Process;
using IO.Scanbot.Sdk.UI.View.Camera;
using IO.Scanbot.Sdk.UI.View.Camera.Configuration;
using IO.Scanbot.Sdk.Util.Thread;
using ReadyToUseUI.Droid.Fragments;
using ReadyToUseUI.Droid.Listeners;
using ReadyToUseUI.Droid.Utils;
using DocumentSDK.NET.Model;
using IO.Scanbot.Sdk.Tiff.Model;
using ReadyToUseUI.Droid.Model;
using static Java.Interop.JniEnvironment;
using IO.Scanbot.Sdk.Ocr;
using static IO.Scanbot.Sdk.Ocr.IOpticalCharacterRecognizer;

namespace ReadyToUseUI.Droid.Activities
{
    [Activity]
    public class PagePreviewActivity : AppCompatActivity, IFiltersListener
    {
        const int FILTER_UI_REQUEST_CODE = 7777;
        const int CAMERA_ACTIVITY = 8888;

        const string FILTERS_MENU_TAG = "FILTERS_MENU_TAG";
        const string SAVE_MENU_TAG = "SAVE_MENU_TAG";

        private IO.Scanbot.Sdk.ScanbotSDK scanbotSDK;
        private IO.Scanbot.Sdk.Persistence.PageFileStorage pageStorage;
        private IO.Scanbot.Sdk.Docprocessing.PageProcessor pageProcessor;

        PageAdapter adapter;
        RecyclerView recycleView;

        FilterBottomSheetMenuFragment filterFragment;
        SaveBottomSheetMenuFragment saveFragment;

        ProgressBar progress;
        TextView delete, filter, addPage, results;
        Button save;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            scanbotSDK = new IO.Scanbot.Sdk.ScanbotSDK(this);
            pageStorage = scanbotSDK.CreatePageFileStorage();
            pageProcessor = scanbotSDK.CreatePageProcessor();

            SetContentView(Resource.Layout.activity_page_preview);

            SetupToolbar();

            filterFragment = new FilterBottomSheetMenuFragment();
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

            adapter = new PageAdapter(scanbotSDK.FileIOProcessor(), pageStorage);
            adapter.HasStableIds = true;
            adapter.Context = this;

            recycleView = FindViewById<RecyclerView>(Resource.Id.pages_preview);
            recycleView.HasFixedSize = true;
            recycleView.SetAdapter(adapter);
            recycleView.SetLayoutManager(new GridLayoutManager(this, 3));

            addPage = FindViewById<TextView>(Resource.Id.action_add_page);
            addPage.Text = Texts.add_page;
            addPage.Click += delegate
            {
                var configuration = new DocumentScannerConfiguration();
                configuration.SetCameraPreviewMode(IO.Scanbot.Sdk.Camera.CameraPreviewMode.FillIn);
                configuration.SetIgnoreBadAspectRatio(true);
                var intent = DocumentScannerActivity.NewIntent(this, configuration);
                StartActivityForResult(intent, CAMERA_ACTIVITY);
            };

            results = FindViewById<TextView>(Resource.Id.scan_results);
            results.Text = Texts.scan_results;

            delete = FindViewById<TextView>(Resource.Id.action_delete_all);
            delete.Text = Texts.delete_all;
            delete.Click += delegate
            {
                pageStorage.RemoveAll();
                adapter.Refresh();
                delete.Enabled = false;
                filter.Enabled = false;
                save.Enabled = false;
            };

            filter = FindViewById<TextView>(Resource.Id.action_filter);
            filter.Text = Texts.filter;
            filter.Click += delegate
            {
                var existing = SupportFragmentManager.FindFragmentByTag(FILTERS_MENU_TAG);
                filterFragment.Show(SupportFragmentManager, FILTERS_MENU_TAG);
            };

            save = FindViewById<Button>(Resource.Id.action_save_document);
            save.Text = Texts.save;
            save.Click += delegate
            {
                var existing = SupportFragmentManager.FindFragmentByTag(SAVE_MENU_TAG);
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

            adapter.Refresh();
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
            delete.Enabled = !adapter.IsEmpty;
            filter.Enabled = !adapter.IsEmpty;
            save.Enabled = !adapter.IsEmpty;
        }

        public void SaveTiff() => SaveDocument(SaveType.TIFF);

        public void SaveWithOcr() => SaveDocument(SaveType.OCR);
        
        public void SaveWithoutOcr() => SaveDocument(SaveType.Plain);

        void SaveDocument(SaveType type)
        {
            if (!scanbotSDK.LicenseInfo.IsValid)
            {
                Alert.ShowLicenseDialog(this);
                return;
            }

            Task.Run(delegate
            {
                var pagesUri = adapter.GetDocumentUris();
                var output = GetOutputUri(".pdf");

                if (type == SaveType.TIFF)
                {
                    output = GetOutputUri(".tiff");
                    // Please note that some compression types are only compatible for 1-bit encoded images (binarized black & white images)!
                    var options = new IO.Scanbot.Sdk.Tiff.Model.TIFFImageWriterParameters(
                        ImageFilterType.PureBinarized,
                        250,
                        IO.Scanbot.Sdk.Tiff.Model.TIFFImageWriterCompressionOptions.CompressionCcittfax4,
                        Array.Empty<TIFFImageWriterUserDefinedField>());

                    scanbotSDK.CreateTiffWriter().WriteTIFFFromFiles(pagesUri.Select(i => new Java.IO.File(i.Path)).ToArray(), false, new Java.IO.File(output.Path), options);
                }
                else if (type == SaveType.OCR)
                {
                    // This is the new OCR configuration with ML which doesn't require the languages.
                    var recognitionMode = IOpticalCharacterRecognizer.EngineMode.Tesseract;
                    var recognizer = scanbotSDK.CreateOcrRecognizer();

                    // to use legacy configuration we have to pass the installed languages.
                    if (recognitionMode == IOpticalCharacterRecognizer.EngineMode.Tesseract)
                    {
                        var languages = recognizer.InstalledLanguages;
                        if (languages.Count == 0)
                        {
                            RunOnUiThread(delegate
                            {
                                Alert.Toast(this, "OCR languages blobs are not available");
                            });
                            return;
                        }

                        var ocrConfig = new OcrConfig(IOpticalCharacterRecognizer.EngineMode.Tesseract, recognizer.InstalledLanguages);
                        recognizer.SetOcrConfig(ocrConfig);
                    }
                    else
                    {
                        recognizer.SetOcrConfig(new OcrConfig(IOpticalCharacterRecognizer.EngineMode.ScanbotOcr));
                    }

                    var pdfFile = recognizer.RecognizeTextWithPdfFromUris(pagesUri, MainApplication.USE_ENCRYPTION, IO.Scanbot.Pdf.Model.PdfConfig.DefaultConfig());
                    File.Move(pdfFile.SandwichedPdfDocumentFile.AbsolutePath, new Java.IO.File(output.Path).AbsolutePath);
                }
                else
                {
                    var pdfFile = scanbotSDK.CreatePdfRenderer().RenderDocumentFromImages(pagesUri, false, IO.Scanbot.Pdf.Model.PdfConfig.DefaultConfig());
                    File.Move(pdfFile.AbsolutePath, new Java.IO.File(output.Path).AbsolutePath);
                }

                Java.IO.File file = Copier.Copy(this, output);

                var intent = new Intent(Intent.ActionView, output);
                 
                var authority = ApplicationContext.PackageName + ".provider";
                var uri = FileProvider.GetUriForFile(this, authority, file);

                intent.SetDataAndType(uri, MimeUtils.GetMimeByName(file.Name));
                intent.SetFlags(ActivityFlags.ClearWhenTaskReset | ActivityFlags.NewTask);
                intent.AddFlags(ActivityFlags.GrantReadUriPermission | ActivityFlags.GrantWriteUriPermission);

                RunOnUiThread(delegate
                {
                    StartActivity(Intent.CreateChooser(intent, output.LastPathSegment));
                    Alert.Toast(this, "File saved to: " + output.Path);
                });
            });
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

        public void ApplyFilter(ImageFilterType type)
        {
            progress.Visibility = ViewStates.Visible;
            Task.Run(delegate
            {
                foreach (var pageId in pageStorage.StoredPages)
                {
                    pageProcessor.ApplyFilter(new Page().Copy(pageId: pageId), type);
                }

                RunOnUiThread(delegate
                {
                    adapter.NotifyDataSetChanged();
                    progress.Visibility = ViewStates.Gone;
                });
            });
        }

        public void OnRecycleViewItemClick(View v)
        {
            var position = recycleView.GetChildLayoutPosition(v);
            var intent = PageFilterActivity.CreateIntent(this, adapter.PageIdForIndex(position));
            StartActivityForResult(intent, FILTER_UI_REQUEST_CODE);
        }
    }

    class PageAdapter :  RecyclerView.Adapter
    {
        private IO.Scanbot.Sdk.Persistence.Fileio.IFileIOProcessor fileProcessor;
        private IO.Scanbot.Sdk.Persistence.PageFileStorage pageStorage;
        private IList<string> pageIds;

        RecyclerViewItemClick listener;

        public PageAdapter(IO.Scanbot.Sdk.Persistence.Fileio.IFileIOProcessor fileProcessor, IO.Scanbot.Sdk.Persistence.PageFileStorage pageStorage)
        {
            this.fileProcessor = fileProcessor;
            this.pageStorage = pageStorage;
            Refresh();
        }

        public void Refresh()
        {
            pageIds = pageStorage.StoredPages ?? new List<string>();
            NotifyDataSetChanged();
        }

        public string PageIdForIndex(int index)
        {
            return pageIds[index];
        }

        private Context context;
        public Context Context
        {
            get => context;
            set
            {
                context = value;
                listener = new RecyclerViewItemClick(Context as PagePreviewActivity);
            }
        }
        public override int ItemCount => pageIds.Count;

        public bool IsEmpty { get => ItemCount == 0; }

        public List<Android.Net.Uri> GetDocumentUris()
        {
            var uris = new List<Android.Net.Uri>();

            foreach (string pageId in pageIds)
            {
                var documentUri = pageStorage.GetImageURI(pageId, PageFileStorage.PageFileType.Document);
                var originalUri = pageStorage.GetImageURI(pageId, PageFileStorage.PageFileType.Original);
                if (File.Exists(documentUri.Path))
                {
                    uris.Add(documentUri);
                }
                else
                {
                    uris.Add(originalUri);
                }
            }

            return uris;
        }

        public override long GetItemId(int position)
        {
            return pageIds[position].GetHashCode();
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = LayoutInflater.From(Context).Inflate(Resource.Layout.item_page, parent, false);
            view.SetOnClickListener(listener);
            return new PageViewHolder(view);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var pageId = pageIds[position];
            var path = pageStorage.GetPreviewImageURI(pageId, PageFileStorage.PageFileType.Document);
            var original = pageStorage.GetPreviewImageURI(pageId, PageFileStorage.PageFileType.Original);

            (holder as PageViewHolder).image.SetImageResource(0);

            var options = new BitmapFactory.Options();
            if (File.Exists(path.Path))
            {
                var bitmap = fileProcessor.ReadImage(path, options);

                (holder as PageViewHolder).image.SetImageBitmap(bitmap);
            }
            else
            {
                var bitmap = fileProcessor.ReadImage(original, options);
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

    class RecyclerViewItemClick : Java.Lang.Object, View.IOnClickListener
    {
        public PagePreviewActivity Context { get; private set; }

        public RecyclerViewItemClick(PagePreviewActivity context)
        {
            Context = context;
        }

        public void OnClick(View v)
        {
            Context.OnRecycleViewItemClick(v);
        }
    }
}

