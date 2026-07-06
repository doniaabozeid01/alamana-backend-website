using System.Linq;
using Alamana.Data.Entities;
using Alamana.Service.Category.Dtos;
using AutoMapper;

namespace Alamana.Service.Product.Dtos
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Products, ProductDto>()
                .ForMember(d => d.GalleryUrls, opt => opt.MapFrom(s =>
                    s.Media.Select(m => new mediaDto { Url = m.Url, Type = m.Type }).ToList()))
                .ForMember(d => d.Details, opt => opt.MapFrom(s =>
                    s.DetailEntries
                        .OrderBy(e => e.SortOrder)
                        .ThenBy(e => e.Id)
                        .Select(e => new ProductDetailEntryDto
                        {
                            Id = e.Id,
                            Key = e.EntryKey,
                            Value = e.EntryValue,
                            SortOrder = e.SortOrder
                        })
                        .ToList()))
                .ForMember(d => d.category, opt => opt.MapFrom(s => s.Category == null
                    ? null
                    : new productCategoryDto
                    {
                        Id = s.Category.Id,
                        NameEn = s.Category.NameEn,
                        NameAr = s.Category.NameAr,
                        DescriptionEn = s.Category.DescriptionEn,
                        DescriptionAr = s.Category.DescriptionAr
                    }))
                .ForMember(d => d.Price, opt => opt.Ignore())
                .ForMember(d => d.New, opt => opt.Ignore())
                .ForMember(d => d.Discount, opt => opt.Ignore())
                .ForMember(d => d.priceAfterDiscount, opt => opt.Ignore());
        }
    }
}
