using NetCoreStack.Contracts;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NetCoreStack.Domain.Contracts
{
    public class Album : EntityBaseSql
    {
        public Album()
        {
            // TODO: Temporary hack to populate the orderdetails until EF does this automatically.
            OrderDetails = new List<OrderDetail>();
        }

        public long GenreId { get; set; }

        public long ArtistId { get; set; }

        [Required]
        [StringLength(160, MinimumLength = 2)]
        public string Title { get; set; }

        [Required]
        [RangeAttribute(typeof(decimal), "0.01", "100")] // Long-form constructor to work around https://github.com/dotnet/coreclr/issues/2172
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Display(Name = "Album Art URL")]
        [StringLength(1024)]
        public string AlbumArtUrl { get; set; }

        public virtual Genre Genre { get; set; }

        public virtual Artist Artist { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
