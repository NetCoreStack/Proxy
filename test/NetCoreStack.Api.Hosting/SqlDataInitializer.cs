using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetCoreStack.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreStack.Api.Hosting
{
    public static class SqlDataInitializer
    {
        const string imgUrl = "http://placehold.it/200x100";

        public static void InitializeMusicStoreSqlDatabase(IServiceProvider serviceProvider)
        {
            using (var db = serviceProvider.GetService<MusicStoreContext>())
            {
                if (db.Database.EnsureCreated())
                {
                    InsertTestData(serviceProvider);
                }
            }
        }

        private static void InsertTestData(IServiceProvider serviceProvider)
        {
            var albums = SqlSampleData.GetAlbums(imgUrl, SqlSampleData.Genres, SqlSampleData.Artists);
            AddOrUpdate(serviceProvider, g => g.Id, SqlSampleData.Genres.Select(genre => genre.Value));
            AddOrUpdate(serviceProvider, a => a.Id, SqlSampleData.Artists.Select(artist => artist.Value));
            AddOrUpdate(serviceProvider, a => a.Id, albums);
        }
        
        private static void AddOrUpdate<TEntity>(
            IServiceProvider serviceProvider,
            Func<TEntity, object> propertyToMatch, IEnumerable<TEntity> entities)
            where TEntity : class
        {
            // Query in a separate context so that we can attach existing entities as modified
            List<TEntity> existingData;

            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            using (var db = scope.ServiceProvider.GetService<MusicStoreContext>())
            {
                existingData = db.Set<TEntity>().ToList();
            }

            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            using (var db = scope.ServiceProvider.GetService<MusicStoreContext>())
            {
                foreach (var item in entities)
                {
                    db.Entry(item).State = existingData.Any(g => propertyToMatch(g).Equals(propertyToMatch(item)))
                        ? EntityState.Modified
                        : EntityState.Added;
                }

                db.SaveChanges();
            }
        }
    }
}
