namespace FoodDelivery.Enums
{
    public enum UserRole
    {
        Admin,
        Customer,
        RestaurantOwner
    }

    public enum OrderStatus
    {
        Pending,
        Preparing,
        OutForDelivery,
        Delivered,
        Cancelled
    }
}
