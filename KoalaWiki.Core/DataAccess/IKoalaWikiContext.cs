using System.Threading;
using System.Threading.Tasks;
using KoalaWiki.Domains;
using KoalaWiki.Entities;
using KoalaWiki.Entities.DocumentFile;
using Microsoft.EntityFrameworkCore;

namespace KoalaWiki.Core.DataAccess;

/// <summary>
/// IKoalaWikiContext 接口定义了 KoalaWiki 应用程序的数据库上下文。
/// 该接口的主要职责是提供对数据库实体的访问以及数据库操作的相关方法。
/// </summary>
public interface IKoalaWikiContext
{
    /// <summary>
    /// 获取或设置仓库实体的数据库集合。
    /// </summary>
    DbSet<Warehouse> Warehouses { get; set; }

    /// <summary>
    /// 获取或设置文档目录实体的数据库集合。
    /// </summary>
    DbSet<DocumentCatalog> DocumentCatalogs { get; set; }

    /// <summary>
    /// 获取或设置文档实体的数据库集合。
    /// </summary>
    DbSet<Document> Documents { get; set; }

    /// <summary>
    /// 获取或设置文档文件项实体的数据库集合。
    /// </summary>
    DbSet<DocumentFileItem> DocumentFileItems { get; set; }

    /// <summary>
    /// 获取或设置文档文件项源实体的数据库集合。
    /// </summary>
    DbSet<DocumentFileItemSource> DocumentFileItemSources { get; set; }

    /// <summary>
    /// 获取或设置文档概览实体的数据库集合。
    /// </summary>
    DbSet<DocumentOverview> DocumentOverviews { get; set; }

    /// <summary>
    /// 获取或设置文档提交记录实体的数据库集合。
    /// </summary>
    DbSet<DocumentCommitRecord> DocumentCommitRecords { get; set; }

    /// <summary>
    /// 获取或设置聊天共享消息实体的数据库集合。
    /// </summary>
    DbSet<ChatShareMessage> ChatShareMessages { get; set; }

    /// <summary>
    /// 获取或设置聊天共享消息项实体的数据库集合。
    /// </summary>
    DbSet<ChatShareMessageItem> ChatShareMessageItems { get; set; }

    /// <summary>
    /// 异步保存所有对数据库的更改。
    /// </summary>
    /// <param name="cancellationToken">用于取消操作的取消令牌</param>
    /// <returns>表示异步操作的任务，返回受影响的行数</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());

    /// <summary>
    /// 异步运行数据库迁移。
    /// </summary>
    /// <returns>表示异步操作的任务</returns>
    Task RunMigrateAsync();
}