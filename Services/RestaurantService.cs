using FoodDelivery.DTOs;
using FoodDelivery.Models;
using FoodDelivery.Interfaces.Services;
using FoodDelivery.Interfaces.Repositories;

namespace FoodDelivery.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IRepository<MenuItem> _menuItemRepository;

        public RestaurantService(IRestaurantRepository restaurantRepository, IRepository<MenuItem> menuItemRepository)
        {
            _restaurantRepository = restaurantRepository;
            _menuItemRepository = menuItemRepository;
        }

        public async Task<IEnumerable<RestaurantDto>> getAllRestaurantsAsync(string? location)
        {
            var restaurants = string.IsNullOrEmpty(location)
                ? await _restaurantRepository.getAllAsync()
                : await _restaurantRepository.findAsync(r => r.Location.Contains(location));

            return restaurants.Select(r => new RestaurantDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                Address = r.Address,
                Location = r.Location,
                OwnerId = r.OwnerId
            });
        }

        public async Task<RestaurantDto?> getRestaurantByIdAsync(int id)
        {
            var r = await _restaurantRepository.getByIdAsync(id);
            if (r == null) return null;

            return new RestaurantDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                Address = r.Address,
                Location = r.Location,
                OwnerId = r.OwnerId
            };
        }

        public async Task<RestaurantDto> createRestaurantAsync(CreateRestaurantRequest request, int ownerId)
        {
            var restaurant = new Restaurant
            {
                Name = request.Name,
                Description = request.Description,
                Address = request.Address,
                Location = request.Location,
                OwnerId = ownerId
            };

            await _restaurantRepository.addAsync(restaurant);
            await _restaurantRepository.completeAsync();

            return new RestaurantDto
            {
                Id = restaurant.Id,
                Name = restaurant.Name,
                Description = restaurant.Description,
                Address = restaurant.Address,
                Location = restaurant.Location,
                OwnerId = restaurant.OwnerId
            };
        }

        public async Task<bool> updateRestaurantAsync(int id, CreateRestaurantRequest request, int ownerId)
        {
            var r = await _restaurantRepository.getByIdAsync(id);
            if (r == null || r.OwnerId != ownerId) return false;

            r.Name = request.Name;
            r.Description = request.Description;
            r.Address = request.Address;
            r.Location = request.Location;

            _restaurantRepository.update(r);
            await _restaurantRepository.completeAsync();
            return true;
        }

        public async Task<bool> deleteRestaurantAsync(int id, int ownerId)
        {
            var r = await _restaurantRepository.getByIdAsync(id);
            if (r == null || r.OwnerId != ownerId) return false;

            _restaurantRepository.remove(r);
            await _restaurantRepository.completeAsync();
            return true;
        }

        public async Task<IEnumerable<MenuItemDto>> getMenuItemsAsync(int restaurantId, string? category)
        {
            var menuItems = await _menuItemRepository.findAsync(m => m.RestaurantId == restaurantId &&
                (string.IsNullOrEmpty(category) || m.Category.Contains(category)));

            return menuItems.Select(m => new MenuItemDto
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description,
                Price = m.Price,
                Category = m.Category,
                RestaurantId = m.RestaurantId
            });
        }

        public async Task<MenuItemDto> addMenuItemAsync(int restaurantId, CreateMenuItemRequest request, int ownerId)
        {
            var restaurant = await _restaurantRepository.getByIdAsync(restaurantId);
            if (restaurant == null || restaurant.OwnerId != ownerId)
                throw new UnauthorizedAccessException("Not authorized to add menu items to this restaurant.");

            var item = new MenuItem
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Category = request.Category,
                RestaurantId = restaurantId
            };

            await _menuItemRepository.addAsync(item);
            await _menuItemRepository.completeAsync();

            return new MenuItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                Category = item.Category,
                RestaurantId = item.RestaurantId
            };
        }

        public async Task<bool> updateMenuItemAsync(int menuItemId, CreateMenuItemRequest request, int ownerId)
        {
            var item = await _menuItemRepository.getByIdAsync(menuItemId);
            if (item == null) return false;

            var restaurant = await _restaurantRepository.getByIdAsync(item.RestaurantId);
            if (restaurant == null || restaurant.OwnerId != ownerId) return false;

            item.Name = request.Name;
            item.Description = request.Description;
            item.Price = request.Price;
            item.Category = request.Category;

            _menuItemRepository.update(item);
            await _menuItemRepository.completeAsync();
            return true;
        }

        public async Task<bool> deleteMenuItemAsync(int menuItemId, int ownerId)
        {
            var item = await _menuItemRepository.getByIdAsync(menuItemId);
            if (item == null) return false;

            var restaurant = await _restaurantRepository.getByIdAsync(item.RestaurantId);
            if (restaurant == null || restaurant.OwnerId != ownerId) return false;

            _menuItemRepository.remove(item);
            await _menuItemRepository.completeAsync();
            return true;
        }
    }
}
