using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Service.Product.Dtos;

namespace Alamana.Service.ProductFavourite.Dtos
{
    public class FavouriteDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string name { get; set; }
        public string name_ar { get; set; }
        public int discount { get; set; }
        public int oldPrice { get; set; }
        public int newPrice { get; set; }
        public string slug { get; set; }
        public string imagePath { get; set; }
        public ProductDto Product { get; set; }
        public string UserId { get; set; }
        public UserDto User { get; set; }
        //public int stock { get; set; }
        public DateTime AddedOn { get; set; }
    }

}
