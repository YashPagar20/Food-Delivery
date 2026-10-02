using System.ComponentModel.DataAnnotations;

namespace FoodDelivery.DTOs
{
    public class CreateReviewRequest
    {
        [Required]
        public int OrderId { get; set; }

        [Required, Range(1, 5)]
        public int Rating { get; set; }

        public string Comment { get; set; } = string.Empty;
    }

    public class ReviewDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int RestaurantId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class RestaurantRatingSummaryDto
    {
        public int RestaurantId { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public List<ReviewDto> RecentReviews { get; set; } = new();
    }
}
