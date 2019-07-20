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
            if (!File.Exists(DbFile))
            {
                CreateDatabase();
            }

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
                        Geo_x = mapPoint.Geo[0],
                        Geo_y = mapPoint.Geo[1],
                        mapPoint.Area,
                        mapPoint.Hide
                    }).First();
            }
        }

        public MapPoint Get(int id)
        {
            if (!File.Exists(DbFile)) return null;

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

        private static void CreateDatabase()
        {
            using (var cnn = SimpleDbConnection())
            {
                cnn.Open();
                cnn.Execute(
                    @"create table MapPoints
                      (
                         ID                           integer primary key AUTOINCREMENT,
                         TweetId                      varchar(100) not null,
                         Text                         nvarchar(300) not null,
                         TweetUrl                     varchar(500) not null,
                         Img                          varchar(500) not null,
                         TwitterHandle                varchar(100) not null,
                         LocationConfidence           varchar(100) not null,
                         CreatedDate                  datetime not null,
                         Geo_x                        real not null,
                         Geo_y                        real not null,
                         Area                         varchar(100) not null,
                         Hide                         integer not null
                      )");
            }
        }
    }
}
