using ItemsCache.Refresh.Abstraction.Interfaces;
using ItemsCache.Refresh.Core.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace ItemsCache.Refresh.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRefreshItemCacheHandlers<TCacheItem, TKey>(this IServiceCollection serviceCollection)
            where TCacheItem : class
            where TKey : notnull
        {
            serviceCollection.AddTransient<IRefreshItemCacheHandler<TCacheItem, TKey>, UpdatedRefreshItemCacheHandler<TCacheItem, TKey>>();
            serviceCollection.AddTransient<IRefreshItemCacheHandler<TCacheItem, TKey>, DeletedRefreshItemCacheHandler<TCacheItem, TKey>>();
            serviceCollection.AddTransient<IRefreshItemCacheHandlerFactory<TCacheItem, TKey>, RefreshItemCacheHandlerFactory<TCacheItem, TKey>>();
            
            return serviceCollection;
        }
    }
}
