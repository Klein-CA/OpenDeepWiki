using KoalaWiki.Entities;

namespace KoalaWiki.Domains;

public class ChatShareMessageItem : Entity<string>
{
    /// <summary>
    /// 关联id
    /// </summary>
    public string ChatShareMessageId { get; set; } = string.Empty;

    /// <summary>
    /// 仓库id
    /// </summary>
    public string WarehouseId { get; set; } = string.Empty;

    /// <summary>
    /// 问题内容
    /// </summary>
    public string Question { get; set; } = string.Empty;

    /// <summary>
    /// 回答内容
    /// </summary>
    public string Answer { get; set; } = string.Empty;

    /// <summary>
    /// 思考内容
    /// </summary>
    public string Think { get; set; } = string.Empty;

    /// <summary>
    /// 请求token
    /// </summary>
    public int PromptToken { get; set; }

    /// <summary>
    /// 完成token
    /// </summary>
    public int CompletionToken { get; set; }

    /// <summary>
    /// 总耗时
    /// </summary>
    public int TotalTime { get; set; }
    
    public List<string> Files { get; set; } = new List<string>();
}