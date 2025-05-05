namespace KoalaWiki.Dto;

/// <summary>
/// 分页数据传输对象，用于封装分页查询结果。
/// </summary>
/// <typeparam name="T">数据类型。</typeparam>
/// <remarks>
/// 初始化 <see cref="PageDto{T}"/> 类的新实例。
/// </remarks>
/// <param name="total">总记录数。</param>
/// <param name="items">当前页的数据项列表。</param>
public class PageDto<T>(int total, IList<T> items)
{
    /// <summary>
    /// 获取或设置总记录数。
    /// </summary>
    public int Total { get; set; } = total;

    /// <summary>
    /// 获取或设置当前页的数据项列表。
    /// </summary>
    public IList<T> Items { get; set; } = items;
}