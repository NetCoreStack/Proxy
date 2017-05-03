using NetCoreStack.Contracts;
using System.ComponentModel.DataAnnotations;

namespace NetCoreStack.Domain.Contracts
{
    public class Artist : EntityBaseSql
    {
        [Required]
        public string Name { get; set; }
    }
}
