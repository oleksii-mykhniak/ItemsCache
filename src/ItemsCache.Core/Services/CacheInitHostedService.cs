using ItemsCache.Core.Abstraction.Exceptions;
using ItemsCache.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ItemsCache.Core.Services;

internal sealed class CacheInitHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public CacheInitHostedService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var itemsCacheInitService = scope.ServiceProvider.GetRequiredService<IItemsCacheInitService>();
            
        var initialized = await itemsCacheInitService.TryInitCacheAsync(cancellationToken);
        
        if (!initialized)
            throw new CacheItemException("Failed to initialize items cache");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}