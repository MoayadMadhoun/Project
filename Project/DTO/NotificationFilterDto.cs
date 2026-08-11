using System;
using Project.Models.Enums;

namespace Project.DTO
{
    public class NotificationFilterDto
    {
        public string? SearchTerm { get; set; }
        public bool? IsRead { get; set; }
        public NotificationType? Type { get; set; }
        public string? DateFilter { get; set; } // "today", "week", "month", "all"
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class PagedNotificationResult
    {
        public List<NotificationDto> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int UnreadCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPreviousPage => Page > 1;
        public bool HasNextPage => Page < TotalPages;
    }
}
