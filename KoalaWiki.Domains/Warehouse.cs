namespace KoalaWiki.Entities;

public class Warehouse : Entity<string>
{
    /// <summary>
    /// 组织名称
    /// </summary>
    /// <returns></returns>
    public string OrganizationName { get; set; } = string.Empty;

    /// <summary>
    /// 仓库名称
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// 仓库描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 仓库地址
    /// </summary>
    /// <returns></returns>
    public required string Address { get; set; }

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

    /// <summary>
    /// 仓库类型
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// 仓库分支
    /// </summary>
    public string? Branch { get; set; }

    /// <summary>
    /// 仓库状态
    /// </summary>
    public WarehouseStatus Status { get; set; }

    /// <summary>
    /// 错误信息
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// 构建提示词
    /// </summary>
    public string? Prompt { get; set; }

    /// <summary>
    /// 仓库版本
    /// </summary>
    public string? Version { get; set; }

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
}