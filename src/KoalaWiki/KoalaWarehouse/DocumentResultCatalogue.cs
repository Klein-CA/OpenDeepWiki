using System.ComponentModel;
using Newtonsoft.Json;

namespace KoalaWiki.KoalaWarehouse;

/// <summary>
/// 文档结果目录类，用于表示文档目录的结构。
/// </summary>
public class DocumentResultCatalogue
{
    /// <summary>
    /// 获取或设置文档目录项的列表。
    /// </summary>
    [JsonProperty("items")]
    public List<DocumentResultCatalogueItem> Items { get; set; } = [];
}

/// <summary>
/// 文档结果目录项类，表示文档目录中的一个项。
/// </summary>
public class DocumentResultCatalogueItem
{
    /// <summary>
    /// 获取或设置文档目录项的名称。
    /// </summary>
    [JsonProperty("name")]
    [Description("A concise description that is suitable as a description for the document directory")]
    public string? Name { get; set; }

    /// <summary>
    /// 获取或设置文档目录项的标题，通常用于URL路径。
    /// </summary>
    [JsonProperty("title")]
    [Description("Lowercase, hyphenated slugs for URL paths (e.g., \"api-reference\")")]
    public string? Title { get; set; }

    /// <summary>
    /// 获取或设置文档目录项的简短描述。
    /// </summary>
    [JsonProperty("prompt")]
    [Description("A short description of the document directory")]
    public string? Prompt { get; set; }

    /// <summary>
    /// 获取或设置文档目录项的子项列表。
    /// </summary>
    [JsonProperty("children")]
    public List<DocumentResultCatalogueChildItem> Children { get; set; } = [];
}

/// <summary>
/// 文档结果目录子项类，表示文档目录项的子项。
/// </summary>
public class DocumentResultCatalogueChildItem
{
    /// <summary>
    /// 获取或设置文档目录子项的名称。
    /// </summary>
    [JsonProperty("name")]
    [Description("A concise description that is suitable as a description for the document directory")]
    public string? Name { get; set; }

    /// <summary>
    /// 获取或设置文档目录子项的标题，通常用于URL路径。
    /// </summary>
    [JsonProperty("title")]
    [Description("Lowercase, hyphenated slugs for URL paths (e.g., \"api-reference\")")]
    public string? Title { get; set; }

    /// <summary>
    /// 获取或设置文档目录子项的简短描述。
    /// </summary>
    [JsonProperty("prompt")]
    [Description("A short description of the document directory")]
    public string? Prompt { get; set; }

    /// <summary>
    /// 获取或设置文档目录子项的子项列表。
    /// </summary>
    [JsonProperty("children")]
    public List<DocumentResultCatalogueChildItem1> Children { get; set; } = [];
}

/// <summary>
/// 文档结果目录子项1类，表示文档目录子项的子项。
/// </summary>
public class DocumentResultCatalogueChildItem1
{
    /// <summary>
    /// 获取或设置文档目录子项1的名称。
    /// </summary>
    [JsonProperty("name")]
    [Description("A concise description that is suitable as a description for the document directory")]
    public string? Name { get; set; }

    /// <summary>
    /// 获取或设置文档目录子项1的标题，通常用于URL路径。
    /// </summary>
    [JsonProperty("title")]
    [Description("Lowercase, hyphenated slugs for URL paths (e.g., \"api-reference\")")]
    public string? Title { get; set; }

    /// <summary>
    /// 获取或设置文档目录子项1的简短描述。
    /// </summary>
    [JsonProperty("prompt")]
    [Description("A short description of the document directory")]
    public string? Prompt { get; set; }
}