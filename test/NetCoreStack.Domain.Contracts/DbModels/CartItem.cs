using System;
using System.ComponentModel.DataAnnotations;

namespace NetCoreStack.Domain.Contracts
{
    public class CartItem : EntityIdentitySql
    {
        [Required]
        public string CartId { get; set; }
        public long AlbumId { get; set; }
        public long Count { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DateCreated { get; set; }

        public virtual Album Album { get; set; }
    }
}
