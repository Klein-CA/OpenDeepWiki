using System.ComponentModel.DataAnnotations.Schema;
using KoalaWiki.Domains;

namespace KoalaWiki.Entities;

/// <summary>
/// DocumentCatalog 类表示文档目录实体，继承自 Entity 基类。
/// 该类用于管理文档目录的相关信息，包括名称、URL、描述、父级目录、排序等。
/// </summary>
public class DocumentCatalog : Entity<string>
{
    /// <summary>
    /// 获取或设置目录名称。
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置目录的 URL。
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置目录的描述信息。
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置目录的父级目录 ID。
    /// </summary>
    public string? ParentId { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置当前目录的排序值。
    /// </summary>
    public int Order { get; set; } = 0;

    /// <summary>
    /// 获取或设置关联的文档 ID。
    /// </summary>
    public string DucumentId { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置关联的仓库 ID。
    /// </summary>
    public string WarehouseId { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置目录的提示信息。此属性不会被映射到数据库。
    /// </summary>
    [NotMapped]
    public string Prompt { get; set; } = string.Empty;
}