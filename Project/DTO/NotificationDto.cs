using System;
using Project.Models.Enums;

namespace Project.DTO
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public string TypeName => Type.ToString();
        public string Icon { get; set; } 
        public string IconClass { get; set; } 
        public string ColorClass { get; set; } = string.Empty;
        public string BadgeColorClass { get; set; } = string.Empty;
        public string ToastBgClass { get; set; } = string.Empty;
        public string? Url { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public string RelativeTime { get; set; } = string.Empty;
        public string? SenderId { get; set; }
        public string? SenderName { get; set; }
        public string? SenderImage { get; set; }
        public string? ReferenceId { get; set; }
        public string? ReferenceType { get; set; }
    }
}
