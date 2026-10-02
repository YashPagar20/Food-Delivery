using FoodDelivery.DTOs;

namespace FoodDelivery.Interfaces.Services
{
    public interface IReviewService
    {
        Task<ReviewDto> addReviewAsync(CreateReviewRequest request, int customerId);
        Task<RestaurantRatingSummaryDto> getRestaurantReviewsAsync(int restaurantId);
    }
}
