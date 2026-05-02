using FoodDelivery.DTOs;

namespace FoodDelivery.Services
{
    public interface IRestaurantService
    {
        Task<IEnumerable<RestaurantDto>> GetAllRestaurantsAsync(string? location);
        Task<RestaurantDto?> GetRestaurantByIdAsync(int id);
        Task<RestaurantDto> CreateRestaurantAsync(CreateRestaurantRequest request, int ownerId);
        Task<bool> UpdateRestaurantAsync(int id, CreateRestaurantRequest request, int ownerId);
        Task<bool> DeleteRestaurantAsync(int id, int ownerId);

        Task<IEnumerable<MenuItemDto>> GetMenuItemsAsync(int restaurantId, string? category);
        Task<MenuItemDto> AddMenuItemAsync(int restaurantId, CreateMenuItemRequest request, int ownerId);
        Task<bool> UpdateMenuItemAsync(int menuItemId, CreateMenuItemRequest request, int ownerId);
        Task<bool> DeleteMenuItemAsync(int menuItemId, int ownerId);
    }
}
