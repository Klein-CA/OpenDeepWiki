using FastService;
using KoalaWiki.Core.DataAccess;
using KoalaWiki.Dto;
using KoalaWiki.Entities;
using KoalaWiki.Functions;
using KoalaWiki.KoalaWarehouse;
using LibGit2Sharp;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace KoalaWiki.Services;

/// <summary>
/// WarehouseService 类用于处理与仓库相关的业务逻辑。
/// 该类提供了仓库的查询、提交、获取变更日志、获取仓库概述以及获取仓库列表等功能。
/// </summary>
public class WarehouseService(IKoalaWikiContext access, IMapper mapper, WarehouseStore warehouseStore) : FastApi
{
    /// <summary>
    /// 查询上次提交的仓库信息。
    /// </summary>
    /// <param name="address">仓库地址</param>
    /// <returns>包含仓库信息的匿名对象</returns>
    public async Task<object> GetLastWarehouseAsync(string address)
    {
        // 判断是否.git结束，如果不是需要添加
        if (!address.EndsWith(".git"))
        {
            address += ".git";
        }

        var query = await access.Warehouses
            .AsNoTracking()
            .Where(x => x.Address == address)
            .FirstOrDefaultAsync() ?? throw new NotFoundException("仓库不存在");

        return new
        {
            query.Name,
            query.Address,
            query.Description,
            query.Version,
            query.Status,
            query.Error
        };
    }

    /// <summary>
    /// 获取指定仓库的变更日志。
    /// </summary>
    /// <param name="owner">仓库所有者</param>
    /// <param name="name">仓库名称</param>
    /// <returns>文档提交记录实体</returns>
    public async Task<DocumentCommitRecord?> GetChangeLogAsync(string owner, string name)
    {
        var warehouse = await access.Warehouses
            .AsNoTracking()
            .Where(x => x.Name == name && x.OrganizationName == owner)
            .FirstOrDefaultAsync() ?? throw new NotFoundException("仓库不存在");
        var commit = await access.DocumentCommitRecords.FirstOrDefaultAsync(x => x.WarehouseId == warehouse.Id);

        return commit;
    }

    /// <summary>
    /// 提交仓库信息。
    /// </summary>
    /// <param name="input">仓库输入信息</param>
    /// <param name="context">HTTP 上下文</param>
    public async Task SubmitWarehouseAsync(WarehouseInput input, HttpContext context)
    {
        try
        {
            if (!input.Address.EndsWith(".git"))
            {
                input.Address += ".git";
            }

            var value = await access.Warehouses.FirstOrDefaultAsync(x => x.Address == input.Address);
            // 判断这个仓库是否已经添加
            if (value?.Status is WarehouseStatus.Completed or WarehouseStatus.Pending or WarehouseStatus.Processing)
            {
                throw new Exception("存在相同名称的渠道");
            }

            // 删除旧的仓库
            var oldWarehouse = await access.Warehouses
                .Where(x => x.Address == input.Address)
                .ExecuteDeleteAsync();

            var entity = mapper.Map<Warehouse>(input);
            entity.Name = string.Empty;
            entity.Description = string.Empty;
            entity.Version = string.Empty;
            entity.Error = string.Empty;
            entity.Prompt = string.Empty;
            entity.Branch = string.Empty;
            entity.Type = "git";
            entity.CreatedAt = DateTime.UtcNow;

            entity.Id = Guid.NewGuid().ToString();
            await access.Warehouses.AddAsync(entity);
            await access.SaveChangesAsync();

            await warehouseStore.WriteAsync(entity);

            await context.Response.WriteAsJsonAsync(new
            {
                code = 200,
                message = "提交成功"
            });
        }
        catch (Exception e)
        {
            await context.Response.WriteAsJsonAsync(new
            {
                code = 500,
                message = e.Message
            });
        }
    }

    /// <summary>
    /// 获取指定仓库的概述信息。
    /// </summary>
    /// <param name="owner">仓库所有者</param>
    /// <param name="name">仓库名称</param>
    /// <param name="context">HTTP 上下文</param>
    public async Task GetWarehouseOverviewAsync(string owner, string name, HttpContext context)
    {
        var query = await access.Warehouses
            .AsNoTracking()
            .Where(x => x.Name == name && x.OrganizationName == owner)
            .FirstOrDefaultAsync() ?? throw new NotFoundException("仓库不存在");

        var document = await access.Documents
            .AsNoTracking()
            .Where(x => x.WarehouseId == query.Id)
            .FirstOrDefaultAsync();

        var overview = await access.DocumentOverviews.FirstOrDefaultAsync(x => x.DocumentId == document.Id) ?? throw new NotFoundException("没有找到概述");

        await context.Response.WriteAsJsonAsync(new
        {
            content = overview.Content,
            title = overview.Title
        });
    }

    /// <summary>
    /// 获取仓库列表，支持分页查询。
    /// </summary>
    /// <param name="page">当前页码</param>
    /// <param name="pageSize">每页大小</param>
    /// <returns>包含仓库列表的分页数据</returns>
    public async Task<PageDto<Warehouse>> GetWarehouseListAsync(int page, int pageSize)
    {
        var query = access.Warehouses
            .AsNoTracking()
            .Where(x => x.Status == WarehouseStatus.Completed);

        var total = await query.CountAsync();
        var list = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PageDto<Warehouse>(total, list);
    }

    [EndpointSummary("获取指定仓库代码文件")]
    public async Task<ResultDto<string>> GetFileContent(string warehouseId, string path)
    {
        var query = await access.Documents
            .AsNoTracking()
            .Where(x => x.WarehouseId == warehouseId)
            .FirstOrDefaultAsync();

        if (query == null)
        {
            throw new NotFoundException("文件不存在");
        }

        var fileFunction = new FileFunction(query.GitPath);

        var result = await fileFunction.ReadFileAsync(path);

        return ResultDto<string>.Success(result);
    }
}