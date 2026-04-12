using EcommercePos.Shared.Common;

namespace EcommercePos.Shared.Common;

public static class CurrencyHelper
{
    public static string FormatPrice(decimal price, string currencySymbol = "Tk", int decimalPlaces = 2)
    {
        return $"{currencySymbol} {price.ToString("N" + decimalPlaces)}";
    }

    public static string FormatPriceShort(decimal price, string currencySymbol = "Tk")
    {
        if (price >= 10000000)
            return $"{currencySymbol} {(price / 10000000):N1}Cr";
        if (price >= 100000)
            return $"{currencySymbol} {(price / 100000):N1}L";
        if (price >= 1000)
            return $"{currencySymbol} {(price / 1000):N1}K";
        return $"{currencySymbol} {price:N0}";
    }

    public static decimal CalculatePercentage(decimal amount, decimal percentage)
    {
        return Math.Round(amount * (percentage / 100), 2);
    }

    public static decimal RoundToTwoDecimals(decimal amount)
    {
        return Math.Round(amount, 2);
    }
}

