using AndroidNetUri = Android.Net.Uri;
using AndroidFile = Java.IO.File;
using Android.Graphics;
using IO.Scanbot.Sdk.UI.Result;
using static AndroidX.Core.Location.LocationRequestCompat;
using IO.Scanbot.Sdk.Persistence.Fileio;

namespace ClassicComponent.Droid
{
    public class TempImageStorage
    {
        public readonly string TempDir;

        private static TempImageStorage instance;
        private IFileIOProcessor ioProcessor;


        public TempImageStorage(IFileIOProcessor ioProcessor)
        {
            this.TempDir = System.IO.Path.Combine(GetDefaultBaseTempDir(), NewUUID());
            Directory.CreateDirectory(this.TempDir);
            this.ioProcessor = ioProcessor;
        }

        private TempImageStorage()
        {
            
        }

        public static void Init(IFileIOProcessor ioProcessor)
        {
            if (instance == null)
            {
                instance = new TempImageStorage(ioProcessor);
            }
        }

        public static TempImageStorage Instance
        {
            get
            {
                return instance;
            }
        }

        private readonly List<AndroidNetUri> images = new List<AndroidNetUri>();

        public TempImageStorage(string tempDir)
        {
            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }
            if (!Directory.Exists(tempDir))
            {
                throw new IOException("Could not create temp directory: " + tempDir);
            }
            this.TempDir = tempDir;
        }

        private static string GetDefaultBaseTempDir()
        {
            // Environment.SpecialFolder.MyDocuments is internal, private storage (NO access via adb debugging tools!).
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var sbsdkTmp = System.IO.Path.Combine(documents, "sbsdk-temp");
            Directory.CreateDirectory(sbsdkTmp);
            return sbsdkTmp;
        }

        private static string NewUUID()
        {
            return Java.Util.UUID.RandomUUID().ToString();
        }

        public AndroidNetUri AddImage(Bitmap image, int quality = 80)
        {
            byte[] bytes;
            using (MemoryStream ms = new MemoryStream())
            {
                image.Compress(Bitmap.CompressFormat.Jpeg, quality, ms);
                bytes = ms.ToArray();
            }

            var storageFilePath = GetNewFileUri(".jpg").Path;
            AndroidFile file = new AndroidFile(storageFilePath);

            var uri = AndroidNetUri.FromFile(file);
            this.images.Add(uri);
            ioProcessor.Write(bytes, file);
            return uri;
        }

        private AndroidNetUri GetNewFileUri(string extension)
        {
            if (!Directory.Exists(this.TempDir))
            {
                Directory.CreateDirectory(this.TempDir);
            }
            string fileName = NewUUID();
            if (!String.IsNullOrEmpty(extension))
            {
                if (extension[0] != '.')
                    fileName += '.';
                fileName += extension;
            }
            return AndroidNetUri.FromFile(new AndroidFile(System.IO.Path.Combine(this.TempDir, fileName)));
        }

        private AndroidNetUri[] GetImages()
        {
            return this.images.ToArray();
        }

        private int Count()
        {
            return this.images.Count;
        }

        /// <summary>
        /// Recursively deletes current temp storage directory! 
        /// So the content as well as the directory itself will be deleted!
        /// </summary>
        private void CleanUp()
        {
            Console.WriteLine("Recursively deleting temp storage directory: " + this.TempDir);
            if (Directory.Exists(this.TempDir))
            {
                Directory.Delete(this.TempDir, true);
                Directory.CreateDirectory(this.TempDir);
            }
        }
    }
}

