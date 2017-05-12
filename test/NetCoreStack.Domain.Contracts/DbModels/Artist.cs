using NetCoreStack.Data.Contracts;
using System.ComponentModel.DataAnnotations;

namespace NetCoreStack.Domain.Contracts
{
    public class Artist : EntityIdentitySql
    {
        [Required]
        public string Name { get; set; }
    }
}
