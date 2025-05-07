using KoalaWiki.Entities;

namespace KoalaWiki.Domains;

/// <summary>
/// Entity 类是一个泛型实体基类，用于表示具有唯一标识符和创建时间戳的实体。
/// 该类实现了 IEntity 和 ICreateEntity 接口，提供实体的基本属性和行为。
/// </summary>
/// <typeparam name="TKey">实体唯一标识符的类型</typeparam>
public class Entity<TKey> : IEntity<TKey>, ICreateEntity
{
    /// <summary>
    /// 获取或设置实体的唯一标识符。
    /// </summary>
    public required TKey Id { get; set; }

    /// <summary>
    /// 获取或设置实体的创建时间戳。
    /// </summary>
    public DateTime CreatedAt { get; set; }
}