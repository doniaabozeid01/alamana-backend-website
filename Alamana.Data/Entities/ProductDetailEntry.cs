namespace Alamana.Data.Entities
{
    /// <summary>قسم جينيريك (مفتاح/قيمة) مرتبط بمنتج.</summary>
    public class ProductDetailEntry : BaseEntity
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Products Product { get; set; }
        public string EntryKeyEn { get; set; }
        public string EntryKeyAr { get; set; }
        public string EntryValueEn { get; set; }
        public string EntryValueAr { get; set; }
        public int SortOrder { get; set; }
    }
}
