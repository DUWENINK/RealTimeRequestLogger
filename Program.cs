using Microsoft.AspNetCore.SignalR;
using RealTimeRequestLogger.Hubs;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add SignalR and Razor Pages support
builder.Services.AddRazorPages();
builder.Services.AddSignalR();

var app = builder.Build();

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
    var responseMode = context.Request.Query["responseMode"]; // New parameter to control response behavior
    var headers = context.Request.Headers;
    string body = string.Empty;

    // Check if customKey exists
    if (string.IsNullOrEmpty(customKey))
    {
        Console.WriteLine("customKey not provided");
        await next();
        return;
    }

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

    // Broadcast via SignalR
    var hubContext = context.RequestServices.GetRequiredService<IHubContext<RequestHub>>();
    Console.WriteLine($"Broadcasting to group {customKey}");
    await hubContext.Clients.Group(customKey).SendAsync("ReceiveRequestDetails", requestDetails);

    // Handle custom response if responseMode is specified
    if (!string.IsNullOrEmpty(responseMode))
    {
        object responseData;
        
        try
        {
            // If body is provided and valid JSON, use it as response
            if (!string.IsNullOrEmpty(body))
            {
                responseData = JsonSerializer.Deserialize<object>(body);
            }
            else
            {
                // Default response if no body provided
                responseData = new
                {
                    message = "Custom response",
                    timestamp = DateTime.UtcNow,
                    path = path.ToString(),
                    method = method
                };
            }

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(responseData);
            return;
        }
        catch (JsonException)
        {
            // If JSON parsing fails, return error
            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Invalid JSON in request body",
                timestamp = DateTime.UtcNow
            });
            return;
        }
    }

    await next();
});

app.Run();
