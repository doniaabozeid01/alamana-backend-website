using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using Alamana.Service.Product.Dtos;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;

namespace Alamana.ModelBinding;

/// <summary>
/// Swagger غالبًا يبعت عناصر <c>Details</c> كحقل واحد لكل index فيه JSON كامل.
/// الـ binder الافتراضي ما يحوّلهاش لـ <see cref="ProductDetailFormItem"/>، فالقائمة تفضل فاضية.
/// </summary>
public sealed class ProductDetailsListModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        return context.Metadata.ModelType == typeof(List<ProductDetailFormItem>)
            ? new ProductDetailsListModelBinder()
            : null;
    }
}

public sealed class ProductDetailsListModelBinder : IModelBinder
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    private sealed class JsonRow
    {
        public string? Key { get; set; }
        public string? Value { get; set; }
        public int? Order { get; set; }
        public int? SortOrder { get; set; }
    }

    private sealed class Accumulator
    {
        public string? RawJson { get; set; }
        public Dictionary<string, string> Flat { get; } = new(StringComparer.OrdinalIgnoreCase);
    }

    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (!bindingContext.HttpContext.Request.HasFormContentType)
        {
            bindingContext.Result = ModelBindingResult.Success(null);
            return Task.CompletedTask;
        }

        var form = bindingContext.HttpContext.Request.Form;
        var fullName = bindingContext.ModelName;
        // ASP.NET بيستخدم اسم زي productDto.Details بينما Swagger كثيرًا يبعت Details[0].Key من غير البادئة
        var shortName = fullName.Contains('.', StringComparison.Ordinal)
            ? fullName[(fullName.LastIndexOf('.') + 1)..]
            : fullName;

        // "Details" صريحًا: أحيانًا ModelName ما يطابقش مفتاح الـ form اللي Swagger بيبعته
        var nameVariants = new[] { fullName, shortName, "Details" }
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var byIndex = new Dictionary<int, Accumulator>();

        foreach (var segment in nameVariants)
        {
            var escaped = Regex.Escape(segment);
            // Details[0] أو productDto.Details[0] أو foo.Details[0]
            var reIndexed = new Regex(
                $@"(?:^|.*\.){escaped}\[(\d+)\](?:\.(.+))?$",
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            // بعض العملاء: Details[0][Key]
            var reBracket = new Regex(
                $@"(?:^|.*\.){escaped}\[(\d+)\]\[([^\]]+)\]$",
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

            foreach (var field in form)
            {
                AccumulateField(byIndex, field, reIndexed);
                AccumulateField(byIndex, field, reBracket);
            }
        }

        if (byIndex.Count == 0)
        {
            // Swagger كثيرًا يبعت حقل واحد اسمه Details: إما [{...}] أو كائن واحد {...}
            if (TryGetRootDetailsFormValue(form, nameVariants, out var rootRaw))
            {
                var trimmed = rootRaw.TrimStart();
                if (trimmed.StartsWith('['))
                {
                    var parsed = TryParseDetailsJsonArray(rootRaw);
                    if (parsed.Count > 0)
                    {
                        bindingContext.Result = ModelBindingResult.Success(parsed);
                        return Task.CompletedTask;
                    }
                }
                else if (trimmed.StartsWith('{'))
                {
                    var one = TryFromJson(rootRaw, 0);
                    if (one != null)
                    {
                        bindingContext.Result = ModelBindingResult.Success(new List<ProductDetailFormItem> { one });
                        return Task.CompletedTask;
                    }
                }
            }

            bindingContext.Result = ModelBindingResult.Success(null);
            return Task.CompletedTask;
        }

        var built = new List<ProductDetailFormItem>();
        foreach (var index in byIndex.Keys.OrderBy(i => i))
        {
            var acc = byIndex[index];
            var item = TryFromFlat(acc.Flat) ?? TryFromJson(acc.RawJson, index);
            if (item != null)
                built.Add(item);
        }

        bindingContext.Result = ModelBindingResult.Success(built);
        return Task.CompletedTask;
    }

    private static bool TryGetRootDetailsFormValue(
        IFormCollection form,
        IReadOnlyList<string> nameVariants,
        out string raw)
    {
        foreach (var segment in nameVariants)
        {
            if (form.TryGetValue(segment, out var values) && !StringValues.IsNullOrEmpty(values))
            {
                raw = values.ToString();
                if (!string.IsNullOrWhiteSpace(raw))
                    return true;
            }
        }

        foreach (var kv in form)
        {
            if (string.Equals(kv.Key, "Details", StringComparison.OrdinalIgnoreCase))
            {
                raw = kv.Value.ToString();
                if (!string.IsNullOrWhiteSpace(raw))
                    return true;
            }

            if (kv.Key.EndsWith(".Details", StringComparison.OrdinalIgnoreCase))
            {
                raw = kv.Value.ToString();
                if (!string.IsNullOrWhiteSpace(raw))
                    return true;
            }
        }

        raw = string.Empty;
        return false;
    }

    private static void AccumulateField(
        Dictionary<int, Accumulator> byIndex,
        KeyValuePair<string, StringValues> field,
        Regex re)
    {
        var m = re.Match(field.Key);
        if (!m.Success)
            return;

        var index = int.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture);
        var subKey = m.Groups.Count > 2 && m.Groups[2].Success ? m.Groups[2].Value : null;

        if (!byIndex.TryGetValue(index, out var acc))
        {
            acc = new Accumulator();
            byIndex[index] = acc;
        }

        if (string.IsNullOrEmpty(subKey))
            acc.RawJson = field.Value.ToString();
        else
            acc.Flat[subKey.Trim()] = field.Value.ToString();
    }

    private static List<ProductDetailFormItem> TryParseDetailsJsonArray(string raw)
    {
        var list = new List<ProductDetailFormItem>();
        try
        {
            var rows = JsonSerializer.Deserialize<List<JsonRow>>(raw.Trim(), JsonOptions);
            if (rows == null)
                return list;

            for (var i = 0; i < rows.Count; i++)
            {
                var row = rows[i];
                if (row == null || string.IsNullOrWhiteSpace(row.Key))
                    continue;

                list.Add(new ProductDetailFormItem
                {
                    Key = row.Key.Trim(),
                    Value = row.Value?.Trim() ?? string.Empty,
                    Order = row.Order ?? row.SortOrder ?? i,
                });
            }
        }
        catch
        {
            // تجاهل JSON غير صالح لهذا المسار
        }

        return list;
    }

    private static ProductDetailFormItem? TryFromFlat(Dictionary<string, string> flat)
    {
        if (flat.Count == 0)
            return null;

        string? key = Get(flat, "Key");
        if (string.IsNullOrWhiteSpace(key))
            return null;

        var value = Get(flat, "Value") ?? string.Empty;
        int? order = null;
        var orderStr = Get(flat, "Order") ?? Get(flat, "SortOrder");
        if (!string.IsNullOrWhiteSpace(orderStr) && int.TryParse(orderStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out var o))
            order = o;

        return new ProductDetailFormItem { Key = key, Value = value, Order = order };
    }

    private static string? Get(Dictionary<string, string> flat, string name)
    {
        foreach (var kv in flat)
        {
            if (string.Equals(kv.Key, name, StringComparison.OrdinalIgnoreCase))
                return kv.Value;
        }

        return null;
    }

    private static ProductDetailFormItem? TryFromJson(string? raw, int fallbackIndex)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return null;

        var trimmed = raw.Trim();
        if (!trimmed.StartsWith('{'))
            return null;

        try
        {
            var row = JsonSerializer.Deserialize<JsonRow>(trimmed, JsonOptions);
            if (row == null || string.IsNullOrWhiteSpace(row.Key))
                return null;

            var order = row.Order ?? row.SortOrder ?? fallbackIndex;
            return new ProductDetailFormItem
            {
                Key = row.Key.Trim(),
                Value = row.Value?.Trim() ?? string.Empty,
                Order = order,
            };
        }
        catch
        {
            return null;
        }
    }
}
