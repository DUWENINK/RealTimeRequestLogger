using Microsoft.AspNetCore.SignalR;
using RealTimeRequestLogger.Hubs;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ��� SignalR �� Razor Pages ֧��
builder.Services.AddRazorPages();
builder.Services.AddSignalR();

var app = builder.Build();

// ���� HTTP ����ܵ�
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

// �������м��
app.Use(async (context, next) =>
{
    // ��ȡ����������Ϣ
    var method = context.Request.Method;
    var path = context.Request.Path;
    var queryString = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : "N/A";
    var customKey = context.Request.Query["customKey"]; // ��ȡ customKey
    var headers = context.Request.Headers;
    string body = string.Empty;

    // ��� customKey �Ƿ����
    if (string.IsNullOrEmpty(customKey))
    {
        Console.WriteLine("customKey δ�ṩ");
        await next(); // customKey ������ʱ������ִ����һ���м��
        return;
    }

    // ��ȡ�����壨����У�
    if (context.Request.ContentLength > 0)
    {
        context.Request.EnableBuffering(); // �����ȡ��������
        using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, leaveOpen: true))
        {
            body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0; // ������λ�ã�ȷ�������ܼ�����ȡ
        }
    }

    // ��������������Ϣ
    var requestDetails = $"Method: {method}\nPath: {path}\nQuery String: {queryString}\nHeaders:\n";
    foreach (var header in headers)
    {
        requestDetails += $"{header.Key}: {header.Value}\n";
    }
    requestDetails += $"Body:\n{body}\n";

    // ͨ�� SignalR ��ָ���� customKey ��㲥��Ϣ
    var hubContext = context.RequestServices.GetRequiredService<IHubContext<RequestHub>>();
    Console.WriteLine($"�� group {customKey} ������������");
    await hubContext.Clients.Group(customKey).SendAsync("ReceiveRequestDetails", requestDetails);

    await next(); // ����������һ���м��
});

app.Run();
