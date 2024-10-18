using Microsoft.AspNetCore.SignalR;
using RealTimeRequestLogger.Hubs;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSignalR();  // 添加 SignalR 支持

var app = builder.Build();

// Configure the HTTP request pipeline.
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

app.Use(async (context, next) =>
{
    // 获取请求方法、路径、头信息和 Query String
    var method = context.Request.Method;
    var path = context.Request.Path;
    var queryString = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : "N/A"; // 捕获 Query String
    var headers = context.Request.Headers;
    string body = string.Empty;

    // 读取请求体（如果存在）
    if (context.Request.ContentLength > 0)
    {
        context.Request.EnableBuffering();
        using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, leaveOpen: true))
        {
            body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0; // 重置流的位置
        }
    }

    // 构建请求的详细信息
    var requestDetails = $"Method: {method}\nPath: {path}\nQuery String: {queryString}\nHeaders:\n";
    foreach (var header in headers)
    {
        requestDetails += $"{header.Key}: {header.Value}\n";
    }
    requestDetails += $"Body:\n{body}\n";

    // 发送到 SignalR
    var hubContext = context.RequestServices.GetRequiredService<IHubContext<RequestHub>>();
    await hubContext.Clients.All.SendAsync("ReceiveRequestDetails", requestDetails);

    await next();
});





app.Run();
