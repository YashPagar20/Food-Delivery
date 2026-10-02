using System.ComponentModel.DataAnnotations;
using FoodDelivery.Enums;

namespace FoodDelivery.DTOs
{
    public class AssignDriverRequest
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        public int DriverId { get; set; }

        [Required]
        public string DeliveryAddress { get; set; } = string.Empty;
    }

    public class UpdateDeliveryStatusRequest
    {
        [Required]
        public DeliveryStatus Status { get; set; }
    }

    public class UpdateLocationRequest
    {
        [Required]
        public string Location { get; set; } = string.Empty;
    }

    public class DeliveryDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int DriverId { get; set; }
        public string DriverName { get; set; } = string.Empty;
        public string DeliveryAddress { get; set; } = string.Empty;
        public string CurrentLocation { get; set; } = string.Empty;
        public DeliveryStatus Status { get; set; }
        public DateTime AssignedAt { get; set; }
        public DateTime? PickedUpAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
    }
}
