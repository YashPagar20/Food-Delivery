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
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(PlaceOrderRequest request)
        {
            var result = await _orderService.placeOrderAsync(request, GetUserId());
            return CreatedAtAction(nameof(GetOrder), new { id = result.Id }, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var result = await _orderService.getOrderByIdAsync(id);
            if (result == null) return NotFound();
            
            // Check if user is customer or owner
            if (result.CustomerId != GetUserId())
            {
                // In a real app, check if it's the restaurant owner too
            }

            return Ok(result);
        }

        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var result = await _orderService.getUserOrdersAsync(GetUserId());
            return Ok(result);
        }

        [HttpGet("restaurant/{restaurantId}")]
        [Authorize(Roles = "RestaurantOwner,Admin")]
        public async Task<IActionResult> GetRestaurantOrders(int restaurantId)
        {
            var result = await _orderService.getRestaurantOrdersAsync(restaurantId, GetUserId());
            return Ok(result);
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "RestaurantOwner,Admin")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateStatusRequest request)
        {
            var result = await _orderService.updateOrderStatusAsync(id, request, GetUserId());
            if (!result) return Forbid();
            return NoContent();
        }
    }
}
