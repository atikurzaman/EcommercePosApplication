using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

using SuperShop.Shared.Common;

namespace SuperShop.Shared.Common;

public static class StringHelper
{
    public static string GenerateSlug(string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        text = text.ToLowerInvariant();
        text = RemoveDiacritics(text);
        text = Regex.Replace(text, @"[^a-z0-9\s-]", "");
        text = Regex.Replace(text, @"\s+", "-").Trim();
        text = Regex.Replace(text, @"-+", "-");
        
        return text.Trim('-');
    }

    private static string RemoveDiacritics(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

    public static string GenerateSKU(string productName, int productId)
    {
        var prefix = new string(productName
            .Where(char.IsLetter)
            .Take(3)
            .ToArray())
            .ToUpper();
        
        return $"{prefix}-{productId:D6}";
    }

    public static string GenerateOrderNumber(int orderId)
    {
        return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{orderId:D6}";
    }

    public static string GenerateInvoiceNumber(int orderId)
    {
        return $"INV-{DateTime.UtcNow:yyyyMMdd}-{orderId:D6}";
    }

    public static string GenerateRandomToken(int length = 32)
    {
        var bytes = new byte[length];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    public static string MaskEmail(string email)
    {
        if (string.IsNullOrEmpty(email) || !email.Contains('@'))
            return email;

        var parts = email.Split('@');
        var localPart = parts[0];
        var domain = parts[1];

        if (localPart.Length <= 2)
            return email;

        var masked = localPart[0] + new string('*', localPart.Length - 2) + localPart[^1];
        return $"{masked}@{domain}";
    }

    public static string MaskPhone(string phone)
    {
        if (string.IsNullOrEmpty(phone) || phone.Length < 4)
            return phone;

        return new string('*', phone.Length - 4) + phone[^4..];
    }
}

