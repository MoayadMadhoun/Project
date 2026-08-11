using System;
using Project.DTO;
using Project.Models;
using Project.Models.Enums;

namespace Project.Helpers
{
    public static class NotificationHelper
    {
        public static string GetRelativeTime(DateTime dateTime)
        {
            var utcTime = dateTime.Kind == DateTimeKind.Utc ? dateTime : dateTime.ToUniversalTime();
            var timeSpan = DateTime.UtcNow - utcTime;

            if (timeSpan.TotalSeconds < 60)
                return "الآن";

            if (timeSpan.TotalMinutes < 60)
            {
                int minutes = (int)timeSpan.TotalMinutes;
                if (minutes == 1) return "منذ دقيقة واحدة";
                if (minutes == 2) return "منذ دقيقتين";
                if (minutes >= 3 && minutes <= 10) return $"منذ {minutes} دقائق";
                return $"منذ {minutes} دقيقة";
            }

            if (timeSpan.TotalHours < 24)
            {
                int hours = (int)timeSpan.TotalHours;
                if (hours == 1) return "منذ ساعة واحدة";
                if (hours == 2) return "منذ ساعتين";
                if (hours >= 3 && hours <= 10) return $"منذ {hours} ساعات";
                return $"منذ {hours} ساعة";
            }

            if (timeSpan.TotalDays < 2)
                return "أمس";

            if (timeSpan.TotalDays < 7)
            {
                int days = (int)timeSpan.TotalDays;
                if (days == 2) return "منذ يومين";
                if (days >= 3 && days <= 10) return $"منذ {days} أيام";
                return $"منذ {days} يوماً";
            }

            if (timeSpan.TotalDays < 30)
            {
                int weeks = (int)(timeSpan.TotalDays / 7);
                if (weeks == 1) return "منذ أسبوع";
                if (weeks == 2) return "منذ أسبوعين";
                return $"منذ {weeks} أسابيع";
            }

            if (timeSpan.TotalDays < 365)
            {
                int months = (int)(timeSpan.TotalDays / 30);
                if (months == 1) return "منذ شهر";
                if (months == 2) return "منذ شهرين";
                return $"منذ {months} أشهر";
            }

            int years = (int)(timeSpan.TotalDays / 365);
            if (years == 1) return "منذ سنة";
            if (years == 2) return "منذ سنتين";
            return $"منذ {years} سنوات";
        }

        public static string GetIconClass(NotificationType type, string? customIcon = null)
        {
            if (!string.IsNullOrWhiteSpace(customIcon))
                return customIcon;

            return type switch
            {
                NotificationType.Success => "check_circle",
                NotificationType.Info => "info",
                NotificationType.Warning => "warning",
                NotificationType.Error => "error",
                NotificationType.Message => "chat",
                NotificationType.Application => "assignment_ind",
                NotificationType.Evaluation => "grading",
                NotificationType.Training => "work",
                NotificationType.System => "notifications_active",
                _ => "notifications"
            };
        }

        public static string RenderIconHtml(string? iconClass, string extraCss = "text-2xl")
        {
            if (string.IsNullOrWhiteSpace(iconClass))
                iconClass = "notifications";

            if (iconClass.StartsWith("fa-") || iconClass.StartsWith("fa ") || iconClass.StartsWith("fas ") || iconClass.StartsWith("far ") || iconClass.StartsWith("fab ") || iconClass.StartsWith("bi-") || iconClass.StartsWith("bi "))
            {
                return $"<i class=\"{iconClass} {extraCss}\"></i>";
            }

            return $"<span class=\"material-symbols-outlined {extraCss}\">{iconClass}</span>";
        }

