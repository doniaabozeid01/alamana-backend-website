using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using Alamana.Service.Product;
using Alamana.Service.Product.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;

namespace Alamana.ModelBinding;

/// <summary>
/// يدعم الطريقة الموصى بها لـ <c>multipart/form-data</c>: <c>Details[i].Key</c> / <c>Value</c> / <c>SortOrder</c>.
/// ويدعم احتياطيًا حقول Swagger (كائن أو مصفوفة JSON تحت اسم <c>Details</c>).
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
            // Swagger / عملاء آخرون: Details.0.key أو productDto.Details.0.key
            var reDot = new Regex(
                $@"(?:^|.*\.){escaped}\.(\d+)\.(.+)$",
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

            foreach (var field in form)
            {
                AccumulateField(byIndex, field, reIndexed);
                AccumulateField(byIndex, field, reBracket);
                AccumulateField(byIndex, field, reDot);
            }
        }

        if (byIndex.Count == 0)
        {
            // حقل جذر واحد: Details = [{...}] أو {...}
            if (TryGetRootDetailsFormValue(form, nameVariants, out var rootRaw) &&
                TryBindDetailsFromJsonString(rootRaw, out var fromRoot))
            {
                bindingContext.Result = ModelBindingResult.Success(fromRoot);
                return Task.CompletedTask;
            }

            // احتياطي: أي مفتاح فيه "Details" وقيمته JSON (Swagger أحيانًا يغيّر اسم الجزء)
            if (TryScavengeDetailsJsonFromForm(form, out var scavenged))
            {
                bindingContext.Result = ModelBindingResult.Success(scavenged);
                return Task.CompletedTask;
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

    private static bool TryBindDetailsFromJsonString(string rootRaw, out List<ProductDetailFormItem> result)
    {
        result = new List<ProductDetailFormItem>();
        var trimmed = rootRaw.Trim().TrimStart('\uFEFF');
        if (trimmed.StartsWith('['))
        {
            result = TryParseDetailsJsonArray(rootRaw);
            return result.Count > 0;
        }

        if (!trimmed.StartsWith('{'))
            return false;

        var one = TryFromJson(rootRaw, 0);
        if (one == null)
            return false;

        result = new List<ProductDetailFormItem> { one };
        return true;
    }

    private static bool TryScavengeDetailsJsonFromForm(IFormCollection form, out List<ProductDetailFormItem> result)
    {
        result = new List<ProductDetailFormItem>();
        foreach (var kv in form)
        {
            if (kv.Key.Contains("DetailsJson", StringComparison.OrdinalIgnoreCase))
                continue;
            if (!kv.Key.Contains("Details", StringComparison.OrdinalIgnoreCase))
                continue;

            var v = kv.Value.ToString();
            if (string.IsNullOrWhiteSpace(v))
                continue;

            var t = v.Trim().TrimStart('\uFEFF');
            if (!t.StartsWith('[') && !t.StartsWith('{'))
                continue;

            if (TryBindDetailsFromJsonString(v, out var parsed) && parsed.Count > 0)
            {
                result = parsed;
                return true;
            }
        }

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
        var normalized = ProductDetailsJsonNormalize.ForJsonParse(raw);
        var list = TryParseDetailsJsonArrayCore(normalized);
        if (list.Count > 0)
            return list;

        var recovered = ProductDetailsJsonNormalize.TryRecoverUtf8MisreadAsLatin1(normalized);
        if (string.IsNullOrEmpty(recovered))
            return list;

        return TryParseDetailsJsonArrayCore(ProductDetailsJsonNormalize.ForJsonParse(recovered));
    }

    private static List<ProductDetailFormItem> TryParseDetailsJsonArrayCore(string normalized)
    {
        var list = new List<ProductDetailFormItem>();
        try
        {
            var rows = JsonSerializer.Deserialize<List<JsonRow>>(normalized, JsonOptions);
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
                    SortOrder = row.Order ?? row.SortOrder ?? i,
                });
            }
        }
        catch (JsonException)
        {
            // يُعاد المحاولة من المستدعي بعد استرداد الترميز إن وُجد
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

        return new ProductDetailFormItem { Key = key, Value = value, SortOrder = order };
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

        var trimmed = ProductDetailsJsonNormalize.ForJsonParse(raw);
        if (!trimmed.StartsWith('{'))
            return null;

        var item = TryDeserializeSingleJsonRow(trimmed, fallbackIndex);
        if (item != null)
            return item;

        var recovered = ProductDetailsJsonNormalize.TryRecoverUtf8MisreadAsLatin1(trimmed);
        if (string.IsNullOrEmpty(recovered))
            return null;

        trimmed = ProductDetailsJsonNormalize.ForJsonParse(recovered);
        return !trimmed.StartsWith('{') ? null : TryDeserializeSingleJsonRow(trimmed, fallbackIndex);
    }

    private static ProductDetailFormItem? TryDeserializeSingleJsonRow(string json, int fallbackIndex)
    {
        try
        {
            var row = JsonSerializer.Deserialize<JsonRow>(json, JsonOptions);
            if (row == null || string.IsNullOrWhiteSpace(row.Key))
                return null;

            var order = row.Order ?? row.SortOrder ?? fallbackIndex;
            return new ProductDetailFormItem
            {
                Key = row.Key.Trim(),
                Value = row.Value?.Trim() ?? string.Empty,
                SortOrder = order,
            };
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
