namespace Alamana.Service.Product.Dtos
{
    /// <summary>
    /// صف تفصيل للمنتج. مع <c>multipart/form-data</c> (مثل Angular + صور) الأفضل مفاتيح منفصلة:
    /// <c>Details[i].Key</c>, <c>Details[i].Value</c>, <c>Details[i].SortOrder</c>.
    /// يُقبل في JSON (Swagger/احتياطي) أيضًا الحقول <c>order</c> و <c>sortOrder</c> عبر الـ model binder.
    /// </summary>
    public class ProductDetailFormItem
    {
        public string? Key { get; set; }
        public string? Value { get; set; }

        /// <summary>ترتيب العرض؛ لو فاضي يُستخدم ترتيب الصف في القائمة.</summary>
        public int? SortOrder { get; set; }
    }
}
