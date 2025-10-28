using ItemsCache.Core.Abstraction.Interfaces;
using ItemsCache.Refresh.Abstraction.Interfaces;
using ItemsCache.Refresh.Abstraction.Models;
using Microsoft.Extensions.Logging;

namespace ItemsCache.Refresh.Core.Handlers
{
    internal sealed class UpdatedRefreshItemCacheHandler<TCacheItem, TKey> : IRefreshItemCacheHandler<TCacheItem, TKey>
        where TKey : notnull
    {
        private readonly IItemsCacheServiceWithModifications<TCacheItem, TKey> _itemsCacheService;
        private readonly ILogger<UpdatedRefreshItemCacheHandler<TCacheItem, TKey>> _logger;

        public UpdatedRefreshItemCacheHandler(IItemsCacheServiceWithModifications<TCacheItem, TKey> itemsCacheService, ILogger<UpdatedRefreshItemCacheHandler<TCacheItem, TKey>> logger)
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
