using KoalaWiki.Core.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace KoalaWiki.Provider.Sqlite;

/// <summary>
/// SqliteContext 类表示 SQLite 数据库上下文，继承自 KoalaWikiContext。
/// 该类用于配置和管理 SQLite 数据库的连接和操作。
/// </summary>
public class SqliteContext(DbContextOptions<SqliteContext> options)
    : KoalaWikiContext<SqliteContext>(options)
{
    /// <summary>
    /// 配置数据库上下文的选项。
    /// </summary>
    /// <param name="optionsBuilder">用于配置数据库上下文的选项构建器</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ConfigureWarnings(warnings =>
            warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
    }
}