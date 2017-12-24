﻿using NetCoreStack.Contracts;
using System.ComponentModel.DataAnnotations;

namespace NetCoreStack.Domain.Contracts
{
    [CollectionName("Artists")]
    public class ArtistBson : EntityIdentityBson
    {
        [Required]
        public string Name { get; set; }
    }
}
