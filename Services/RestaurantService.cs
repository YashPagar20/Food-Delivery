using Microsoft.EntityFrameworkCore;
using FoodDelivery.Data;
using FoodDelivery.DTOs;
using FoodDelivery.Models;

namespace FoodDelivery.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly ApplicationDbContext _context;

        public RestaurantService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RestaurantDto>> GetAllRestaurantsAsync(string? location)
        {
            var query = _context.Restaurants.AsQueryable();
            if (!string.IsNullOrEmpty(location))
            {
                query = query.Where(r => r.Location.Contains(location));
            }

            return await query.Select(r => new RestaurantDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                Address = r.Address,
                Location = r.Location,
                OwnerId = r.OwnerId
            }).ToListAsync();
        }

        public async Task<RestaurantDto?> GetRestaurantByIdAsync(int id)
        {
            var r = await _context.Restaurants.FindAsync(id);
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

        public async Task<RestaurantDto> CreateRestaurantAsync(CreateRestaurantRequest request, int ownerId)
        {
            var restaurant = new Restaurant
            {
                Name = request.Name,
                Description = request.Description,
                Address = request.Address,
                Location = request.Location,
                OwnerId = ownerId
            };

            _context.Restaurants.Add(restaurant);
            await _context.SaveChangesAsync();

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

        public async Task<bool> UpdateRestaurantAsync(int id, CreateRestaurantRequest request, int ownerId)
        {
            var r = await _context.Restaurants.FindAsync(id);
            if (r == null || r.OwnerId != ownerId) return false;

            r.Name = request.Name;
            r.Description = request.Description;
            r.Address = request.Address;
            r.Location = request.Location;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteRestaurantAsync(int id, int ownerId)
        {
            var r = await _context.Restaurants.FindAsync(id);
            if (r == null || r.OwnerId != ownerId) return false;

            _context.Restaurants.Remove(r);
            await _context.SaveChangesAsync();
            return true;
        }

        // Menu Item Logic
        public async Task<IEnumerable<MenuItemDto>> GetMenuItemsAsync(int restaurantId, string? category)
        {
            var query = _context.MenuItems.Where(m => m.RestaurantId == restaurantId);
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(m => m.Category.Contains(category));
            }

            return await query.Select(m => new MenuItemDto
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description,
                Price = m.Price,
                Category = m.Category,
                RestaurantId = m.RestaurantId
            }).ToListAsync();
        }

        public async Task<MenuItemDto> AddMenuItemAsync(int restaurantId, CreateMenuItemRequest request, int ownerId)
        {
            var restaurant = await _context.Restaurants.FindAsync(restaurantId);
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

            _context.MenuItems.Add(item);
            await _context.SaveChangesAsync();

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

        public async Task<bool> UpdateMenuItemAsync(int menuItemId, CreateMenuItemRequest request, int ownerId)
        {
            var item = await _context.MenuItems.Include(m => m.Restaurant).FirstOrDefaultAsync(m => m.Id == menuItemId);
            if (item == null || item.Restaurant?.OwnerId != ownerId) return false;

            item.Name = request.Name;
            item.Description = request.Description;
            item.Price = request.Price;
            item.Category = request.Category;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteMenuItemAsync(int menuItemId, int ownerId)
        {
            var item = await _context.MenuItems.Include(m => m.Restaurant).FirstOrDefaultAsync(m => m.Id == menuItemId);
            if (item == null || item.Restaurant?.OwnerId != ownerId) return false;

            _context.MenuItems.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
