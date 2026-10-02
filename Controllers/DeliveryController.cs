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
    public class DeliveryController : ControllerBase
    {
        private readonly IDeliveryService _deliveryService;

        public DeliveryController(IDeliveryService deliveryService)
        {
            _deliveryService = deliveryService;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpPost("assign")]
        [Authorize(Roles = "Admin,RestaurantOwner")]
        public async Task<IActionResult> AssignDriver([FromBody] AssignDriverRequest request)
        {
            var result = await _deliveryService.assignDriverAsync(request);
            return Ok(result);
        }

        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetDelivery(int orderId)
        {
            var result = await _deliveryService.getDeliveryByOrderIdAsync(orderId, GetUserId());
            if (result == null) return NotFound("Delivery record not found.");
            return Ok(result);
        }

        [HttpGet("driver/my-deliveries")]
        [Authorize(Roles = "DeliveryPartner,Admin")]
        public async Task<IActionResult> GetMyDeliveries()
        {
            var result = await _deliveryService.getDriverDeliveriesAsync(GetUserId());
            return Ok(result);
        }

        [HttpPatch("{deliveryId}/status")]
        [Authorize(Roles = "DeliveryPartner,Admin")]
        public async Task<IActionResult> UpdateStatus(int deliveryId, [FromBody] UpdateDeliveryStatusRequest request)
        {
            var success = await _deliveryService.updateDeliveryStatusAsync(deliveryId, request, GetUserId());
            if (!success) return BadRequest("Unable to update delivery status.");
            return NoContent();
        }

        [HttpPatch("{deliveryId}/location")]
        [Authorize(Roles = "DeliveryPartner,Admin")]
        public async Task<IActionResult> UpdateLocation(int deliveryId, [FromBody] UpdateLocationRequest request)
        {
            var success = await _deliveryService.updateDriverLocationAsync(deliveryId, request, GetUserId());
            if (!success) return BadRequest("Unable to update driver location.");
            return NoContent();
        }
    }
}
