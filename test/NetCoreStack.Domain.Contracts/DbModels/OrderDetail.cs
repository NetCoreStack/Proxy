namespace NetCoreStack.Domain.Contracts
{
    public class OrderDetail : EntityIdentitySql
    {
        public long OrderId { get; set; }
        public long AlbumId { get; set; }
        public long Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public virtual Album Album { get; set; }
        public virtual Order Order { get; set; }
    }
}
