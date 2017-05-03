using NetCoreStack.Contracts;
using System.ComponentModel.DataAnnotations;

namespace NetCoreStack.Domain.Contracts
{
    [BsonCollectionName("Genres")]
    public class GenreBson : EntityIdentityBson
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}
