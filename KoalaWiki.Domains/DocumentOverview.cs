using KoalaWiki.Domains;

namespace KoalaWiki.Entities;

/// <summary>
/// DocumentOverview 类表示文档概览实体，继承自 Entity 基类。
/// 该类用于管理文档的概览信息，包括绑定的文档 ID、内容以及标题。
/// </summary>
public class DocumentOverview : Entity<string>
{
    /// <summary>
    /// 获取或设置绑定的文档 ID。
    /// </summary>
    public string DocumentId { get; set; }

    /// <summary>
    /// 获取或设置文档的内容。
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// 获取或设置文档的标题。
    /// </summary>
    public string Title { get; set; }
}