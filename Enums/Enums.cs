namespace FoodDelivery.Enums
{
    public enum UserRole
    {
        Admin,
        Customer,
        RestaurantOwner,
        DeliveryPartner
    }

    public enum OrderStatus
    {
        Pending,
        Preparing,
        OutForDelivery,
        Delivered,
        Cancelled
    }

    public enum PaymentMethod
    {
        CreditCard,
        DebitCard,
        UPI,
        NetBanking,
        CashOnDelivery
    }

    public enum PaymentStatus
    {
        Pending,
        Completed,
        Failed,
        Refunded
    }

    public enum DeliveryStatus
    {
        Assigned,
        PickedUp,
        InTransit,
        Delivered,
        Failed
    }

    public enum NotificationType
    {
        Email,
        SMS,
        InApp
    }

    public enum NotificationStatus
    {
        Pending,
        Sent,
        Failed
    }

    public enum DiscountType
    {
        Percentage,
        FlatAmount
    }
}
