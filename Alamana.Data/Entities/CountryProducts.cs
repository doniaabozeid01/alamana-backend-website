namespace Alamana.Data.Entities
{
    public class CountryProducts : BaseEntity
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Products Product { get; set; }
        public int CountryId { get; set; }
        public country Country { get; set; }
        public int Stock { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public bool IsNew { get; set; }
        public bool IsHeroProduct { get; set; }
    }
}
