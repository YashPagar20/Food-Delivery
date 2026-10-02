using FoodDelivery.DTOs;

namespace FoodDelivery.Interfaces.Services
{
    public interface IOrderService
    {
        Task<OrderDto> placeOrderAsync(PlaceOrderRequest request, int customerId);
        Task<OrderDto?> getOrderByIdAsync(int id);
        Task<IEnumerable<OrderDto>> getUserOrdersAsync(int userId);
        Task<IEnumerable<OrderDto>> getRestaurantOrdersAsync(int restaurantId, int ownerId);
        Task<bool> updateOrderStatusAsync(int orderId, UpdateStatusRequest request, int userId);
    }
}
