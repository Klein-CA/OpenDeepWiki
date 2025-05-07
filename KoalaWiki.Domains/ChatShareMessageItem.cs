namespace KoalaWiki.Domains;

/// <summary>
/// 聊天共享信息项
/// </summary>
public class ChatShareMessageItem : Entity<string>
{
    /// <summary>
    /// 获取或设置关联的聊天共享消息ID，用于标识所属的聊天共享消息。
    /// </summary>
    public string ChatShareMessageId { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置仓库ID，表示该消息项所属的仓库。
    /// </summary>
    public string WarehouseId { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置用户提出的问题内容。
    /// </summary>
    public string Question { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置AI生成的回答内容。
    /// </summary>
    public string Answer { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置回答生成过程中的思考内容，通常用于调试或展示推理过程。
    /// </summary>
    public string Think { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置请求中使用的提示词（Prompt）所消耗的token数量。
    /// </summary>
    public int PromptToken { get; set; }

    /// <summary>
    /// 获取或设置生成回答过程中所消耗的完成token数量。
    /// </summary>
    public int CompletionToken { get; set; }

    /// <summary>
    /// 获取或设置处理该消息项所耗费的总时间（单位为毫秒）。
    /// </summary>
    public int TotalTime { get; set; }

    /// <summary>
    /// 获取或设置与当前消息项关联的文件路径或标识符列表。
    /// </summary>
    public List<string> Files { get; set; } = [];
}