using KoalaWiki.Functions;
using Microsoft.SemanticKernel;
using Serilog;

#pragma warning disable SKEXP0010

namespace KoalaWiki;

/// <summary>
/// KernelFactory 类用于创建和配置 Semantic Kernel 实例。
/// 该类提供了构建和配置 Kernel 的方法，支持添加 OpenAI 聊天完成功能、代码分析插件以及文件函数。
/// </summary>
public class KernelFactory
{
    public static Kernel GetKernel(string embeddingEndpoint,
        string embeddingApiKey,
        string embeddingModel)
    {
        var kernelBuilder = Kernel.CreateBuilder();

        kernelBuilder.Services.AddSerilog(Log.Logger);

        kernelBuilder.AddOpenAIChatCompletion(embeddingModel, new Uri(embeddingEndpoint), embeddingApiKey,
            httpClient: new HttpClient(new KoalaHttpClientHandler()
            {
                AllowAutoRedirect = true,
                MaxAutomaticRedirections = 5,
                MaxConnectionsPerServer = 200,
            })
            {
                Timeout = TimeSpan.FromSeconds(16000),
            });

        var kernel = kernelBuilder.Build();

        return kernel;
    }

    /// <summary>
    /// 获取配置好的 Semantic Kernel 实例。
    /// </summary>
    /// <param name="chatEndpoint">OpenAI 聊天服务的终结点</param>
    /// <param name="embeddingApiKey">OpenAI 聊天服务的 API 密钥</param>
    /// <param name="gitPath">Git 仓库路径</param>
    /// <param name="model">使用的模型名称，默认为 "gpt-4.1"</param>
    /// <param name="isCodeAnalysis">是否启用代码分析插件，默认为 true</param>
    /// <returns>配置好的 Semantic Kernel 实例</returns>
    public static Kernel GetKernel(string chatEndpoint,
        string embeddingApiKey,
        string gitPath,
        string model = "gpt-4.1", bool isCodeAnalysis = true)
    {
        var kernelBuilder = Kernel.CreateBuilder();

        // 添加 Serilog 日志服务
        kernelBuilder.Services.AddSerilog(Log.Logger);

        kernelBuilder.AddOpenAIChatCompletion(model, new Uri(chatEndpoint), embeddingApiKey,
            httpClient: new HttpClient(new KoalaHttpClientHandler()
            {
                // 添加重试配置
                AllowAutoRedirect = true,
                MaxAutomaticRedirections = 5,
                MaxConnectionsPerServer = 200,
            })
            {
                // 设置超时时间
                Timeout = TimeSpan.FromSeconds(16000),
            });

        // 如果启用代码分析，添加代码分析插件
        if (isCodeAnalysis)
        {
            kernelBuilder.Plugins.AddFromPromptDirectory(Path.Combine(AppContext.BaseDirectory, "plugins",
                "CodeAnalysis"));
        }

        // 添加文件函数
        var fileFunction = new FileFunction(gitPath);
        kernelBuilder.Plugins.AddFromObject(fileFunction);

        // 构建并返回 Kernel 实例
        var kernel = kernelBuilder.Build();

        return kernel;
    }
}