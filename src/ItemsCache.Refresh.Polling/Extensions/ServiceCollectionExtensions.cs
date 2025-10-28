using ItemsCache.Refresh.Core.Extensions;
using ItemsCache.Refresh.Polling.Abstraction;
using ItemsCache.Refresh.Polling.Abstraction.Models;
using ItemsCache.Refresh.Polling.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ItemsCache.Refresh.Polling.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPollingRefreshItemCacheHandler<TCacheItem, TKey, TRefreshContext>(this IServiceCollection serviceCollection, IConfiguration configuration)
            where TRefreshContext : class
            where TCacheItem : class
            where TKey : notnull
        {
            // Register refresh handlers
            serviceCollection.AddRefreshItemCacheHandlers<TCacheItem, TKey>();
            
            // Register polling-specific services
            serviceCollection.AddTransient<IItemsCachePollingRefresher, ItemsCachePollingRefresher<TCacheItem, TKey, TRefreshContext>>();
            serviceCollection.AddHostedService<ItemsCachePollingRefreshBackgroundService>();
            
            serviceCollection.Configure<ItemsCacheOptions>(configuration.GetSection("CacheOptions"));
            
            return serviceCollection;
        }
    }
}
