using FastService;
using KoalaWiki.Core.DataAccess;
using KoalaWiki.Domains;
using KoalaWiki.Dto;
using Microsoft.EntityFrameworkCore;

namespace KoalaWiki.Services;

public class ChatShareMessageService(IKoalaWikiContext koalaWikiContext) : FastApi
{
    public async Task<ResultDto<object>> GetListAsync(string chatShareMessageId, int page,
        int pageSize)
    {
        var chatMessage = koalaWikiContext.ChatShareMessages
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == chatShareMessageId);
        
        var list = await koalaWikiContext.ChatShareMessageItems
            .AsNoTracking()
            .Where(x => x.ChatShareMessageId == chatShareMessageId)
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var total = await koalaWikiContext.ChatShareMessageItems
            .AsNoTracking()
            .CountAsync(x => x.Id == chatShareMessageId);

        return ResultDto<object>.Success(new
        {
            items = list,
            total = total,
            info = chatMessage
        });
    }

    public async Task<ResultDto<string>> CreateAsync(ChatShareMessageInput input, HttpContext context)
    {
        // 获取ip
        var ip = context.Connection.RemoteIpAddress?.ToString();
        if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
        {
            ip = forwardedFor.ToString();
        }
        else if (context.Request.Headers.TryGetValue("X-Real-IP", out var realIp))
        {
            ip = realIp.ToString();
        }

        var warehouse = await koalaWikiContext.Warehouses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.OrganizationName == input.Owner && x.Name == input.Name);

        var chatShareMessage = new ChatShareMessage
        {
            Ip = ip,
            Id = Guid.NewGuid().ToString(),
            WarehouseId = warehouse.Id,
            IsDeep = input.IsDeep,
            Question = input.Message,
        };

        await koalaWikiContext.ChatShareMessages.AddAsync(chatShareMessage);

        await koalaWikiContext.SaveChangesAsync();

        return ResultDto<string>.Success(chatShareMessage.Id);
    }
}