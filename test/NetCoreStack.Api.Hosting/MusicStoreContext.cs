using Microsoft.EntityFrameworkCore;
using NetCoreStack.Data.Context;
using NetCoreStack.Data.Interfaces;
using NetCoreStack.Domain.Contracts;

namespace NetCoreStack.Api.Hosting
{
    public class MusicStoreContext : EfCoreContext
    {
        public MusicStoreContext(IDataContextConfigurationAccessor configurator, 
            IDatabasePreCommitFilter filter = null) : base(configurator, filter)
        {
        }

        public DbSet<Album> Albums { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (Configurator != null)
            {
                optionsBuilder.UseSqlite(Configurator.SqlConnectionString);
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Configure pluralization
            builder.Entity<Album>().ToTable("Albums");
            builder.Entity<Artist>().ToTable("Artists");
            builder.Entity<Order>().ToTable("Orders");
            builder.Entity<Genre>().ToTable("Genres");
            builder.Entity<CartItem>().ToTable("CartItems");
            builder.Entity<OrderDetail>().ToTable("OrderDetails");

            base.OnModelCreating(builder);
        }
    }
}
