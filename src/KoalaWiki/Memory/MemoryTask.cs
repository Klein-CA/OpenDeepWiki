namespace KoalaWiki.Memory;

public class MemoryTask : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!stoppingToken.IsCancellationRequested)
        {
            
        }
    }
}