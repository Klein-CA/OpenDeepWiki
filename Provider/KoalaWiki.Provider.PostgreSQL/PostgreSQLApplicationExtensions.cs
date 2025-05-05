using KoalaWiki.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KoalaWiki.Provider.PostgreSQL;

/// <summary>
/// PostgreSQL数据库上下文扩展类，用于在ASP.NET Core应用程序中配置和添加PostgreSQL数据库上下文服务。
/// </summary>
public static class PostgreSQLApplicationExtensions
{
    /// <summary>
    /// 将PostgreSQL数据库上下文服务添加到服务集合中。
    /// </summary>
    /// <param name="services">服务集合，用于注册服务。</param>
    /// <param name="configuration">配置对象，用于获取数据库连接字符串。</param>
    /// <returns>返回配置后的服务集合，以便支持链式调用。</returns>
    public static IServiceCollection AddPostgreSQLDbContext(this IServiceCollection services,
        IConfiguration configuration)
    {
        // 添加数据访问服务，配置PostgreSQL上下文
        services.AddDataAccess<PostgreSQLContext>((provider, builder) =>
        {
            // 使用Npgsql作为PostgreSQL的数据库提供程序，并从配置中获取默认连接字符串
            builder.UseNpgsql(configuration.GetConnectionString("Default"));

            // 配置日志工厂，不将SQL日志输出到控制台
            builder.UseLoggerFactory(LoggerFactory.Create(_ => { }));
        });

        // 返回服务集合，支持链式调用
        return services;
    }
}