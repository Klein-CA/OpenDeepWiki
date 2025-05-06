namespace KoalaWiki.Memory;

public static class MemoryExtensions
{
    public static IServiceCollection AddKoalaMemory(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHostedService<MemoryTask>();

        return services;
    }
}