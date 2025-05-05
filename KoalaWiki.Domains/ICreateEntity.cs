namespace KoalaWiki.Entities;

public interface ICreateEntity
{
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }
}