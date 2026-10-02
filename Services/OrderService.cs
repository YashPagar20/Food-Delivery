using Microsoft.AspNetCore.SignalR;
using FoodDelivery.DTOs;
using FoodDelivery.Enums;
using FoodDelivery.Models;
using FoodDelivery.Hubs;
using FoodDelivery.Interfaces.Services;
using FoodDelivery.Interfaces.Repositories;

namespace FoodDelivery.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IRepository<MenuItem> _menuItemRepository;
        private readonly ICouponRepository _couponRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHubContext<OrderHub> _hubContext;
        private readonly ICouponService _couponService;
        private readonly INotificationService _notificationService;

        public OrderService(
            IOrderRepository orderRepository,
            IRestaurantRepository restaurantRepository,
            IRepository<MenuItem> menuItemRepository,
            ICouponRepository couponRepository,
            IUserRepository userRepository,
            IHubContext<OrderHub> hubContext,
            ICouponService couponService,
            INotificationService notificationService)
        {
            _orderRepository = orderRepository;
            _restaurantRepository = restaurantRepository;
            _menuItemRepository = menuItemRepository;
            _couponRepository = couponRepository;
            _userRepository = userRepository;
            _hubContext = hubContext;
            _couponService = couponService;
            _notificationService = notificationService;
        }

        public async Task<OrderDto> placeOrderAsync(PlaceOrderRequest request, int customerId)
        {
            var restaurant = await _restaurantRepository.getByIdAsync(request.RestaurantId);
            if (restaurant == null) throw new KeyNotFoundException("Restaurant not found.");

            var order = new Order
            {
                CustomerId = customerId,
                RestaurantId = request.RestaurantId,
                TotalPrice = 0,
                DiscountAmount = 0,
                FinalPrice = 0,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending
            };

            decimal total = 0;

            foreach (var itemReq in request.Items)
            {
                var menuItem = await _menuItemRepository.getByIdAsync(itemReq.MenuItemId);
                if (menuItem == null || menuItem.RestaurantId != request.RestaurantId)
                    throw new InvalidOperationException($"Menu item {itemReq.MenuItemId} not found or doesn't belong to this restaurant.");

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
            order.FinalPrice = total;

            if (!string.IsNullOrWhiteSpace(request.CouponCode))
            {
                var couponResponse = await _couponService.applyCouponAsync(new ApplyCouponRequest
                {
                    Code = request.CouponCode,
                    OrderTotal = total
                });

                if (couponResponse.IsValid)
                {
                    var coupon = await _couponRepository.getByCodeAsync(couponResponse.Code);
                    if (coupon != null)
                    {
                        order.CouponId = coupon.Id;
                        order.Coupon = coupon;
                        order.DiscountAmount = couponResponse.DiscountAmount;
                        order.FinalPrice = couponResponse.FinalTotal;
                        await _couponService.incrementUsageAsync(couponResponse.Code);
                    }
                }
            }

            await _orderRepository.addAsync(order);
            await _orderRepository.completeAsync();

            await _notificationService.sendNotificationAsync(new SendNotificationRequest
            {
                UserId = customerId,
                Title = "Order Placed Successfully",
                Message = $"Your order #{order.Id} with {restaurant.Name} for ${order.FinalPrice:F2} has been placed.",
                Type = NotificationType.InApp
            });

            return MapToDto(order);
        }

        public async Task<OrderDto?> getOrderByIdAsync(int id)
        {
            var order = await _orderRepository.getOrderWithDetailsAsync(id);
            return order == null ? null : MapToDto(order);
        }

        public async Task<IEnumerable<OrderDto>> getUserOrdersAsync(int userId)
        {
            var orders = await _orderRepository.getUserOrdersWithDetailsAsync(userId);
            return orders.Select(MapToDto);
        }

        public async Task<IEnumerable<OrderDto>> getRestaurantOrdersAsync(int restaurantId, int ownerId)
        {
            var restaurant = await _restaurantRepository.getByIdAsync(restaurantId);
            if (restaurant == null || restaurant.OwnerId != ownerId)
                throw new UnauthorizedAccessException("Not authorized to view orders for this restaurant.");

            var orders = await _orderRepository.getRestaurantOrdersWithDetailsAsync(restaurantId);
            return orders.Select(MapToDto);
        }

        public async Task<bool> updateOrderStatusAsync(int orderId, UpdateStatusRequest request, int userId)
        {
            var order = await _orderRepository.getOrderWithDetailsAsync(orderId);
            if (order == null) return false;

            if (order.Restaurant?.OwnerId != userId)
            {
                var user = await _userRepository.getByIdAsync(userId);
                if (user?.Role != UserRole.Admin) return false;
            }

            order.Status = request.Status;
            _orderRepository.update(order);
            await _orderRepository.completeAsync();

            await _hubContext.Clients.Group($"Order_{orderId}")
                .SendAsync("ReceiveStatusUpdate", new { OrderId = orderId, Status = order.Status.ToString() });

            await _notificationService.sendNotificationAsync(new SendNotificationRequest
            {
                UserId = order.CustomerId,
                Title = "Order Status Updated",
                Message = $"Your order #{orderId} status is now {request.Status}.",
                Type = NotificationType.InApp
            });

            return true;
        }

        private static OrderDto MapToDto(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                RestaurantId = order.RestaurantId,
                TotalPrice = order.TotalPrice,
                DiscountAmount = order.DiscountAmount,
                FinalPrice = order.FinalPrice > 0 ? order.FinalPrice : order.TotalPrice,
                CouponCode = order.Coupon?.Code,
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
