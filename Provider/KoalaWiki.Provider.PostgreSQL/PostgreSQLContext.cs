using KoalaWiki.Core.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace KoalaWiki.Provider.PostgreSQL;

/// <summary>
/// PostgreSQLContext 类，继承自 KoalaWikiContext，用于处理与 PostgreSQL 数据库的上下文操作。
/// </summary>
/// <param name="options">DbContextOptions<PostgreSQLContext> 类型的参数，用于配置上下文选项。</param>
public class PostgreSQLContext(DbContextOptions<PostgreSQLContext> options)
    : KoalaWikiContext<PostgreSQLContext>(options)
{
    /// <summary>
    /// 配置 DbContextOptionsBuilder，用于在上下文配置时忽略特定的警告。
    /// </summary>
    /// <param name="optionsBuilder">DbContextOptionsBuilder 实例，用于配置上下文选项。</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
    }
}