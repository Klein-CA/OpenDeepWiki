using System.ComponentModel.DataAnnotations.Schema;

namespace KoalaWiki.Domains;

/// <summary>
/// 聊天共享消息
/// </summary>
public class ChatShareMessage : Entity<string>
{
    /// <summary>
    /// 获取或设置关联的仓库ID，用于标识该聊天共享消息所属的仓库。
    /// </summary>
    public string WarehouseId { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置一个值，指示该消息是否启用了深度推理功能。
    /// </summary>
    public bool IsDeep { get; set; } = false;

    /// <summary>
    /// 获取或设置聊天共享消息的标题。
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置发起请求的客户端IP地址。
    /// </summary>
    public string Ip { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置用户提出的初始问题内容。
    /// </summary>
    public string Question { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置与当前聊天共享消息相关的消息项列表。
    /// 该属性不映射到数据库。
    /// </summary>
    [NotMapped]
    public List<ChatShareMessageItem> Items { get; set; } = [];
}

