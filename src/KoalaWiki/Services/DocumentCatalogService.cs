using FastService;
using KoalaWiki.Core.DataAccess;
using KoalaWiki.Entities;
using LibGit2Sharp;
using Microsoft.EntityFrameworkCore;

namespace KoalaWiki.Services;

/// <summary>
/// DocumentCatalogService 类用于处理与文档目录相关的业务逻辑。
/// 该类提供了获取文档目录列表、根据目录 ID 获取文件内容以及构建文档目录树形结构的功能。
/// </summary>
public class DocumentCatalogService(IKoalaWikiContext dbAccess) : FastApi
{
    /// <summary>
    /// 获取指定仓库的文档目录列表。
    /// </summary>
    /// <param name="organizationName">组织名称</param>
    /// <param name="name">仓库名称</param>
    /// <returns>包含文档目录列表、最后更新时间、仓库描述等信息的匿名对象</returns>
    /// <exception cref="NotFoundException">如果仓库不存在，抛出此异常</exception>
    public async Task<object> GetDocumentCatalogsAsync(string organizationName, string name)
    {
        var warehouse = await dbAccess.Warehouses
            .AsNoTracking()
            .Where(x => x.Name == name && x.OrganizationName == organizationName)
            .FirstOrDefaultAsync() ?? throw new NotFoundException("仓库不存在");

        var document = await dbAccess.Documents
            .AsNoTracking()
            .Where(x => x.WarehouseId == warehouse.Id)
            .FirstOrDefaultAsync();

        var documentCatalogs = await dbAccess.DocumentCatalogs
            .Where(x => x.WarehouseId == warehouse.Id)
            .ToListAsync();

        string lastUpdate;

        // 如果最近更新时间是今天那么只需要显示小时
        if (document?.LastUpdate != null)
        {
            var time = DateTime.Now - document.LastUpdate;
            lastUpdate = time.Days == 0 ? $"{time.Hours}小时前" : $"{time.Days}天前";

            // 如果超过7天，显示日期
            if (time.Days > 7)
            {
                lastUpdate = document.LastUpdate.ToString("yyyy-MM-dd");
            }
        }
        else
        {
            lastUpdate = "刚刚";
        }

        return new
        {
            items = BuildDocumentTree(documentCatalogs),
            lastUpdate,
            document?.Description,
            git = warehouse.Address,
            document?.LikeCount,
            document?.Status,
            document?.CommentCount,
        };
    }

    /// <summary>
    /// 根据目录 ID 获取文件内容。
    /// </summary>
    /// <param name="httpContext">HTTP 上下文</param>
    /// <param name="owner">仓库所有者</param>
    /// <param name="name">仓库名称</param>
    /// <param name="path">目录路径</param>
    /// <returns>包含文件内容、标题、引用文件等信息的 JSON 响应</returns>
    /// <exception cref="NotFoundException">如果仓库或文件不存在，抛出此异常</exception>
    public async Task GetDocumentByIdAsync(HttpContext httpContext, string owner, string name, string path)
    {
        // 先根据仓库名称和组织名称找到仓库
        var query = await dbAccess.Warehouses
            .AsNoTracking()
            .Where(x => x.Name == name && x.OrganizationName == owner)
            .FirstOrDefaultAsync() ?? throw new NotFoundException("仓库不存在");

        // 找到catalog
        var id = await dbAccess.DocumentCatalogs
            .AsNoTracking()
            .Where(x => x.WarehouseId == query.Id && x.Url == path)
            .Select(x => x.Id)
            .FirstOrDefaultAsync();

        var item = await dbAccess.DocumentFileItems
            .AsNoTracking()
            .Where(x => x.DocumentCatalogId == id)
            .FirstOrDefaultAsync() ?? throw new NotFoundException("文件不存在");

        // 找到所有引用文件
        var fileSource = await dbAccess.DocumentFileItemSources.Where(x => x.DocumentFileItemId == item.Id)
            .ToListAsync();

        //md
        await httpContext.Response.WriteAsJsonAsync(new
        {
            content = item.Content,
            title = item.Title,
            fileSource,
            address = query?.Address.Replace(".git", string.Empty),
            query?.Branch,
        });
    }

    /// <summary>
    /// 递归构建文档目录树形结构。
    /// </summary>
    /// <param name="documents">所有文档目录列表</param>
    /// <returns>树形结构文档目录</returns>
    private List<object> BuildDocumentTree(List<DocumentCatalog> documents)
    {
        var result = new List<object>();

        // 获取顶级目录
        var topLevel = documents.Where(x => x.ParentId == null).OrderBy(x => x.Order).ToList();

        foreach (var item in topLevel)
        {
            var children = GetChildren(item.Id, documents);
            if (children == null || children.Count == 0)
            {
                result.Add(new
                {
                    label = item.Name,
                    item.Url,
                    item.Description,
                    key = item.Id
                });
            }
            else
            {
                result.Add(new
                {
                    label = item.Name,
                    item.Description,
                    item.Url,
                    key = item.Id,
                    children
                });
            }
        }

        return result;
    }

    /// <summary>
    /// 递归获取子目录。
    /// </summary>
    /// <param name="parentId">父目录 ID</param>
    /// <param name="documents">所有文档目录列表</param>
    /// <returns>子目录列表</returns>
    private List<object> GetChildren(string parentId, List<DocumentCatalog> documents)
    {
        var children = new List<object>();
        var directChildren = documents.Where(x => x.ParentId == parentId).OrderBy(x => x.Order).ToList();

        foreach (var child in directChildren)
        {
            // 递归获取子目录的子目录
            var subChildren = GetChildren(child.Id, documents);

            if (subChildren == null || subChildren.Count == 0)
            {
                children.Add(new
                {
                    label = child.Name,
                    child.Url,
                    key = child.Id,
                    child.Description
                });
            }
            else
            {
                children.Add(new
                {
                    label = child.Name,
                    key = child.Id,
                    child.Url,
                    child.Description,
                    children = subChildren
                });
            }
        }

        return children;
    }
}