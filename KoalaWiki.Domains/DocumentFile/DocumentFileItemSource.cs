namespace KoalaWiki.Entities.DocumentFile;

/// <summary>
/// DocumentFileItemSource 类表示文档文件项的源文件实体，继承自 Entity 基类。
/// 该类用于管理文档文件项的源文件信息，包括源文件地址、名称以及关联的文档文件项。
/// </summary>
public class DocumentFileItemSource : Entity<string>
{
    /// <summary>
    /// 获取或设置关联的文档文件项 ID。
    /// </summary>
    public string DocumentFileItemId { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置源文件的地址。
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置源文件的名称。
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 获取或设置关联的文档文件项实体。
    /// </summary>
    public DocumentFileItem DocumentFileItem { get; set; } = null!;
}