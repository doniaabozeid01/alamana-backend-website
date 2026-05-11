namespace Alamana.Service.Product.Dtos
{
    /// <summary>
    /// صف تفصيل للمنتج — في Swagger / multipart استخدمي:
    /// Details[0].Key, Details[0].Value, Details[0].Order ثم "Add item" لصف تاني.
    /// </summary>
    public class ProductDetailFormItem
    {
        public string? Key { get; set; }
        public string? Value { get; set; }
        /// <summary>ترتيب العرض؛ لو فاضي يستخدم ترتيب الصف في القائمة.</summary>
        public int? Order { get; set; }
    }
}
