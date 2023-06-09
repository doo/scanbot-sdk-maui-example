﻿using DocumentSDK.MAUI.Services;
using SQLite;

namespace DocumentSDK.MAUI.Example.Models
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

        static SQLiteAsyncConnection Database = new SQLiteAsyncConnection(DatabasePath, Flags);

        private PageStorage() {}

        public async Task InitializeAsync()
        {
            var result = await Database.CreateTablesAsync(CreateFlags.None, typeof(DBPage));
            Console.WriteLine("Storage initialize: " + result);
        }

        public async Task<int> Save(IScannedPageService page)
        {
            var dbPage = DBPage.From(page);
            return await Database.InsertAsync(dbPage);
        }

        public async Task<int> Update(IScannedPageService page)
        {
            return await Database.UpdateAsync(DBPage.From(page));
        }

        public async Task<List<DBPage>> Load()
        {
            var pages = await Database.Table<DBPage>().ToListAsync();
            return pages;
        }

        public async Task<int> Delete(IScannedPageService page)
        {
            return await Database.DeleteAsync(DBPage.From(page));
        }

        public async Task<int> Clear()
        {
            var mapping = Database.TableMappings.First(tables => tables.TableName == "DBPage");
            if (mapping != null)
            {
                return await Database.DeleteAllAsync(mapping);
            }

            return -1;
        }
    }

    /*
     * SQLite storage requires a non-abstract class with a constructor and primitive types
     */
    public class DBPage
    {
        [PrimaryKey]
        public string Id { get; set; }

        public int Filter { get; set; }
        public int DetectionStatus { get; set; }

        public double X1 { get; set; }
        public double Y1 { get; set; }
        public double X2 { get; set; }
        public double Y2 { get; set; }
        public double X3 { get; set; }
        public double Y3 { get; set; }
        public double X4 { get; set; }
        public double Y4 { get; set; }

        public static DBPage From(IScannedPageService page)
        {

            var result = new DBPage
            {
                Id = page.Id,
                Filter = (int)page.Filter,
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
            var result = new List<Point>();
            result.Add(new Point(X1, Y1));
            result.Add(new Point(X2, Y2));
            result.Add(new Point(X3, Y3));
            result.Add(new Point(X4, Y4));
            return result.ToArray();
        }

    }
}

