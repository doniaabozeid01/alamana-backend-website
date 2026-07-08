namespace Alamana.Service.Product.Dtos
{
    /// <summary>
    /// صف تفصيل للمنتج. مع <c>multipart/form-data</c>:
    /// <c>Details[i].KeyEn</c>, <c>Details[i].KeyAr</c>, <c>Details[i].ValueEn</c>, <c>Details[i].ValueAr</c>, <c>Details[i].SortOrder</c>.
    /// يُقبل احتياطيًا <c>Key</c> / <c>Value</c> كإنجليزي.
    /// </summary>
    public class ProductDetailFormItem
    {
        public string? KeyEn { get; set; }
        public string? KeyAr { get; set; }
        public string? ValueEn { get; set; }
        public string? ValueAr { get; set; }

        /// <summary>احتياطي — يُعامل كـ KeyEn.</summary>
        public string? Key { get; set; }

        /// <summary>احتياطي — يُعامل كـ ValueEn.</summary>
        public string? Value { get; set; }

        public int? SortOrder { get; set; }
    }
}
