using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FoodDelivery.DTOs;
using FoodDelivery.Services;

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
            var result = await _restaurantService.GetAllRestaurantsAsync(location);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRestaurant(int id)
        {
            var result = await _restaurantService.GetRestaurantByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "RestaurantOwner,Admin")]
        public async Task<IActionResult> CreateRestaurant(CreateRestaurantRequest request)
        {
            var result = await _restaurantService.CreateRestaurantAsync(request, GetUserId());
            return CreatedAtAction(nameof(GetRestaurant), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "RestaurantOwner,Admin")]
        public async Task<IActionResult> UpdateRestaurant(int id, CreateRestaurantRequest request)
        {
            var result = await _restaurantService.UpdateRestaurantAsync(id, request, GetUserId());
            if (!result) return Forbid();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "RestaurantOwner,Admin")]
        public async Task<IActionResult> DeleteRestaurant(int id)
        {
            var result = await _restaurantService.DeleteRestaurantAsync(id, GetUserId());
            if (!result) return Forbid();
            return NoContent();
        }

        // Menu Endpoints
        [HttpGet("{restaurantId}/menu")]
        public async Task<IActionResult> GetMenu(int restaurantId, [FromQuery] string? category)
        {
            var result = await _restaurantService.GetMenuItemsAsync(restaurantId, category);
            return Ok(result);
        }

        [HttpPost("{restaurantId}/menu")]
        [Authorize(Roles = "RestaurantOwner,Admin")]
        public async Task<IActionResult> AddMenuItem(int restaurantId, CreateMenuItemRequest request)
        {
            var result = await _restaurantService.AddMenuItemAsync(restaurantId, request, GetUserId());
            return Ok(result);
        }

        [HttpPut("menu/{menuItemId}")]
        [Authorize(Roles = "RestaurantOwner,Admin")]
        public async Task<IActionResult> UpdateMenuItem(int menuItemId, CreateMenuItemRequest request)
        {
            var result = await _restaurantService.UpdateMenuItemAsync(menuItemId, request, GetUserId());
            if (!result) return Forbid();
            return NoContent();
        }

        [HttpDelete("menu/{menuItemId}")]
        [Authorize(Roles = "RestaurantOwner,Admin")]
        public async Task<IActionResult> DeleteMenuItem(int menuItemId)
        {
            var result = await _restaurantService.DeleteMenuItemAsync(menuItemId, GetUserId());
            if (!result) return Forbid();
            return NoContent();
        }
    }
}
