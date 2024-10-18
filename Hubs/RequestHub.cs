using Microsoft.AspNetCore.SignalR;

namespace RealTimeRequestLogger.Hubs
{
    public class RequestHub : Hub
    {
        public async Task SendRequestDetails(string message)
        {
            await Clients.All.SendAsync("ReceiveRequestDetails", message);
        }
    }
}
