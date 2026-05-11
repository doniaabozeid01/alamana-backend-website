namespace Alamana.Data.Entities
{
    public class AdvertisementProduct : BaseEntity
    {
        public int Id { get; set; }
        public int AdvertisementId { get; set; }
        public Advertisements Advertisement { get; set; }
        public int ProductId { get; set; }
        public Products Product { get; set; }
    }
}
