using Microsoft.AspNetCore.Http;

namespace Alamana.Service.Category.Dtos
{
    public class AddCategoryDto
    {
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionAr { get; set; }
        public IFormFile? Image { get; set; }
        public IFormFile? MobileImage { get; set; }
    }
}
