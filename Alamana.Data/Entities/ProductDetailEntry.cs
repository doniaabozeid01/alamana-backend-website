namespace Alamana.Data.Entities
{
    /// <summary>قسم جينيريك (مفتاح/قيمة) مرتبط بمنتج.</summary>
    public class ProductDetailEntry : BaseEntity
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Products Product { get; set; }
        public string EntryKey { get; set; }
        public string EntryValue { get; set; }
        public int SortOrder { get; set; }
    }
}
