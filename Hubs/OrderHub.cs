using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace FoodDelivery.Hubs
{
    public class OrderHub : Hub
    {
        // Clients will join a group named after their OrderId or UserId
        public async Task JoinOrderGroup(int orderId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Order_{orderId}");
        }

        public async Task LeaveOrderGroup(int orderId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Order_{orderId}");
        }
    }
}
