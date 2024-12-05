using ReadyToUseUI.Maui.Utils;
using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Document;
using SQLite;
using System.Linq;
using ScanbotSDK.MAUI.RTU.v1;

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
                    SDKUtils.JsonToFilter(page.ParametricFilters) ?? new [] { ParametricFilter.FromLegacyFilter(ImageFilter.None) },
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

        public async void ValidateDatabaseMigration()
        {
            try
            {
                // Migration for parametric filters
                var parametricFilterExists = await IsFieldExist(nameof(DBPage), "ParametricFilters");
                if (parametricFilterExists)
                {
                    // Already migrated
                    return;
                }

                await MigrateTableToParametricFilter();
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
            }
        }

        private async Task MigrateTableToParametricFilter()
        {
            try
            {
                // It creates/updates a table based on the new(current) schema.
                // So our Database table is updated with the new ParametricFilter field.
                var sqliteDb = await GetDatabaseAsync();
                var existingPages = await sqliteDb.Table<DBPage>().ToListAsync();

                foreach (var page in existingPages)
                {
                    ParametricFilter newFilter = (ImageFilter)page.Filter;
                    page.ParametricFilters = SDKUtils.FilterToJson(new[] { newFilter });
                }

                await sqliteDb.UpdateAllAsync(existingPages);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception("Error while migrating to the Parametric Filters. \n " + e.Message);
            }
        }

        // Returns [True]
        // - if the column exists in your table.
        // - if the table doesn't exist at all.
        // Return [False]
        // - If the field exists in the table.
        private async Task<bool> IsFieldExist(string tableName, string fieldName)
        {
            var sqliteDatabase = new SQLiteAsyncConnection(DatabasePath, Flags);
            var columns = await sqliteDatabase.GetTableInfoAsync(tableName);

            if (columns == null || columns.Count == 0)
            {
                // no columns available, hence table doesn't exists
                return true;
            }

            return columns.Any(column => column.Name == fieldName);
        }
    }

    /*
     * SQLite storage requires a non-abstract class with a constructor and primitive types
     */
    public class DBPage
    {
        [PrimaryKey] public string Id { get; set; }
        
        [Obsolete("The ImageFilter is obsolete. The ParametricFilters will be used instead.")]
        public int Filter { get; set; }

        /// <summary>
        /// Json Dictionary of the parametric filters.
        /// Key: Filter Name (e.g: GrayScale)
        /// Value: Json string of the Filter object.  
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