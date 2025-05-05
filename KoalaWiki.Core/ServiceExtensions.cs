using System;
using KoalaWiki.Core.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace KoalaWiki.Core;

/// <summary>
/// ServiceExtensions 类提供了一些用于扩展服务集合的静态方法。
/// 该类主要用于简化服务注册和配置的过程。
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// 添加数据库上下文服务到服务集合中。
    /// </summary>
    /// <typeparam name="TContext">数据库上下文类型，必须继承自 KoalaWikiContext</typeparam>
    /// <param name="services">服务集合</param>
    /// <param name="configureContext">用于配置数据库上下文的委托</param>
    /// <returns>配置后的服务集合</returns>
    public static IServiceCollection AddDataAccess<TContext>(this IServiceCollection services,
        Action<IServiceProvider, DbContextOptionsBuilder> configureContext) where TContext : KoalaWikiContext<TContext>
    {
        services.AddDbContext<IKoalaWikiContext, TContext>(configureContext);
        return services;
    }
}