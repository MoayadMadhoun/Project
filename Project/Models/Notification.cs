using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Project.Models.Enums;

namespace Project.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(450)]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public virtual AspNetUser? User { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;

        [Required]
        public NotificationType Type { get; set; } = NotificationType.Info;

        [MaxLength(100)]
        public string? Icon { get; set; }

        [MaxLength(500)]
        public string? Url { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ReadAt { get; set; }

        [MaxLength(450)]
        public string? SenderId { get; set; }

        [ForeignKey(nameof(SenderId))]
        public virtual AspNetUser? Sender { get; set; }

        [MaxLength(100)]
        public string? ReferenceId { get; set; }

        [MaxLength(100)]
        public string? ReferenceType { get; set; }
    }
}
