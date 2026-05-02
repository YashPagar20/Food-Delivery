using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FoodDelivery.DTOs;
using FoodDelivery.Services;

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
            var result = await _orderService.PlaceOrderAsync(request, GetUserId());
            return CreatedAtAction(nameof(GetOrder), new { id = result.Id }, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var result = await _orderService.GetOrderByIdAsync(id);
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
            var result = await _orderService.GetUserOrdersAsync(GetUserId());
            return Ok(result);
        }

        [HttpGet("restaurant/{restaurantId}")]
        [Authorize(Roles = "RestaurantOwner,Admin")]
        public async Task<IActionResult> GetRestaurantOrders(int restaurantId)
        {
            var result = await _orderService.GetRestaurantOrdersAsync(restaurantId, GetUserId());
            return Ok(result);
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "RestaurantOwner,Admin")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateStatusRequest request)
        {
            var result = await _orderService.UpdateOrderStatusAsync(id, request, GetUserId());
            if (!result) return Forbid();
            return NoContent();
        }
    }
}
