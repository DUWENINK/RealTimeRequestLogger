using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RealTimeRequestLogger.Data;
using RealTimeRequestLogger.Hubs;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// 添加数据库支持
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=app.db"));

// 添加SignalR和Razor Pages支持
builder.Services.AddRazorPages();
builder.Services.AddSignalR();

var app = builder.Build();

// 确保数据库已创建
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
}

// Configure HTTP request pipeline
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

// Configure SignalR route
app.MapHub<RequestHub>("/requestHub");

// Request middleware
app.Use(async (context, next) =>
{
    // Get request information
    var method = context.Request.Method;
    var path = context.Request.Path;
    var queryString = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : "N/A";
    var customKey = context.Request.Query["customKey"];
    var headers = context.Request.Headers;
    string body = string.Empty;

    // Read request body (if any)
    if (context.Request.ContentLength > 0)
    {
        context.Request.EnableBuffering();
        using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, leaveOpen: true))
        {
            body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;
        }
    }

    // Prepare request details for SignalR
    var requestDetails = $"Method: {method}\nPath: {path}\nQuery String: {queryString}\nHeaders:\n";
    foreach (var header in headers)
    {
        requestDetails += $"{header.Key}: {header.Value}\n";
    }
    requestDetails += $"Body:\n{body}\n";

    // Broadcast via SignalR if customKey exists
    if (!string.IsNullOrEmpty(customKey))
    {
        var hubContext = context.RequestServices.GetRequiredService<IHubContext<RequestHub>>();
        Console.WriteLine($"Broadcasting to group {customKey}");
        await hubContext.Clients.Group(customKey).SendAsync("ReceiveRequestDetails", requestDetails);
    }

    // Handle custom response parameters
    var customResponse = context.Request.Query["__response"].ToString();
    var customStatus = context.Request.Query["__status"].ToString();
    var customDelay = context.Request.Query["__delay"].ToString();

    // 检查数据库中是否有匹配的URL响应
    var dbContext = context.RequestServices.GetRequiredService<ApplicationDbContext>();
    var urlResponse = await dbContext.UrlResponses.FirstOrDefaultAsync(u => u.Path == path.ToString());
    
    if (urlResponse != null)
    {
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(urlResponse.JsonResponse);
        return;
    }
    
    // 处理自定义响应参数
    if (!string.IsNullOrEmpty(customResponse) || !string.IsNullOrEmpty(customStatus) || !string.IsNullOrEmpty(customDelay))
    {
        // Handle custom delay
        if (!string.IsNullOrEmpty(customDelay) && int.TryParse(customDelay, out int delay))
        {
            await Task.Delay(delay);
        }

        // Handle custom status code
        if (!string.IsNullOrEmpty(customStatus) && int.TryParse(customStatus, out int statusCode))
        {
            context.Response.StatusCode = statusCode;
        }

        // Handle custom response body
        if (!string.IsNullOrEmpty(customResponse))
        {
            try
            {
                // Try to parse as JSON first
                using (JsonDocument.Parse(customResponse))
                {
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(customResponse);
                }
            }
            catch (JsonException)
            {
                // If not valid JSON, return as plain text
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync(customResponse);
            }
            return;
        }
        else if (context.Response.StatusCode != 200)
        {
            // If only status code was set, return a default response
            await context.Response.WriteAsJsonAsync(new
            {
                statusCode = context.Response.StatusCode,
                message = "Custom status response",
                timestamp = DateTime.UtcNow
            });
            return;
        }
    }

    await next();
});

app.Run();
