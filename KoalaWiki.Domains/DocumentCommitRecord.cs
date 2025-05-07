using KoalaWiki.Domains;

namespace KoalaWiki.Entities;

/// <summary>
/// DocumentCommitRecord 类表示文档提交记录实体，继承自 Entity 基类。
/// 该类用于管理文档的提交记录信息，包括仓库 ID、提交 ID、提交消息、作者以及最后更新时间。
/// </summary>
public class DocumentCommitRecord : Entity<string>
{
    /// <summary>
    /// 获取或设置关联的仓库 ID。
    /// </summary>
    public string WarehouseId { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置提交的唯一标识符。
    /// </summary>
    public string CommitId { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置提交的消息内容。
    /// </summary>
    public string CommitMessage { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置提交的作者。
    /// </summary>
    public string Author { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置最后更新时间。
    /// </summary>
    public DateTime LastUpdate { get; set; } = DateTime.Now;
}