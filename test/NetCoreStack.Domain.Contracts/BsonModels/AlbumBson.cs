using NetCoreStack.Contracts;
using System.ComponentModel.DataAnnotations;

namespace NetCoreStack.Domain.Contracts
{
    [BsonCollectionName("Albums")]
    public class AlbumBson : EntityIdentityBson
    {
        [Required]
        [StringLength(160, MinimumLength = 2)]
        public string Title { get; set; }

        [Required]
        [RangeAttribute(typeof(decimal), "0.01", "100")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Display(Name = "Album Art URL")]
        [StringLength(1024)]
        public string AlbumArtUrl { get; set; }

        public virtual GenreBson Genre { get; set; }

        public virtual ArtistBson Artist { get; set; }
    }
}
