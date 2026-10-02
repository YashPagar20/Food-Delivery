using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FoodDelivery.DTOs;
using FoodDelivery.Interfaces.Services;

namespace FoodDelivery.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet("my-notifications")]
        public async Task<IActionResult> GetMyNotifications()
        {
            var result = await _notificationService.getUserNotificationsAsync(GetUserId());
            return Ok(result);
        }

        [HttpPost("send")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SendNotification([FromBody] SendNotificationRequest request)
        {
            var result = await _notificationService.sendNotificationAsync(request);
            return Ok(result);
        }

        [HttpPatch("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var success = await _notificationService.markAsReadAsync(id, GetUserId());
            if (!success) return NotFound("Notification not found.");
            return NoContent();
        }
    }
}
