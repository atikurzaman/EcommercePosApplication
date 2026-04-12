using EcommercePos.Shared.Common;

namespace EcommercePos.Shared.Common;

public static class DateTimeHelper
{
    public static DateTime GetBangladeshTime()
    {
        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, 
            TimeZoneInfo.FindSystemTimeZoneById("Bangladesh Standard Time"));
    }

    public static string ToFriendlyDate(DateTime? date)
    {
        if (!date.HasValue) return "N/A";
        
        var diff = DateTime.UtcNow - date.Value;
        
        if (diff.TotalSeconds < 60)
            return "Just now";
        if (diff.TotalMinutes < 60)
            return $"{(int)diff.TotalMinutes} minutes ago";
        if (diff.TotalHours < 24)
            return $"{(int)diff.TotalHours} hours ago";
        if (diff.TotalDays < 7)
            return $"{(int)diff.TotalDays} days ago";
        if (diff.TotalDays < 30)
            return $"{(int)(diff.TotalDays / 7)} weeks ago";
        if (diff.TotalDays < 365)
            return $"{(int)(diff.TotalDays / 30)} months ago";
        
        return date.Value.ToString("MMM dd, yyyy");
    }

    public static string ToShortDate(DateTime? date)
    {
        return date?.ToString("MMM dd, yyyy") ?? "N/A";
    }

    public static string ToLongDate(DateTime? date)
    {
        return date?.ToString("MMMM dd, yyyy") ?? "N/A";
    }

    public static string ToDateTime(DateTime? date)
    {
        return date?.ToString("MMM dd, yyyy HH:mm") ?? "N/A";
    }
}

