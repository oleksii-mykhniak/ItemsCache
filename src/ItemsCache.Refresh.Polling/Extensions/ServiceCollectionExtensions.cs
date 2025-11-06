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
        public static IServiceCollection AddPollingRefreshItemCacheHandler<TKey, TCacheItem, TRefreshContext>(this IServiceCollection serviceCollection, IConfiguration configuration)
            where TKey : notnull
            where TCacheItem : class
            where TRefreshContext : class
        {
            // Register refresh handlers
            serviceCollection.AddRefreshItemCacheHandlers<TKey, TCacheItem>();
            
            // Register polling-specific services
            serviceCollection.AddTransient<IItemsCachePollingRefresher, ItemsCachePollingRefresher<TKey, TCacheItem, TRefreshContext>>();
            serviceCollection.AddHostedService<ItemsCachePollingRefreshBackgroundService>();
            
            serviceCollection.Configure<ItemsCacheOptions>(configuration.GetSection("CacheOptions"));
            
            return serviceCollection;
        }
    }
}
