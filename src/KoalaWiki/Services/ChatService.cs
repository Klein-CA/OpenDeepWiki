using System.Diagnostics;
using System.Text;
using System.Text.Json;
using FastService;
using KoalaWiki.Core.DataAccess;
using KoalaWiki.Domains;
using KoalaWiki.Dto;
using KoalaWiki.KoalaWarehouse;
using KoalaWiki.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using OpenAI.Chat;

#pragma warning disable SKEXP0001

namespace KoalaWiki.Services;

public class ChatService(IKoalaWikiContext koala) : FastApi
{
    public async Task CompletionsAsync(CompletionsInput input, HttpContext context)
    {
        var chatShareMessage = await koala.ChatShareMessages
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == input.ChatShareMessageId);

        var warehouse = await koala.Warehouses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == chatShareMessage.WarehouseId);

        // 是否第一次对话
        var isFirst =
            await koala.ChatShareMessageItems.AnyAsync(x => x.ChatShareMessageId == input.ChatShareMessageId) == false;

        var document = await koala.Documents
            .AsNoTracking()
            .Where(x => x.WarehouseId == warehouse.Id)
            .FirstOrDefaultAsync();

        if (document == null)
        {
            throw new Exception("空异常 Document");
        }

        var kernel = KernelFactory.GetKernel(OpenAIOptions.Endpoint,
            OpenAIOptions.ChatApiKey, document.GitPath, OpenAIOptions.ChatModel, false);

        var chat = kernel.GetRequiredService<IChatCompletionService>();

        // 解析仓库的目录结构
        var path = document.GitPath;

        var fileKernel = KernelFactory.GetKernel(OpenAIOptions.Endpoint,
            OpenAIOptions.ChatApiKey, path, OpenAIOptions.ChatModel, false);

        var history = new ChatHistory();

        if (isFirst)
        {
            input.Question = chatShareMessage!.Question;
        }
        else
        {
            throw new Exception("未实现多次对话。");
        }

        var conversationHistory = new StringBuilder();
        foreach (var message in input.Messages)
        {
            conversationHistory.Append($"{message.Role}: {message.Content}\n");
        }

        var fileSource = DocumentsService.GetCatalogue(document.GitPath);

        if (chatShareMessage.IsDeep)
        {
            if (isFirst)
            {
                history.AddUserMessage(Prompt.DeepFirstPrompt
                    .Replace("{{$question}}", chatShareMessage!.Question)
                    .Replace("{{$git_repository_url}}", warehouse.Address)
                    .Replace("{{$catalogue}}", string.Join('\n', fileSource)));
            }
            else
            {
                history.AddUserMessage(Prompt.HistoryPrompt
                    .Replace("{{$question}}", input.Question)
                    .Replace("{{$git_repository_url}}", warehouse.Address)
                    .Replace("{{$conversation_history}}", conversationHistory.ToString())
                    .Replace("{{$catalogue}}", string.Join('\n', fileSource)));
            }
        }
        else
        {
            if (isFirst)
            {
                history.AddUserMessage(Prompt.FirstPrompt
                    .Replace("{{$question}}", chatShareMessage!.Question)
                    .Replace("{{$git_repository_url}}", warehouse.Address)
                    .Replace("{{$catalogue}}", string.Join('\n', fileSource)));
            }
            else
            {
                history.AddUserMessage(Prompt.HistoryPrompt
                    .Replace("{{$question}}", input.Question)
                    .Replace("{{$git_repository_url}}", warehouse.Address)
                    .Replace("{{$conversation_history}}", conversationHistory.ToString())
                    .Replace("{{$catalogue}}", string.Join('\n', fileSource)));
            }
        }


        var first = true;

        DocumentContext.DocumentStore = new DocumentStore();
        var items = new ChatShareMessageItem
        {
            Question = input.Question,
            Answer = string.Empty,
            CreatedAt = DateTime.Now,
            WarehouseId = warehouse.Id,
            ChatShareMessageId = input.ChatShareMessageId,
            Id = Guid.NewGuid().ToString(),
        };
        await koala.ChatShareMessageItems.AddAsync(items);

        await koala.SaveChangesAsync();

        var sw = Stopwatch.StartNew();

        int requestToken = 0;
        int completionToken = 0;
        var answer = new StringBuilder();

        var files = new List<string>();

        try
        {
            await foreach (var chatItem in chat.GetStreamingChatMessageContentsAsync(history,
                               new OpenAIPromptExecutionSettings()
                               {
                                   ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
                                   MaxTokens = DocumentsService.GetMaxTokens(OpenAIOptions.ChatModel),
                                   Temperature = 0.5,
                               }, fileKernel))
            {
                if (first)
                {
                    // sse
                    context.Response.Headers.ContentType = "text/event-stream";
                    context.Response.Headers.CacheControl = "no-cache";
                    first = false;
                }

                // 发送数据
                if (chatItem.InnerContent is StreamingChatCompletionUpdate message)
                {
                    if (message.Usage is { InputTokenCount: > 0 })
                    {
                        requestToken += message.Usage.InputTokenCount;
                        completionToken += message.Usage.OutputTokenCount;
                    }

                    if (DocumentContext.DocumentStore.Files.Count > 0)
                    {
                        await context.Response.WriteAsync($"data: {JsonSerializer.Serialize(new
                        {
                            type = "tool",
                            content = DocumentContext.DocumentStore.Files.Distinct(),
                        }, JsonSerializerOptions.Web)}\n\n");
                        files.AddRange(DocumentContext.DocumentStore.Files.Distinct());
                        await context.Response.Body.FlushAsync();
                        DocumentContext.DocumentStore.Files = [];
                        continue;
                    }

                    if (string.IsNullOrEmpty(chatItem.Content))
                    {
                        continue;
                    }

                    answer.Append(chatItem.Content);

                    await context.Response.WriteAsync($"data: {JsonSerializer.Serialize(new
                    {
                        type = "message",
                        content = chatItem.Content,
                    }, JsonSerializerOptions.Web)}\n\n");
                    await context.Response.Body.FlushAsync();
                }
            }

            sw.Stop();

            files = files.Distinct().ToList();
            await koala.ChatShareMessageItems.Where(x => x.Id == items.Id)
                .ExecuteUpdateAsync(x => x.SetProperty(a => a.Answer, answer.ToString())
                    .SetProperty(a => a.PromptToken, requestToken)
                    .SetProperty(a => a.CompletionToken, completionToken)
                    .SetProperty(a => a.Files, files)
                    .SetProperty(a => a.TotalTime, sw.ElapsedMilliseconds));
        }
        catch (Exception e)
        {
            // 删除添加的数据
            await koala.ChatShareMessageItems
                .Where(x => x.Id == items.Id)
                .ExecuteDeleteAsync();
        }

        await context.Response.WriteAsync($"data: [done]\n\n");
    }
}