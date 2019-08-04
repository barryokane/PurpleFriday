using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using Dapper;
using Web.Admin.Data.Models;
using Web.Admin.Models;

namespace Web.Admin.Data
{
    public class SqLiteMapPointRepository : SqLiteBaseRepository, IMapPointRepository
    {

        public SqLiteMapPointRepository(string dataFolderPath) : base(dataFolderPath)
        {

        }

        public List<MapPoint> GetAll(bool includeHidden)
        {
            List<MapPoint> list = null;

            if (!File.Exists(DbFile)) return list;

            using (var cnn = SimpleDbConnection())
            {
                cnn.Open();

                if (includeHidden)
                {
                    list = cnn.Query<SqLiteMapPoint>(
                        @"SELECT Id, TweetId, Text, TweetUrl, Img, TwitterHandle, LocationConfidence,
                        CreatedDate, Geo_x, Geo_y, Area, Hide
                    FROM MapPoints").Select(x => x.ToMapPoint()).ToList();
                }
                else
                {

                    list = cnn.Query<SqLiteMapPoint>(
                        @"SELECT Id, TweetId, Text, TweetUrl, Img, TwitterHandle, LocationConfidence,
                        CreatedDate, Geo_x, Geo_y, Area, Hide
                    FROM MapPoints
                    WHERE Hide = 0").Select(x => x.ToMapPoint()).ToList();
                }
            }
            return list;
        }

        public void AddNew(MapPoint mapPoint)
        {

            using (var cnn = SimpleDbConnection())
            {
                cnn.Open();
                mapPoint.Id = cnn.Query<int>(
                    @"INSERT INTO MapPoints 
                    ( TweetId, Text, TweetUrl, Img, TwitterHandle, LocationConfidence,
                        CreatedDate, Geo_x, Geo_y, Area, Hide ) VALUES 
                    ( @TweetId, @Text, @TweetUrl, @Img, @TwitterHandle, @LocationConfidence,
                        @CreatedDate, @Geo_x, @Geo_y, @Area, @Hide );
                    select last_insert_rowid()", new {
                        mapPoint.TweetId,
                        mapPoint.Text,
                        mapPoint.TweetUrl,
                        mapPoint.Img,
                        mapPoint.TwitterHandle,
                        mapPoint.LocationConfidence,
                        mapPoint.CreatedDate,
                        Geo_x = (mapPoint.Geo != null) ? mapPoint.Geo[0] : 0,
                        Geo_y = (mapPoint.Geo != null) ? mapPoint.Geo[1] : 0,
                        mapPoint.Area,
                        mapPoint.Hide
                    }).First();
            }
        }

        public MapPoint Get(int id)
        {

            using (var cnn = SimpleDbConnection())
            {
                cnn.Open();
                MapPoint result = cnn.Query<SqLiteMapPoint>(
                    @"SELECT Id, TweetId, Text, TweetUrl, Img, TwitterHandle, LocationConfidence,
                        CreatedDate, Geo_x, Geo_y, Area, Hide
                    FROM MapPoints
                    WHERE Id = @id", new { id }).FirstOrDefault().ToMapPoint(); //todo: null check
                return result;
            }
        }

        public int Update(MapPoint mapPoint)
        {

            using (var cnn = SimpleDbConnection())
            {
                cnn.Open();
                var affectedRows = cnn.Execute(
                    @"UPDATE MapPoints SET
                     TweetId=@TweetId, Text=@Text, TweetUrl=@TweetUrl, Img=@Img,
                        TwitterHandle=@TwitterHandle, LocationConfidence=@LocationConfidence,
                        CreatedDate=@CreatedDate, Geo_x=@Geo_x, Geo_y=@Geo_y, Area=@Area, Hide=@Hide 
                    WHERE ID = @Id",
                    SqLiteMapPoint.FromMapPoint(mapPoint));

                return affectedRows;
            }
        }
    }
}
