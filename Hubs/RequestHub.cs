using Microsoft.AspNetCore.SignalR;

namespace RealTimeRequestLogger.Hubs
{
    public class RequestHub : Hub
    {
        // 客户端注册时调用此方法，并根据 customKey 加入组
        public async Task RegisterUser(string customKey)
        {
            // 将 customKey 存储在 Context.Items 中以便断开时使用
            Context.Items["customKey"] = customKey;

            // 将连接的客户端加入到以 customKey 为标识的组
            await Groups.AddToGroupAsync(Context.ConnectionId, customKey);
            Console.WriteLine($"User with ConnectionId {Context.ConnectionId} joined group {customKey}");
        }

        // 客户端注册到全局监听组
        public async Task RegisterGlobalMonitor()
        {
            // 将连接的客户端加入到全局监听组
            Context.Items["isGlobalMonitor"] = "true";
            await Groups.AddToGroupAsync(Context.ConnectionId, "__global_monitor__");
            Console.WriteLine($"User with ConnectionId {Context.ConnectionId} joined global monitor group");
        }

        public override async Task OnDisconnectedAsync(System.Exception exception)
        {
            // 从 Context.Items 获取 customKey
            var customKey = Context.Items["customKey"] as string;
            var isGlobalMonitor = Context.Items["isGlobalMonitor"] as string;

            if (!string.IsNullOrEmpty(customKey))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, customKey);
                Console.WriteLine($"User with ConnectionId {Context.ConnectionId} removed from group {customKey}");
            }

            if (isGlobalMonitor == "true")
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, "__global_monitor__");
                Console.WriteLine($"User with ConnectionId {Context.ConnectionId} removed from global monitor group");
            }

            await base.OnDisconnectedAsync(exception);
        }
    }

}
