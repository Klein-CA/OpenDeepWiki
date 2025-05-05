using KoalaWiki.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KoalaWiki.Provider.Sqlite;

/// <summary>
/// SqliteApplicationExtensions 类提供了一些用于扩展服务集合的静态方法。
/// 该类主要用于简化 SQLite 数据库上下文的注册和配置过程。
/// </summary>
public static class SqliteApplicationExtensions
{
    /// <summary>
    /// 添加 SQLite 数据库上下文服务到服务集合中。
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configuration">配置对象，用于获取连接字符串</param>
    /// <returns>配置后的服务集合</returns>
    public static IServiceCollection AddSqliteDbContext(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDataAccess<SqliteContext>((provider, builder) =>
        {
            builder.UseSqlite(configuration.GetConnectionString("Default"));

            // sql日志不输出控制台
            builder.UseLoggerFactory(LoggerFactory.Create(_ => { }));
        });

        return services;
    }
}