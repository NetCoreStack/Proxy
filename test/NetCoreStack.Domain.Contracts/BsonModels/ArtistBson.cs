using NetCoreStack.Contracts;
using System.ComponentModel.DataAnnotations;

namespace NetCoreStack.Domain.Contracts
{
    [BsonCollectionName("Artists")]
    public class ArtistBson : EntityIdentityBson
    {
        [Required]
        public string Name { get; set; }
    }
}
