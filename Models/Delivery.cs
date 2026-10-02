using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FoodDelivery.Enums;

namespace FoodDelivery.Models
{
    public class Delivery
    {
        [Key]
        public int Id { get; set; }
        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order? Order { get; set; }

        public int DriverId { get; set; }
        [ForeignKey("DriverId")]
        public User? Driver { get; set; }

        public string DeliveryAddress { get; set; } = string.Empty;
        public string CurrentLocation { get; set; } = string.Empty;
        public DeliveryStatus Status { get; set; } = DeliveryStatus.Assigned;
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PickedUpAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
    }
}
