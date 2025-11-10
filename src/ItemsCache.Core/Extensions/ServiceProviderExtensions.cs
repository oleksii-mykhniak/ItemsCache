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
    
    public static IServiceCollection AddItemsCacheGrouped<TKey, TCacheItem, TGroupKey>(
        this IServiceCollection serviceCollection,
        Func<TCacheItem, TGroupKey> keySelector)
        where TKey : notnull
        where TCacheItem : class
        where TGroupKey : notnull
    {
        if (keySelector == null)
            throw new ArgumentNullException(nameof(keySelector));

        serviceCollection.AddSingleton<IItemsCacheGroupedService<TCacheItem, TGroupKey>>(sp =>
        {
            var logger = sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<ItemsCacheGroupedService<TKey, TCacheItem, TGroupKey>>>();
            var groupedService = new ItemsCacheGroupedService<TKey, TCacheItem, TGroupKey>(keySelector, logger);
            
            var cacheService = sp.GetRequiredService<IItemsCacheServiceWithModifications<TKey, TCacheItem>>();
            if (cacheService is ItemsCacheService<TKey, TCacheItem> concreteCache)
            {
                concreteCache.RegisterObserver(groupedService);
            }
            
            return groupedService;
        });

        return serviceCollection;
    }
}