using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.DTO;
using Project.Helpers;
using Project.Hubs;
using Project.Models;
using Project.Models.Enums;

namespace Project.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;
        private readonly UserManager<AspNetUser> _userManager;

        public NotificationService(
            ApplicationDbContext context,
            IHubContext<NotificationHub, INotificationClient> hubContext,
            UserManager<AspNetUser> userManager)
        {
            _context = context;
            _hubContext = hubContext;
            _userManager = userManager;
        }

        public async Task<NotificationDto> CreateNotificationAsync(
            string userId,
            string title,
            string message,
            NotificationType type,
            string? url = null,
            string? icon = null,
            string? senderId = null,
            string? referenceId = null,
            string? referenceType = null)
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                Icon = icon,
                Url = url,
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                SenderId = senderId,
                ReferenceId = referenceId,
                ReferenceType = referenceType
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            // Load Sender if provided
            if (!string.IsNullOrEmpty(senderId))
            {
                await _context.Entry(notification)
                    .Reference(n => n.Sender)
                    .LoadAsync();
            }

            return notification.ToDto();
        }

        public async Task<NotificationDto?> NotifyUserAsync(
            string userId,
            string title,
            string message,
            NotificationType type,
            string? url = null,
            string? icon = null,
            string? senderId = null,
            string? referenceId = null,
            string? referenceType = null)
        {
            if (string.IsNullOrEmpty(userId))
                return null;

            var dto = await CreateNotificationAsync(
                userId, title, message, type, url, icon, senderId, referenceId, referenceType);

            // Send via SignalR
            await _hubContext.Clients.User(userId).ReceiveNotification(dto);

            // Send updated unread count
            var unreadCount = await UnreadCountAsync(userId);
            await _hubContext.Clients.User(userId).UpdateUnreadCount(unreadCount);

            return dto;
        }

        public async Task<List<NotificationDto>> NotifyUsersAsync(
            IEnumerable<string> userIds,
            string title,
            string message,
            NotificationType type,
            string? url = null,
            string? icon = null,
            string? senderId = null,
            string? referenceId = null,
            string? referenceType = null)
        {
            var dtos = new List<NotificationDto>();
            var distinctUserIds = userIds.Where(u => !string.IsNullOrEmpty(u)).Distinct().ToList();

            if (!distinctUserIds.Any())
                return dtos;

            foreach (var userId in distinctUserIds)
            {
                var dto = await NotifyUserAsync(
                    userId, title, message, type, url, icon, senderId, referenceId, referenceType);

                if (dto != null)
                {
                    dtos.Add(dto);
                }
            }

            return dtos;
        }

        public async Task<List<NotificationDto>> NotifyRoleAsync(
            string roleName,
            string title,
            string message,
            NotificationType type,
            string? url = null,
            string? icon = null,
            string? senderId = null,
            string? referenceId = null,
            string? referenceType = null)
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);
            var userIds = usersInRole.Select(u => u.Id).ToList();

            return await NotifyUsersAsync(
                userIds, title, message, type, url, icon, senderId, referenceId, referenceType);
        }

        public async Task<List<NotificationDto>> NotifyAllAsync(
            string title,
            string message,
            NotificationType type,
            string? url = null,
            string? icon = null,
            string? senderId = null,
            string? referenceId = null,
            string? referenceType = null)
        {
            var allUserIds = await _context.Users.Select(u => u.Id).ToListAsync();
            return await NotifyUsersAsync(
                allUserIds, title, message, type, url, icon, senderId, referenceId, referenceType);
        }

        public Task<List<NotificationDto>> NotifyDepartmentHeadsAsync(int departmentId, string title, string message, NotificationType type, string? url = null, string? icon = null, string? senderId = null, string? referenceId = null, string? referenceType = null)
            => NotifyScopedRoleAsync("DepartmentHead", s => s.DepartmentID == departmentId, title, message, type, url, icon, senderId, referenceId, referenceType);

        public Task<List<NotificationDto>> NotifyUniversityTrainingAdminsAsync(int universityId, string title, string message, NotificationType type, string? url = null, string? icon = null, string? senderId = null, string? referenceId = null, string? referenceType = null)
            => NotifyScopedRoleAsync("UniversityTrainingAdmin", s => s.UniversityID == universityId, title, message, type, url, icon, senderId, referenceId, referenceType);

        public Task<List<NotificationDto>> NotifyInstitutionTrainingOfficersAsync(int institutionId, string title, string message, NotificationType type, string? url = null, string? icon = null, string? senderId = null, string? referenceId = null, string? referenceType = null)
            => NotifyScopedRoleAsync("InstitutionTrainingOfficer", s => s.InstitutionID == institutionId, title, message, type, url, icon, senderId, referenceId, referenceType);

        public Task<NotificationDto?> NotifyStudentAsync(string studentUserId, string title, string message, NotificationType type, string? url = null, string? icon = null, string? senderId = null, string? referenceId = null, string? referenceType = null)
            => NotifyUserAsync(studentUserId, title, message, type, url, icon, senderId, referenceId, referenceType);

        private async Task<List<NotificationDto>> NotifyScopedRoleAsync(string roleName, System.Linq.Expressions.Expression<Func<AspNetRoleScope, bool>> scopeFilter, string title, string message, NotificationType type, string? url, string? icon, string? senderId, string? referenceId, string? referenceType)
        {
            var userIds = await _context.AspNetRoleScopes.AsNoTracking()
                .Where(s => s.IsActive && s.Role.Name == roleName).Where(scopeFilter)
                .Select(s => s.UserID).Distinct().ToListAsync();
            return await NotifyUsersAsync(userIds, title, message, type, url, icon, senderId, referenceId, referenceType);
        }

        public async Task<PagedNotificationResult> GetUserNotificationsAsync(string userId, NotificationFilterDto filter)
        {
            var query = _context.Notifications
                .AsNoTracking()
                .Include(n => n.Sender)
                .Where(n => n.UserId == userId);

            // Search Filter
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.Trim().ToLower();
                query = query.Where(n => n.Title.ToLower().Contains(term) || n.Message.ToLower().Contains(term));
            }

            // IsRead Filter
            if (filter.IsRead.HasValue)
            {
                query = query.Where(n => n.IsRead == filter.IsRead.Value);
            }

            // Type Filter
            if (filter.Type.HasValue)
            {
                query = query.Where(n => n.Type == filter.Type.Value);
            }

            // Date Filter
            if (!string.IsNullOrWhiteSpace(filter.DateFilter))
            {
                var now = DateTime.UtcNow;
                switch (filter.DateFilter.ToLower())
                {
                    case "today":
                        var startOfDay = now.Date;
                        query = query.Where(n => n.CreatedAt >= startOfDay);
                        break;
                    case "week":
                        var startOfWeek = now.AddDays(-7);
                        query = query.Where(n => n.CreatedAt >= startOfWeek);
                        break;
                    case "month":
                        var startOfMonth = now.AddDays(-30);
                        query = query.Where(n => n.CreatedAt >= startOfMonth);
                        break;
                }
            }

            var totalCount = await query.CountAsync();
            var unreadCount = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .CountAsync();

            var items = await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedNotificationResult
            {
                Items = items.Select(n => n.ToDto()).ToList(),
                TotalCount = totalCount,
                UnreadCount = unreadCount,
                Page = filter.Page,
                PageSize = filter.PageSize
            };
        }

        public async Task<List<NotificationDto>> GetUnreadNotificationsAsync(string userId, int take = 10)
        {
            var notifications = await _context.Notifications
                .AsNoTracking()
                .Include(n => n.Sender)
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .Take(take)
                .ToListAsync();

            return notifications.Select(n => n.ToDto()).ToList();
        }

        public async Task<bool> MarkAsReadAsync(int notificationId, string userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

            if (notification == null)
                return false;

            if (!notification.IsRead)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                var unreadCount = await UnreadCountAsync(userId);
                await _hubContext.Clients.User(userId).NotificationMarkedRead(notificationId);
                await _hubContext.Clients.User(userId).UpdateUnreadCount(unreadCount);
            }

            return true;
        }

        public async Task<bool> MarkAllAsReadAsync(string userId)
        {
            var unreadNotifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            if (!unreadNotifications.Any())
                return true;

            var now = DateTime.UtcNow;
            foreach (var n in unreadNotifications)
            {
                n.IsRead = true;
                n.ReadAt = now;
            }

            await _context.SaveChangesAsync();

            await _hubContext.Clients.User(userId).AllNotificationsMarkedRead();
            await _hubContext.Clients.User(userId).UpdateUnreadCount(0);

            return true;
        }

        public async Task<bool> DeleteNotificationAsync(int notificationId, string userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

            if (notification == null)
                return false;

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();

            var unreadCount = await UnreadCountAsync(userId);
            await _hubContext.Clients.User(userId).NotificationDeleted(notificationId);
            await _hubContext.Clients.User(userId).UpdateUnreadCount(unreadCount);

            return true;
        }

        public async Task<int> DeleteSelectedAsync(IEnumerable<int> notificationIds, string userId)
        {
            var notifications = await _context.Notifications
                .Where(n => notificationIds.Contains(n.Id) && n.UserId == userId)
                .ToListAsync();

            if (!notifications.Any())
                return 0;

            var count = notifications.Count;
            _context.Notifications.RemoveRange(notifications);
            await _context.SaveChangesAsync();

            var unreadCount = await UnreadCountAsync(userId);
            await _hubContext.Clients.User(userId).UpdateUnreadCount(unreadCount);

            return count;
        }

        public async Task<int> DeleteAllReadAsync(string userId)
        {
            var readNotifications = await _context.Notifications
                .Where(n => n.UserId == userId && n.IsRead)
                .ToListAsync();

            if (!readNotifications.Any())
                return 0;

            var count = readNotifications.Count;
            _context.Notifications.RemoveRange(readNotifications);
            await _context.SaveChangesAsync();

            return count;
        }

        public async Task<int> UnreadCountAsync(string userId)
        {
            return await _context.Notifications
                .AsNoTracking()
                .CountAsync(n => n.UserId == userId && !n.IsRead);
        }
    }
}
