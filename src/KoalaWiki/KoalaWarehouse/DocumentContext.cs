namespace KoalaWiki.KoalaWarehouse;

/// <summary>
/// 文档上下文类，用于管理文档存储的上下文信息。
/// </summary>
public class DocumentContext
{
    /// <summary>
    /// 私有静态只读字段，使用AsyncLocal来存储文档持有者对象，确保异步操作中的线程安全。
    /// </summary>
    private static readonly AsyncLocal<DocumentHolder> _documentHolder = new();

    /// <summary>
    /// 获取或设置当前的文档存储对象。
    /// </summary>
    /// <value>返回当前上下文中的文档存储对象，如果未设置则返回null。</value>
    public static DocumentStore? DocumentStore
    {
        /// <summary>
        /// 获取当前上下文中的文档存储对象。
        /// </summary>
        /// <returns>返回当前上下文中的文档存储对象，如果未设置则返回null。</returns>
        get => _documentHolder.Value?.DocumentStore;

        /// <summary>
        /// 设置当前上下文中的文档存储对象。
        /// </summary>
        /// <param name="value">要设置的文档存储对象。</param>
        set
        {
            _documentHolder.Value ??= new DocumentHolder();
            _documentHolder.Value.DocumentStore = value;
        }
    }

    /// <summary>
    /// 私有内部类，用于持有文档存储对象。
    /// </summary>
    private class DocumentHolder
    {
        /// <summary>
        /// 获取或设置文档存储对象。
        /// </summary>
        /// <value>返回当前持有者中的文档存储对象。</value>
        public DocumentStore DocumentStore { get; set; } = new();
    }
}

/// <summary>
/// 文档存储类，用于存储和管理文件列表。
/// </summary>
public class DocumentStore
{
    /// <summary>
    /// 获取或设置文件列表。
    /// </summary>
    /// <value>返回当前文档存储中的文件列表。</value>
    public List<string> Files { get; set; } = [];
}