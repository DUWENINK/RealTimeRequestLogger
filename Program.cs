using Microsoft.AspNetCore.SignalR;
using RealTimeRequestLogger.Hubs;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSignalR();  // ��� SignalR ֧��

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

// ���� SignalR ·��
app.MapHub<RequestHub>("/requestHub");

app.Use(async (context, next) =>
{
    // ��ȡ���󷽷���·����ͷ��Ϣ�� Query String
    var method = context.Request.Method;
    var path = context.Request.Path;
    var queryString = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : "N/A"; // ���� Query String
    var headers = context.Request.Headers;
    string body = string.Empty;

    // ��ȡ�����壨������ڣ�
    if (context.Request.ContentLength > 0)
    {
        context.Request.EnableBuffering();
        using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, leaveOpen: true))
        {
            body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0; // ��������λ��
        }
    }

    // �����������ϸ��Ϣ
    var requestDetails = $"Method: {method}\nPath: {path}\nQuery String: {queryString}\nHeaders:\n";
    foreach (var header in headers)
    {
        requestDetails += $"{header.Key}: {header.Value}\n";
    }
    requestDetails += $"Body:\n{body}\n";

    // ���͵� SignalR
    var hubContext = context.RequestServices.GetRequiredService<IHubContext<RequestHub>>();
    await hubContext.Clients.All.SendAsync("ReceiveRequestDetails", requestDetails);

    await next();
});





app.Run();
