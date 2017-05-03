using Microsoft.Extensions.DependencyInjection;
using NetCoreStack.Data.Helpers;
using NetCoreStack.Data.Interfaces;
using NetCoreStack.Domain.Contracts;
using System;
using System.Linq;

namespace NetCoreStack.Api.Hosting
{
    public static class BsonDataInitializer
    {
        const string imgUrl = "http://placehold.it/200x100";

        public static void InitializeMusicStoreMongoDb(IServiceProvider serviceProvider)
        {
            using (var db = serviceProvider.GetService<IMongoDbDataContext>())
            {
                var albums = BsonSampleData.GetAlbums(imgUrl, BsonSampleData.Genres, BsonSampleData.Artists);
                if (!albums.Any())
                {
                    db.MongoDatabase.DropCollection(BsonCollectionHelper.GetCollectionName<AlbumBson>());
                    db.Collection<AlbumBson>().InsertMany(albums);
                }
            }
        }
    }
}
