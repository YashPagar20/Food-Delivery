using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using FoodDelivery.Data;
using FoodDelivery.DTOs;
using FoodDelivery.Models;
using FoodDelivery.Hubs;

namespace FoodDelivery.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<OrderHub> _hubContext;

        public OrderService(ApplicationDbContext context, IHubContext<OrderHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<OrderDto> PlaceOrderAsync(PlaceOrderRequest request, int customerId)
        {
            var restaurant = await _context.Restaurants.FindAsync(request.RestaurantId);
            if (restaurant == null) throw new Exception("Restaurant not found.");

            var order = new Order
            {
                CustomerId = customerId,
                RestaurantId = request.RestaurantId,
                TotalPrice = 0, // Will calculate below
                OrderDate = DateTime.UtcNow,
                Status = Enums.OrderStatus.Pending
            };

            decimal total = 0;

            foreach (var itemReq in request.Items)
            {
                var menuItem = await _context.MenuItems.FindAsync(itemReq.MenuItemId);
                if (menuItem == null || menuItem.RestaurantId != request.RestaurantId)
                    throw new Exception($"Menu item {itemReq.MenuItemId} not found or doesn't belong to this restaurant.");

                var orderItem = new OrderItem
                {
                    MenuItemId = itemReq.MenuItemId,
                    Quantity = itemReq.Quantity,
                    UnitPrice = menuItem.Price
                };

                order.OrderItems.Add(orderItem);
                total += menuItem.Price * itemReq.Quantity;
            }

            order.TotalPrice = total;
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return MapToDto(order);
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(o => o.Id == id);

            return order == null ? null : MapToDto(order);
        }

        public async Task<IEnumerable<OrderDto>> GetUserOrdersAsync(int userId)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .Where(o => o.CustomerId == userId)
                .ToListAsync();

            return orders.Select(MapToDto);
        }

        public async Task<IEnumerable<OrderDto>> GetRestaurantOrdersAsync(int restaurantId, int ownerId)
        {
            var restaurant = await _context.Restaurants.FindAsync(restaurantId);
            if (restaurant == null || restaurant.OwnerId != ownerId)
                throw new UnauthorizedAccessException("Not authorized to view orders for this restaurant.");

            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .Where(o => o.RestaurantId == restaurantId)
                .ToListAsync();

            return orders.Select(MapToDto);
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, UpdateStatusRequest request, int userId)
        {
            var order = await _context.Orders.Include(o => o.Restaurant).FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null) return false;

            // Only restaurant owner or admin can update status
            if (order.Restaurant?.OwnerId != userId)
            {
                // check if admin (simplification: assume only owner for now or check user role from DB)
                var user = await _context.Users.FindAsync(userId);
                if (user?.Role != Enums.UserRole.Admin) return false;
            }

            order.Status = request.Status;
            await _context.SaveChangesAsync();

            // Send Real-time notification
            await _hubContext.Clients.Group($"Order_{orderId}")
                .SendAsync("ReceiveStatusUpdate", new { OrderId = orderId, Status = order.Status.ToString() });

            return true;
        }

        private OrderDto MapToDto(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                RestaurantId = order.RestaurantId,
                TotalPrice = order.TotalPrice,
                Status = order.Status,
                OrderDate = order.OrderDate,
                Items = order.OrderItems.Select(oi => new OrderItemDto
                {
                    MenuItemId = oi.MenuItemId,
                    MenuItemName = oi.MenuItem?.Name ?? "Unknown",
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            };
        }
    }
}
