using System.Collections.Generic;
using System.Threading.Tasks;
using Project.DTO;
using Project.Models;
using Project.Models.Enums;

namespace Project.Services
{
    public interface INotificationService
    {
        Task<NotificationDto> CreateNotificationAsync(
            string userId,
            string title,
            string message,
            NotificationType type,
            string? url = null,
            string? icon = null,
            string? senderId = null,
            string? referenceId = null,
            string? referenceType = null);

        Task<NotificationDto?> NotifyUserAsync(
            string userId,
            string title,
            string message,
            NotificationType type,
            string? url = null,
            string? icon = null,
            string? senderId = null,
            string? referenceId = null,
            string? referenceType = null);

        Task<List<NotificationDto>> NotifyUsersAsync(
            IEnumerable<string> userIds,
            string title,
            string message,
            NotificationType type,
            string? url = null,
            string? icon = null,
            string? senderId = null,
            string? referenceId = null,
            string? referenceType = null);

        Task<List<NotificationDto>> NotifyRoleAsync(
            string roleName,
            string title,
            string message,
            NotificationType type,
            string? url = null,
            string? icon = null,
            string? senderId = null,
            string? referenceId = null,
            string? referenceType = null);

        Task<List<NotificationDto>> NotifyAllAsync(
            string title,
            string message,
            NotificationType type,
            string? url = null,
            string? icon = null,
            string? senderId = null,
            string? referenceId = null,
            string? referenceType = null);

        Task<List<NotificationDto>> NotifyDepartmentHeadsAsync(int departmentId, string title, string message, NotificationType type, string? url = null, string? icon = null, string? senderId = null, string? referenceId = null, string? referenceType = null);
        Task<List<NotificationDto>> NotifyUniversityTrainingAdminsAsync(int universityId, string title, string message, NotificationType type, string? url = null, string? icon = null, string? senderId = null, string? referenceId = null, string? referenceType = null);
        Task<List<NotificationDto>> NotifyInstitutionTrainingOfficersAsync(int institutionId, string title, string message, NotificationType type, string? url = null, string? icon = null, string? senderId = null, string? referenceId = null, string? referenceType = null);
        Task<NotificationDto?> NotifyStudentAsync(string studentUserId, string title, string message, NotificationType type, string? url = null, string? icon = null, string? senderId = null, string? referenceId = null, string? referenceType = null);

        Task<PagedNotificationResult> GetUserNotificationsAsync(string userId, NotificationFilterDto filter);

        Task<List<NotificationDto>> GetUnreadNotificationsAsync(string userId, int take = 10);

        Task<bool> MarkAsReadAsync(int notificationId, string userId);

        Task<bool> MarkAllAsReadAsync(string userId);

        Task<bool> DeleteNotificationAsync(int notificationId, string userId);

        Task<int> DeleteSelectedAsync(IEnumerable<int> notificationIds, string userId);

        Task<int> DeleteAllReadAsync(string userId);

        Task<int> UnreadCountAsync(string userId);
    }
}
