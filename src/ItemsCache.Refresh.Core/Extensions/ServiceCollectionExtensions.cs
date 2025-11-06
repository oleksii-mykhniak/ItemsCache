using ItemsCache.Refresh.Abstraction.Interfaces;
using ItemsCache.Refresh.Core.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace ItemsCache.Refresh.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRefreshItemCacheHandlers<TKey, TCacheItem>(this IServiceCollection serviceCollection)
            where TKey : notnull
            where TCacheItem : class
        {
            serviceCollection.AddTransient<IRefreshItemCacheHandler<TKey, TCacheItem>, UpdatedRefreshItemCacheHandler<TKey, TCacheItem>>();
            serviceCollection.AddTransient<IRefreshItemCacheHandler<TKey, TCacheItem>, DeletedRefreshItemCacheHandler<TKey, TCacheItem>>();
            serviceCollection.AddTransient<IRefreshItemCacheHandlerFactory<TKey, TCacheItem>, RefreshItemCacheHandlerFactory<TKey, TCacheItem>>();
            
            return serviceCollection;
        }
    }
}
