using System;

namespace Project.Helpers;

public static class RelativeTimeHelper
{
    public static string GetRelativeTime(DateTime createdAt)
    {
        var utcTime = createdAt.Kind == DateTimeKind.Utc ? createdAt : createdAt.ToUniversalTime();
        var elapsed = DateTime.UtcNow - utcTime;
        if (elapsed.TotalMinutes < 1) return "الآن";
        if (elapsed.TotalMinutes < 2) return "منذ دقيقة";
        if (elapsed.TotalMinutes < 60) return $"منذ {(int)elapsed.TotalMinutes} دقائق";
        if (elapsed.TotalHours < 2) return "منذ ساعة";
        if (elapsed.TotalHours < 3) return "منذ ساعتين";
        if (elapsed.TotalHours < 24) return $"منذ {(int)elapsed.TotalHours} ساعات";
        if (elapsed.TotalDays < 2) return "منذ يوم";
        if (elapsed.TotalDays < 7) return $"منذ {(int)elapsed.TotalDays} أيام";
        if (elapsed.TotalDays < 14) return "منذ أسبوع";
        if (elapsed.TotalDays < 30) return $"منذ {(int)(elapsed.TotalDays / 7)} أسابيع";
        if (elapsed.TotalDays < 60) return "منذ شهر";
        return $"منذ {(int)(elapsed.TotalDays / 30)} أشهر";
    }
}
