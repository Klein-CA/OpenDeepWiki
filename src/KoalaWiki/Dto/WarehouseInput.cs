namespace KoalaWiki.Dto;

/// <summary>
/// 仓库参数
/// </summary>
public class WarehouseInput
{
    /// <summary>
    /// 仓库地址
    /// </summary>
    /// <returns></returns>
    public required string Address { get; set; }

    /// <summary>
    /// 使用模型
    /// </summary>
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// OpenAI 密钥
    /// </summary>
    public string OpenAIKey { get; set; } = string.Empty;

    /// <summary>
    /// OpenAI 端点
    /// </summary>
    public string OpenAIEndpoint { get; set; } = string.Empty;

    /// <summary>
    /// 私有化git账号
    /// </summary>
    public string? GitUserName { get; set; }

    /// <summary>
    /// 私有化git密码
    /// </summary>
    public string? GitPassword { get; set; }

    /// <summary>
    ///  私有化git邮箱
    /// </summary>
    public string? Email { get; set; }
}