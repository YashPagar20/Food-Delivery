using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FoodDelivery.DTOs;
using FoodDelivery.Interfaces.Services;

namespace FoodDelivery.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RestaurantController : ControllerBase
    {
        private readonly IRestaurantService _restaurantService;

        public RestaurantController(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<IActionResult> GetRestaurants([FromQuery] string? location)
        {
            var result = await _restaurantService.getAllRestaurantsAsync(location);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRestaurant(int id)
        {
            var result = await _restaurantService.getRestaurantByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "RestaurantOwner,Admin")]
        public async Task<IActionResult> CreateRestaurant(CreateRestaurantRequest request)
        {
            var result = await _restaurantService.createRestaurantAsync(request, GetUserId());
            return CreatedAtAction(nameof(GetRestaurant), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "RestaurantOwner,Admin")]
        public async Task<IActionResult> UpdateRestaurant(int id, CreateRestaurantRequest request)
        {
            var result = await _restaurantService.updateRestaurantAsync(id, request, GetUserId());
            if (!result) return Forbid();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "RestaurantOwner,Admin")]
        public async Task<IActionResult> DeleteRestaurant(int id)
        {
            var result = await _restaurantService.deleteRestaurantAsync(id, GetUserId());
            if (!result) return Forbid();
            return NoContent();
        }

        // Menu Endpoints
        [HttpGet("{restaurantId}/menu")]
        public async Task<IActionResult> GetMenu(int restaurantId, [FromQuery] string? category)
        {
            var result = await _restaurantService.getMenuItemsAsync(restaurantId, category);
            return Ok(result);
        }

        [HttpPost("{restaurantId}/menu")]
        [Authorize(Roles = "RestaurantOwner,Admin")]
        public async Task<IActionResult> AddMenuItem(int restaurantId, CreateMenuItemRequest request)
        {
            var result = await _restaurantService.addMenuItemAsync(restaurantId, request, GetUserId());
            return Ok(result);
        }

        [HttpPut("menu/{menuItemId}")]
        [Authorize(Roles = "RestaurantOwner,Admin")]
        public async Task<IActionResult> UpdateMenuItem(int menuItemId, CreateMenuItemRequest request)
        {
            var result = await _restaurantService.updateMenuItemAsync(menuItemId, request, GetUserId());
            if (!result) return Forbid();
            return NoContent();
        }

        [HttpDelete("menu/{menuItemId}")]
        [Authorize(Roles = "RestaurantOwner,Admin")]
        public async Task<IActionResult> DeleteMenuItem(int menuItemId)
        {
            var result = await _restaurantService.deleteMenuItemAsync(menuItemId, GetUserId());
            if (!result) return Forbid();
            return NoContent();
        }
    }
}
