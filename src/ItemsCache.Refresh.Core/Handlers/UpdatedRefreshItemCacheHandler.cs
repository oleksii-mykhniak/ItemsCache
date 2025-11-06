using ItemsCache.Core.Abstraction.Interfaces;
using ItemsCache.Refresh.Abstraction.Interfaces;
using ItemsCache.Refresh.Abstraction.Models;
using Microsoft.Extensions.Logging;

namespace ItemsCache.Refresh.Core.Handlers
{
    internal sealed class UpdatedRefreshItemCacheHandler<TKey, TCacheItem> : IRefreshItemCacheHandler<TKey, TCacheItem>
        where TKey : notnull
    {
        private readonly IItemsCacheServiceWithModifications<TKey, TCacheItem> _itemsCacheService;
        private readonly ILogger<UpdatedRefreshItemCacheHandler<TKey, TCacheItem>> _logger;

        public UpdatedRefreshItemCacheHandler(IItemsCacheServiceWithModifications<TKey, TCacheItem> itemsCacheService, ILogger<UpdatedRefreshItemCacheHandler<TKey, TCacheItem>> logger)
        {
            _itemsCacheService = itemsCacheService;
            _logger = logger;
        }

        public RefreshCacheItemStatus Status => RefreshCacheItemStatus.Updated;
        
        public Task<bool> HandleAsync(TKey key, TCacheItem? cacheItem, CancellationToken cancellationToken)
        {
            var result = _itemsCacheService.TrySet(key, cacheItem!);
            
            if (!result)
            {
                _logger.LogWarning("Refreshed item was modified concurrently {CacheItemKey} {CacheItem}", key, cacheItem);
            }
            
            return Task.FromResult(result);
        }
    }
}
