using System.ComponentModel.DataAnnotations;

namespace FoodDelivery.DTOs
{
    public class RestaurantDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int OwnerId { get; set; }
    }

    public class CreateRestaurantRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        [Required]
        public string Location { get; set; } = string.Empty;
    }

    public class MenuItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
        public int RestaurantId { get; set; }
    }

    public class CreateMenuItemRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [Required, Range(0.01, 10000)]
        public decimal Price { get; set; }
        [Required]
        public string Category { get; set; } = string.Empty;
    }
}
