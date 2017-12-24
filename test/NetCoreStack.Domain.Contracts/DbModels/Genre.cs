using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NetCoreStack.Domain.Contracts
{
    public class Genre : EntityIdentitySql
    {
        public Genre()
        {
            Albums = new List<Album>();
        }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public virtual ICollection<Album> Albums { get; set; }
    }
}
