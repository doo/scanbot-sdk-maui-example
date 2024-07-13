using ReadyToUseUI.Maui.Utils;
using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Document;
using SQLite;

namespace ReadyToUseUI.Maui.Models
{
    public class PageStorage
    {
        public static PageStorage Instance = new PageStorage();

        public const string DatabaseFilename = "SBSDKPageStorage.db3";

        public const SQLiteOpenFlags Flags = SQLiteOpenFlags.ReadWrite
                                             | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache;

        public static string DatabasePath
        {
            get
            {
                var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                basePath = Path.Combine(basePath, DatabaseFilename);
                return basePath;
            }
        }

        private SQLiteAsyncConnection database;

        private async Task<SQLiteAsyncConnection> GetDatabaseAsync()
        {
            if (database == null)
            {
                database = new SQLiteAsyncConnection(DatabasePath, Flags);
                var result = await database.CreateTablesAsync(CreateFlags.None, typeof(DBPage));
                Console.WriteLine("Storage initialize: " + result);
            }

            return database;
        }

        private PageStorage()
        {
        }


        public async Task<int> CreateAsync(IScannedPage page)
        {
            var dbPage = DBPage.From(page);
            return await (await GetDatabaseAsync()).InsertAsync(dbPage);
        }

        public async Task<int> UpdateAsync(IScannedPage page)
        {
            return await (await GetDatabaseAsync()).UpdateAsync(DBPage.From(page));
        }

        public async Task<List<IScannedPage>> LoadAsync()
        {
            var db = await GetDatabaseAsync();
            var dbPages = await db.Table<DBPage>().ToListAsync();

            var scannedPages = new List<IScannedPage>();
            foreach (var page in dbPages)
            {
                var result = await ScanbotSDK.MAUI.ScanbotSDK.SDKService.ReconstructPage(
                    page.Id,
                    page.CreatePolygon(),
                    SDKUtils.JsonToFilter(page.ParametricFilters) ??
                    new[] { ParametricFilter.FromLegacyFilter(ImageFilter.None) },
                    (DocumentDetectionStatus)page.DetectionStatus);

                scannedPages.Add(result);
            }

            return scannedPages;
        }

        public async Task<int> DeleteAsync(IScannedPage page)
        {
            return await (await GetDatabaseAsync()).DeleteAsync(DBPage.From(page));
        }

        public async Task<int> ClearAsync()
        {
            var mapping = (await GetDatabaseAsync()).TableMappings.First(tables => tables.TableName == "DBPage");
            if (mapping != null)
            {
                return await (await GetDatabaseAsync()).DeleteAllAsync(mapping);
            }

            return -1;
        }
    }

    /*
     * SQLite storage requires a non-abstract class with a constructor and primitive types
     */
    public class DBPage
    {
        [PrimaryKey] public string Id { get; set; }

        public int Filter { get; set; }

        /// <summary>
        /// Dictionary of all the filter names mapped with their corresponding filter objects. 
        /// </summary>
        public string ParametricFilters { get; set; }
        
        public int DetectionStatus { get; set; }

        public double X1 { get; set; }
        public double Y1 { get; set; }
        public double X2 { get; set; }
        public double Y2 { get; set; }
        public double X3 { get; set; }
        public double Y3 { get; set; }
        public double X4 { get; set; }
        public double Y4 { get; set; }

        public static DBPage From(IScannedPage page)
        {
            var result = new DBPage
            {
                Id = page.Id,
                ParametricFilters = SDKUtils.FilterToJson(page.Filters),
                DetectionStatus = (int)page.DetectionStatus
            };

            result.MapPolygon(page.Polygon);

            return result;
        }

        public void MapPolygon(Point[] points)
        {
            if (points.Length < 4)
            {
                return;
            }

            X1 = points[0].X;
            Y1 = points[0].Y;
            X2 = points[1].X;
            Y2 = points[1].Y;
            X3 = points[2].X;
            Y3 = points[2].Y;
            X4 = points[3].X;
            Y4 = points[3].Y;
        }

        public Point[] CreatePolygon()
        {
            return new List<Point>
            {
                new Point(X1, Y1),
                new Point(X2, Y2),
                new Point(X3, Y3),
                new Point(X4, Y4)
            }.ToArray();
        }
    }
}