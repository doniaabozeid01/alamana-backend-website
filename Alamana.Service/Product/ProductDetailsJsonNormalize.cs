using System.Text;

namespace Alamana.Service.Product;

/// <summary>
/// تطبيع نص JSON للتفاصيل قبل <c>JsonSerializer</c> — يصلح أخطاء شائعة مع النص العربي في multipart / Swagger.
/// </summary>
public static class ProductDetailsJsonNormalize
{
    /// <summary>تحضير النص قبل التحليل (فواصل عربية، علامات اتجاه، اقتباس منسّق، BOM).</summary>
    public static string ForJsonParse(string? raw)
    {
        if (string.IsNullOrEmpty(raw))
            return string.Empty;

        var s = raw.Trim().TrimStart('\uFEFF');

        // فاصلة عربية ، غالبًا تُستخدم بالخطأ بدل فاصلة JSON
        s = s.Replace('\u060C', ',');

        // إزالة علامات الاتجاه والتنسيق التي تكسر النسخ من Word / المحادثات
        foreach (var c in "\u200E\u200F\u202A\u202B\u202C\u202D\u202E\u2066\u2067\u2068\u2069")
            s = s.Replace(c.ToString(), string.Empty);

        // اقتباسات منسّقة → ASCII
        s = s.Replace('\u201C', '"').Replace('\u201D', '"');
        s = s.Replace('\u2018', '\'').Replace('\u2019', '\'');

        return s;
    }

    /// <summary>
    /// لو UTF-8 اتقرأ كـ Latin-1 في طبقة ما، النص يظهر كـ mojibake؛ نعيد بناء UTF-8 من البايتات.
    /// </summary>
    public static string? TryRecoverUtf8MisreadAsLatin1(string s)
    {
        if (string.IsNullOrEmpty(s) || !LooksLikeUtf8MisreadAsLatin1(s))
            return null;

        try
        {
            var latin1 = Encoding.GetEncoding("iso-8859-1");
            var bytes = latin1.GetBytes(s);
            return Encoding.UTF8.GetString(bytes);
        }
        catch
        {
            return null;
        }
    }

    private static bool LooksLikeUtf8MisreadAsLatin1(string s)
    {
        // أنماط شائعة لـ UTF-8 العربي اتقرأ كـ Windows-1252/Latin-1
        return s.Contains('Ã') || s.Contains('Ø') || s.Contains('Ù') || s.Contains('â') || s.Contains('Å');
    }
}
