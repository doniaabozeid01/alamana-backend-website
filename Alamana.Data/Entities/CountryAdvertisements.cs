namespace Alamana.Data.Entities
{
    public class CountryAdvertisements : BaseEntity
    {
        public int Id { get; set; }
        public int AdvertisementId { get; set; }
        public Advertisements Advertisement { get; set; }
        public int CountryId { get; set; }
        public country Country { get; set; }
    }
}
