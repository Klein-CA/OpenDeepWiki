using System.Diagnostics;
using System.Text.Json;
using Newtonsoft.Json;
using Serilog;

namespace KoalaWiki;

/// <summary>
/// KoalaHttpClientHandler 类是一个自定义的 HTTP 客户端处理程序，继承自 HttpClientHandler。
/// 该类用于在发送 HTTP 请求时记录详细的日志信息，包括请求方法、URI、响应状态码以及请求耗时。
/// </summary>
public sealed class KoalaHttpClientHandler : HttpClientHandler
{
    /// <summary>
    /// 发送 HTTP 请求并记录日志。
    /// </summary>
    /// <param name="request">HTTP 请求消息</param>
    /// <param name="cancellationToken">取消操作的令牌</param>
    /// <returns>HTTP 响应消息</returns>
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // 记录请求开始日志
        Log.Logger.Information("HTTP {Method} {Uri}", request.Method, request.RequestUri);

        var json = JsonConvert.DeserializeObject<dynamic>(await request.Content.ReadAsStringAsync());
        // 增加max_token，从max_completion_tokens读取
        if (json != null && json.max_completion_tokens != null)
        {
            var maxToken = json.max_completion_tokens;
            if (maxToken != null)
            {
                json.max_tokens = maxToken;
                json.max_completion_tokens = null;
            }
            
            // 重写请求体
            request.Content = new StringContent(JsonConvert.SerializeObject(json),
                System.Text.Encoding.UTF8, "application/json");
        }

        // 1. 启动计时
        var stopwatch = Stopwatch.StartNew();
        // 2. 发送请求
        var response = await base.SendAsync(request, cancellationToken)
            .ConfigureAwait(false);
        // 3. 停止计时
        stopwatch.Stop();
        // 4. 记录请求完成日志
        Log.Logger.Information(
            "HTTP {Method} {Uri} => {StatusCode} in {ElapsedMilliseconds}ms",
            request.Method,
            request.RequestUri,
            (int)response.StatusCode,
            stopwatch.ElapsedMilliseconds
        );
        return response;
    }
}