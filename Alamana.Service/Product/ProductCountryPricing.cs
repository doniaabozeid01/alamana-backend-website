using Alamana.Data.Entities;

namespace Alamana.Service.Product;

public static class ProductCountryPricing
{
    public static CountryProducts? Resolve(Products product, int? countryId)
    {
        if (product.CountryProducts == null || product.CountryProducts.Count == 0)
            return null;

        if (countryId.HasValue)
            return product.CountryProducts.FirstOrDefault(cp => cp.CountryId == countryId.Value);

        return product.CountryProducts.FirstOrDefault();
    }

    public static decimal GetPrice(Products product, int? countryId) =>
        Resolve(product, countryId)?.Price ?? 0m;

    public static bool GetIsNew(Products product, int? countryId) =>
        Resolve(product, countryId)?.IsNew ?? false;

    public static decimal GetDiscount(Products product, int? countryId) =>
        Resolve(product, countryId)?.Discount ?? 0m;
}
