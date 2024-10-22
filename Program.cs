using Microsoft.AspNetCore.SignalR;
using RealTimeRequestLogger.Hubs;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 添加 SignalR 和 Razor Pages 支持
builder.Services.AddRazorPages();
builder.Services.AddSignalR();

var app = builder.Build();

// 配置 HTTP 请求管道
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

// 配置 SignalR 路由
app.MapHub<RequestHub>("/requestHub");

// 请求处理中间件
app.Use(async (context, next) =>
{
    // 获取请求的相关信息
    var method = context.Request.Method;
    var path = context.Request.Path;
    var queryString = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : "N/A";
    var customKey = context.Request.Query["customKey"]; // 获取 customKey
    var headers = context.Request.Headers;
    string body = string.Empty;

    // 检查 customKey 是否存在
    if (string.IsNullOrEmpty(customKey))
    {
        Console.WriteLine("customKey 未提供");
        await next(); // customKey 不存在时，继续执行下一个中间件
        return;
    }

    // 读取请求体（如果有）
    if (context.Request.ContentLength > 0)
    {
        context.Request.EnableBuffering(); // 允许读取请求体多次
        using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, leaveOpen: true))
        {
            body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0; // 重置流位置，确保后续能继续读取
        }
    }

    // 构建请求详情信息
    var requestDetails = $"Method: {method}\nPath: {path}\nQuery String: {queryString}\nHeaders:\n";
    foreach (var header in headers)
    {
        requestDetails += $"{header.Key}: {header.Value}\n";
    }
    requestDetails += $"Body:\n{body}\n";

    // 通过 SignalR 向指定的 customKey 组广播消息
    var hubContext = context.RequestServices.GetRequiredService<IHubContext<RequestHub>>();
    Console.WriteLine($"向 group {customKey} 发送请求详情");
    await hubContext.Clients.Group(customKey).SendAsync("ReceiveRequestDetails", requestDetails);

    await next(); // 继续处理下一个中间件
});

app.Run();
