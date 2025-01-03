namespace RealTimeRequestLogger.Models;

public class UrlResponse
{
    // 主键ID
    public int Id { get; set; }
    
    // URL路径
    public string Path { get; set; } = string.Empty;
    
    // 返回的JSON内容
    public string JsonResponse { get; set; } = string.Empty;
    
    // 创建时间
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // 最后更新时间
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
