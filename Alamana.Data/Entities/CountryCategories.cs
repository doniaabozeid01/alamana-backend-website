namespace Alamana.Data.Entities
{
    public class CountryCategories : BaseEntity
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public Categories Category { get; set; }
        public int CountryId { get; set; }
        public country Country { get; set; }
    }
}
