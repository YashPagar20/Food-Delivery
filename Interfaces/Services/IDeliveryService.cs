using FoodDelivery.DTOs;

namespace FoodDelivery.Interfaces.Services
{
    public interface IDeliveryService
    {
        Task<DeliveryDto> assignDriverAsync(AssignDriverRequest request);
        Task<DeliveryDto?> getDeliveryByOrderIdAsync(int orderId, int userId);
        Task<IEnumerable<DeliveryDto>> getDriverDeliveriesAsync(int driverId);
        Task<bool> updateDeliveryStatusAsync(int deliveryId, UpdateDeliveryStatusRequest request, int driverId);
        Task<bool> updateDriverLocationAsync(int deliveryId, UpdateLocationRequest request, int driverId);
    }
}
