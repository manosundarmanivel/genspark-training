using Microsoft.AspNetCore.SignalR;

namespace NotifyAPI.Hubs
{
    public class NotificationHub : Hub
    {
        
         public override Task OnConnectedAsync()
    {
        Console.WriteLine($"Client connected: {Context.ConnectionId}");
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"Client disconnected: {Context.ConnectionId}");
        return base.OnDisconnectedAsync(exception);
    }
     }
}