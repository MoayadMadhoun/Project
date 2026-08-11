using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Project.DTO;
using Project.Services;

namespace Project.Hubs
{
    public interface INotificationClient
    {
        Task ReceiveNotification(NotificationDto notification);
        Task UpdateUnreadCount(int unreadCount);
        Task NotificationMarkedRead(int notificationId);
        Task AllNotificationsMarkedRead();
        Task NotificationDeleted(int notificationId);
    }

    [Authorize]
    public class NotificationHub : Hub<INotificationClient>
    {
        private readonly INotificationService _notificationService;

        public NotificationHub(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                var unreadCount = await _notificationService.UnreadCountAsync(userId);
                await Clients.Caller.UpdateUnreadCount(unreadCount);
            }

            await base.OnConnectedAsync();
        }

        public async Task MarkAsRead(int notificationId)
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                var success = await _notificationService.MarkAsReadAsync(notificationId, userId);
                if (success)
                {
                    var unreadCount = await _notificationService.UnreadCountAsync(userId);
                    await Clients.Caller.NotificationMarkedRead(notificationId);
                    await Clients.Caller.UpdateUnreadCount(unreadCount);
                }
            }
        }

        public async Task MarkAllAsRead()
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                await _notificationService.MarkAllAsReadAsync(userId);
                await Clients.Caller.AllNotificationsMarkedRead();
                await Clients.Caller.UpdateUnreadCount(0);
            }
        }

        public async Task<int> GetUnreadCount()
        {
            var userId = Context.UserIdentifier;
            if (string.IsNullOrEmpty(userId))
                return 0;

            return await _notificationService.UnreadCountAsync(userId);
        }

        public async Task SendNotificationToUser(string targetUserId, NotificationDto notification)
        {
            await Clients.User(targetUserId).ReceiveNotification(notification);
        }

        public async Task SendNotificationToUsers(List<string> targetUserIds, NotificationDto notification)
        {
            await Clients.Users(targetUserIds).ReceiveNotification(notification);
        }

        public async Task BroadcastNotification(NotificationDto notification)
        {
            await Clients.All.ReceiveNotification(notification);
        }
    }
}
