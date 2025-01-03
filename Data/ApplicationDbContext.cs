using Microsoft.EntityFrameworkCore;
using RealTimeRequestLogger.Models;

namespace RealTimeRequestLogger.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // URL响应数据表
    public DbSet<UrlResponse> UrlResponses { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 配置UrlResponse实体
        modelBuilder.Entity<UrlResponse>(entity =>
        {
            // 设置Path为必填且最大长度为500
            entity.Property(e => e.Path)
                .IsRequired()
                .HasMaxLength(500);

            // 设置JsonResponse为必填
            entity.Property(e => e.JsonResponse)
                .IsRequired();

            // 创建Path的索引以加快查询速度
            entity.HasIndex(e => e.Path);
        });
    }
}
