using NetCoreStack.Data.Contracts;
using System;

namespace NetCoreStack.Proxy.Test.Contracts
{
    public class EntityAuditInsert : EntityInsertActive
    {
        public long? CreatedByUserUserGroupDomainId { get; set; }
    }

    public class EntityInsertActive : EntityInsertBase
    {
        public bool IsActive { get; set; }

        public EntityInsertActive()
        {
            IsActive = true;
        }
    }

    public class EntityAuditUpdate : EntityAuditInsert
    {
        public long? UpdatedByUserUserGroupDomainId { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class EntityInsertBase : EntityIdentitySql
    {
        public DateTime CreatedDate { get; set; }
    }
}
