using FoodDelivery.DTOs;

namespace FoodDelivery.Services
{
    public interface IOrderService
    {
        Task<OrderDto> PlaceOrderAsync(PlaceOrderRequest request, int customerId);
        Task<OrderDto?> GetOrderByIdAsync(int id);
        Task<IEnumerable<OrderDto>> GetUserOrdersAsync(int userId);
        Task<IEnumerable<OrderDto>> GetRestaurantOrdersAsync(int restaurantId, int ownerId);
        Task<bool> UpdateOrderStatusAsync(int orderId, UpdateStatusRequest request, int userId);
    }
}
