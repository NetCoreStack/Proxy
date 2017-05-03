using NetCoreStack.Contracts;
using System.Collections.Generic;

namespace NetCoreStack.Domain.Contracts
{
    public class InitRequestContext
    {
        public List<IdTextPair> Artists { get; set; }
        public List<IdTextPair> Genres { get; set; }
    }
}