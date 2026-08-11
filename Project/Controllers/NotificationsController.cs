using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.DTO;
using Project.Services;

namespace Project.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        [HttpGet]
        public async Task<IActionResult> GetNotifications([FromQuery] NotificationFilterDto filter)
        {
            if (string.IsNullOrEmpty(CurrentUserId))
                return Unauthorized();

            var result = await _notificationService.GetUserNotificationsAsync(CurrentUserId, filter);
            return Ok(result);
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            if (string.IsNullOrEmpty(CurrentUserId))
                return Unauthorized();

            var count = await _notificationService.UnreadCountAsync(CurrentUserId);
            return Ok(new { count });
        }

        [HttpGet("recent")]
        public async Task<IActionResult> GetRecent([FromQuery] int take = 8)
        {
            if (string.IsNullOrEmpty(CurrentUserId))
                return Unauthorized();

            var items = await _notificationService.GetUnreadNotificationsAsync(CurrentUserId, take);
            
            // If less than take, fetch recent read items to fill dropdown
            if (items.Count < take)
            {
                var pagedResult = await _notificationService.GetUserNotificationsAsync(
                    CurrentUserId, 
                    new NotificationFilterDto { Page = 1, PageSize = take });
                items = pagedResult.Items;
            }

            var unreadCount = await _notificationService.UnreadCountAsync(CurrentUserId);

            return Ok(new { items, unreadCount });
        }

        [HttpPost("{id}/mark-read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            if (string.IsNullOrEmpty(CurrentUserId))
                return Unauthorized();

            var success = await _notificationService.MarkAsReadAsync(id, CurrentUserId);
            if (!success)
                return NotFound(new { message = "Notification not found or access denied." });

            var unreadCount = await _notificationService.UnreadCountAsync(CurrentUserId);
            return Ok(new { success = true, unreadCount });
        }

        [HttpPost("mark-all-read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            if (string.IsNullOrEmpty(CurrentUserId))
                return Unauthorized();

            await _notificationService.MarkAllAsReadAsync(CurrentUserId);
            return Ok(new { success = true, unreadCount = 0 });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            if (string.IsNullOrEmpty(CurrentUserId))
                return Unauthorized();

            var success = await _notificationService.DeleteNotificationAsync(id, CurrentUserId);
            if (!success)
                return NotFound(new { message = "Notification not found or access denied." });

            var unreadCount = await _notificationService.UnreadCountAsync(CurrentUserId);
            return Ok(new { success = true, unreadCount });
        }

        [HttpPost("delete-selected")]
        public async Task<IActionResult> DeleteSelected([FromBody] List<int> ids)
        {
            if (string.IsNullOrEmpty(CurrentUserId))
                return Unauthorized();

            if (ids == null || ids.Count == 0)
                return BadRequest(new { message = "No notification IDs provided." });

            var deletedCount = await _notificationService.DeleteSelectedAsync(ids, CurrentUserId);
            var unreadCount = await _notificationService.UnreadCountAsync(CurrentUserId);
            return Ok(new { success = true, deletedCount, unreadCount });
        }

        [HttpPost("delete-all-read")]
        public async Task<IActionResult> DeleteAllRead()
        {
            if (string.IsNullOrEmpty(CurrentUserId))
                return Unauthorized();

            var deletedCount = await _notificationService.DeleteAllReadAsync(CurrentUserId);
            return Ok(new { success = true, deletedCount });
        }
    }
}