        public static string GetColorClass(NotificationType type)
        {
            return type switch
            {
                NotificationType.Success => "text-emerald-600 bg-emerald-50 border-emerald-200 dark:bg-emerald-950/40 dark:text-emerald-400",
                NotificationType.Info => "text-sky-600 bg-sky-50 border-sky-200 dark:bg-sky-950/40 dark:text-sky-400",
                NotificationType.Warning => "text-amber-600 bg-amber-50 border-amber-200 dark:bg-amber-950/40 dark:text-amber-400",
                NotificationType.Error => "text-rose-600 bg-rose-50 border-rose-200 dark:bg-rose-950/40 dark:text-rose-400",
                NotificationType.Message => "text-indigo-600 bg-indigo-50 border-indigo-200 dark:bg-indigo-950/40 dark:text-indigo-400",
                NotificationType.Application => "text-purple-600 bg-purple-50 border-purple-200 dark:bg-purple-950/40 dark:text-purple-400",
                NotificationType.Evaluation => "text-blue-600 bg-blue-50 border-blue-200 dark:bg-blue-950/40 dark:text-blue-400",
                NotificationType.Training => "text-teal-600 bg-teal-50 border-teal-200 dark:bg-teal-950/40 dark:text-teal-400",
                NotificationType.System => "text-slate-600 bg-slate-100 border-slate-200 dark:bg-slate-800 dark:text-slate-300",
                _ => "text-teal-600 bg-teal-50 border-teal-200 dark:bg-teal-950/40 dark:text-teal-400"
            };
        }

        public static string GetBadgeColorClass(NotificationType type)
        {
            return type switch
            {
                NotificationType.Success => "bg-emerald-100 text-emerald-800 border-emerald-200",
                NotificationType.Info => "bg-sky-100 text-sky-800 border-sky-200",
                NotificationType.Warning => "bg-amber-100 text-amber-800 border-amber-200",
                NotificationType.Error => "bg-rose-100 text-rose-800 border-rose-200",
                NotificationType.Message => "bg-indigo-100 text-indigo-800 border-indigo-200",
                NotificationType.Application => "bg-purple-100 text-purple-800 border-purple-200",
                NotificationType.Evaluation => "bg-blue-100 text-blue-800 border-blue-200",
                NotificationType.Training => "bg-teal-100 text-teal-800 border-teal-200",
                NotificationType.System => "bg-slate-100 text-slate-800 border-slate-200",
                _ => "bg-teal-100 text-teal-800 border-teal-200"
            };
        }

        public static string GetToastBgClass(NotificationType type)
        {
            return type switch
            {
                NotificationType.Success => "border-r-4 border-emerald-500",
                NotificationType.Info => "border-r-4 border-sky-500",
                NotificationType.Warning => "border-r-4 border-amber-500",
                NotificationType.Error => "border-r-4 border-rose-500",
                NotificationType.Message => "border-r-4 border-indigo-500",
                NotificationType.Application => "border-r-4 border-purple-500",
                NotificationType.Evaluation => "border-r-4 border-blue-500",
                NotificationType.Training => "border-r-4 border-teal-500",
                NotificationType.System => "border-r-4 border-slate-500",
                _ => "border-r-4 border-teal-500"
            };
        }

        public static NotificationDto ToDto(this Notification notification)
        {
            return new NotificationDto
            {
                Id = notification.Id,
                UserId = notification.UserId,
                Title = notification.Title,
                Message = notification.Message,
                Type = notification.Type,
                Icon = notification.Icon ?? GetIconClass(notification.Type),
                IconClass = GetIconClass(notification.Type, notification.Icon),
                ColorClass = GetColorClass(notification.Type),
                BadgeColorClass = GetBadgeColorClass(notification.Type),
                ToastBgClass = GetToastBgClass(notification.Type),
                Url = notification.Url,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt,
                ReadAt = notification.ReadAt,
                RelativeTime = RelativeTimeHelper.GetRelativeTime(notification.CreatedAt),
                SenderId = notification.SenderId,
                SenderName = notification.Sender?.FullName,
                SenderImage = string.IsNullOrEmpty(notification.Sender?.ProfileImagePath) 
                    ? "/images/default-user.jpg" 
                    : notification.Sender.ProfileImagePath,
                ReferenceId = notification.ReferenceId,
                ReferenceType = notification.ReferenceType
            };
        }
    }
}
