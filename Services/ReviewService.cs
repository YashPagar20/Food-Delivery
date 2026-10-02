using FoodDelivery.DTOs;
using FoodDelivery.Enums;
using FoodDelivery.Models;
using FoodDelivery.Interfaces.Services;
using FoodDelivery.Interfaces.Repositories;

namespace FoodDelivery.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;

        public ReviewService(
            IReviewRepository reviewRepository,
            IOrderRepository orderRepository,
            IUserRepository userRepository)
        {
            _reviewRepository = reviewRepository;
            _orderRepository = orderRepository;
            _userRepository = userRepository;
        }

        public async Task<ReviewDto> addReviewAsync(CreateReviewRequest request, int customerId)
        {
            var order = await _orderRepository.getByIdAsync(request.OrderId);
            if (order == null)
                throw new KeyNotFoundException($"Order #{request.OrderId} not found.");

            if (order.CustomerId != customerId)
                throw new UnauthorizedAccessException("You can only review your own orders.");

            if (order.Status != OrderStatus.Delivered)
                throw new InvalidOperationException("You can only submit a review after the order has been delivered.");

            var existingReview = await _reviewRepository.getByOrderIdAsync(request.OrderId);
            if (existingReview != null)
                throw new InvalidOperationException("A review has already been submitted for this order.");

            var review = new Review
            {
                OrderId = request.OrderId,
                CustomerId = customerId,
                RestaurantId = order.RestaurantId,
                Rating = request.Rating,
                Comment = request.Comment,
                CreatedAt = DateTime.UtcNow
            };

            await _reviewRepository.addAsync(review);
            await _reviewRepository.completeAsync();

            var customer = await _userRepository.getByIdAsync(customerId);
            return MapToDto(review, customer?.Username ?? "Customer");
        }

        public async Task<RestaurantRatingSummaryDto> getRestaurantReviewsAsync(int restaurantId)
        {
            var reviews = (await _reviewRepository.getRestaurantReviewsAsync(restaurantId)).ToList();

            double avgRating = reviews.Count > 0 ? Math.Round(reviews.Average(r => r.Rating), 1) : 0;

            return new RestaurantRatingSummaryDto
            {
                RestaurantId = restaurantId,
                AverageRating = avgRating,
                TotalReviews = reviews.Count,
                RecentReviews = reviews.Select(r => MapToDto(r, r.Customer?.Username ?? "Customer")).ToList()
            };
        }

        private static ReviewDto MapToDto(Review review, string customerName)
        {
            return new ReviewDto
            {
                Id = review.Id,
                OrderId = review.OrderId,
                CustomerId = review.CustomerId,
                CustomerName = customerName,
                RestaurantId = review.RestaurantId,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt
            };
        }
    }
}
