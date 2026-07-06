using System;
using System.Collections.Generic;
using Alamana.Data.Entities;

namespace Alamana.Service.Category.Dtos
{
    public class categoryDto
    {
        public int Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionAr { get; set; }
        public string? ImagePath { get; set; }
        public string? MobileImagePath { get; set; }
        public List<Products> Products { get; set; }
    }
}
