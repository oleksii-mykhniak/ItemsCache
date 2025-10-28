using ItemsCache.Core.Abstraction.Interfaces;
using ItemsCache.Core.Interfaces;
using ItemsCache.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ItemsCache.Core.Extensions;

public static class ServiceProviderExtensions
{
    public static IServiceCollection AddItemsCache<TCacheItem, TKey>(this IServiceCollection serviceCollection)
        where TCacheItem : class
        where TKey : notnull
    {
        var cache = new ItemsCacheService<TCacheItem, TKey>();
        
        serviceCollection.AddSingleton<IItemsCacheService<TCacheItem, TKey>>(cache);
        serviceCollection.AddSingleton<IItemsCacheServiceWithModifications<TCacheItem, TKey>>(cache);
        
        serviceCollection.AddTransient<IItemsCacheLoader, ItemsCacheLoader<TCacheItem, TKey>>();
        
        serviceCollection.TryAddScoped<IItemsCacheInitService, ItemsCacheInitService>();
        serviceCollection.AddHostedService<CacheInitHostedService>();
            
        return serviceCollection;
    }
}