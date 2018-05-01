using System;
using System.Collections.Generic;

namespace NetCoreStack.Proxy.Test.Contracts
{
    public class Attachment : EntityAuditInsert
    {
        public string Name { get; set; }
        public string Content { get; set; }
    }

    public class SubtitleAttachmentDto
    {

        public List<SubtitleAttachment> SubtitleAttachmentList { get; set; }
    }

    public class SubtitleAttachment : EntityAuditUpdate
    {
        public long CategoryId { get; set; }

        public Category Category { get; set; }

        public long SeasonId { get; set; }

        public Attachment Attachment { get; set; }
    }

    public partial class Category : EntityAuditUpdate
    {

        public string CatName { get; set; }

        public long? DomainID { get; set; }

        public long? ParentCategoryID { get; set; }
        public virtual Category ParentCategory { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public DateTime? FinalDate { get; set; }
    }
}
