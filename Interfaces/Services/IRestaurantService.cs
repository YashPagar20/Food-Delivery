using FoodDelivery.DTOs;

namespace FoodDelivery.Interfaces.Services
{
    public interface IRestaurantService
    {
        Task<IEnumerable<RestaurantDto>> getAllRestaurantsAsync(string? location = null);
        Task<RestaurantDto?> getRestaurantByIdAsync(int id);
        Task<RestaurantDto> createRestaurantAsync(CreateRestaurantRequest request, int ownerId);
        Task<bool> updateRestaurantAsync(int id, CreateRestaurantRequest request, int ownerId);
        Task<bool> deleteRestaurantAsync(int id, int ownerId);

        Task<IEnumerable<MenuItemDto>> getMenuItemsAsync(int restaurantId, string? category = null);
        Task<MenuItemDto> addMenuItemAsync(int restaurantId, CreateMenuItemRequest request, int ownerId);
        Task<bool> updateMenuItemAsync(int menuItemId, CreateMenuItemRequest request, int ownerId);
        Task<bool> deleteMenuItemAsync(int menuItemId, int ownerId);
    }
}
