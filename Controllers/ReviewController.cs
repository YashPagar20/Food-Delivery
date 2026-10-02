using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FoodDelivery.DTOs;
using FoodDelivery.Interfaces.Services;

namespace FoodDelivery.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpPost]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> AddReview([FromBody] CreateReviewRequest request)
        {
            var result = await _reviewService.addReviewAsync(request, GetUserId());
            return Ok(result);
        }

        [HttpGet("restaurant/{restaurantId}")]
        public async Task<IActionResult> GetRestaurantReviews(int restaurantId)
        {
            var result = await _reviewService.getRestaurantReviewsAsync(restaurantId);
            return Ok(result);
        }
    }
}
