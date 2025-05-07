using System.ComponentModel.DataAnnotations.Schema;

namespace KoalaWiki.Domains;

public class ChatShareMessage : Entity<string>
{
    /// <summary>
    /// 关联id
    /// </summary>
    public string WarehouseId { get; set; } = string.Empty;
    
    /// <summary>
    /// 是否深度推理
    /// </summary>
    /// <returns></returns>
    public bool IsDeep { get; set; } = false;
    
    /// <summary>
    /// 标题
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// 请求ip
    /// </summary>
    public string Ip { get; set; } = string.Empty;
    
    /// <summary>
    /// 最开始的问题
    /// </summary>
    public string Question { get; set; } = string.Empty;
    
    [NotMapped]
    public List<ChatShareMessageItem> Items { get; set; } = new();
}

