using ItemsCache.Core.Abstraction.Interfaces;
using ItemsCache.Core.Interfaces;
using ItemsCache.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ItemsCache.Core.Extensions;

public static class ServiceProviderExtensions
{
    public static IServiceCollection AddItemsCache<TKey, TCacheItem>(this IServiceCollection serviceCollection)
        where TKey : notnull
        where TCacheItem : class
    {
        var cache = new ItemsCacheService<TKey, TCacheItem>();
        
        serviceCollection.AddSingleton<IItemsCacheService<TKey, TCacheItem>>(cache);
        serviceCollection.AddSingleton<IItemsCacheServiceWithModifications<TKey, TCacheItem>>(cache);
        
        serviceCollection.AddTransient<IItemsCacheLoader, ItemsCacheLoader<TKey, TCacheItem>>();
        
        serviceCollection.TryAddScoped<IItemsCacheInitService, ItemsCacheInitService>();
        serviceCollection.AddHostedService<CacheInitHostedService>();
            
        return serviceCollection;
    }
}