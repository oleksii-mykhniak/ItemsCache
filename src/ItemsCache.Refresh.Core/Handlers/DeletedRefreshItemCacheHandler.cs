using ItemsCache.Core.Abstraction.Interfaces;
using ItemsCache.Refresh.Abstraction.Interfaces;
using ItemsCache.Refresh.Abstraction.Models;
using Microsoft.Extensions.Logging;

namespace ItemsCache.Refresh.Core.Handlers
{
    internal sealed class DeletedRefreshItemCacheHandler<TKey, TCacheItem> : IRefreshItemCacheHandler<TKey, TCacheItem> where TKey : notnull
    {
        private readonly IItemsCacheServiceWithModifications<TKey, TCacheItem> _itemsCacheService;
        private readonly ILogger<DeletedRefreshItemCacheHandler<TKey, TCacheItem>> _logger;

        public DeletedRefreshItemCacheHandler(IItemsCacheServiceWithModifications<TKey, TCacheItem> itemsCacheService, ILogger<DeletedRefreshItemCacheHandler<TKey, TCacheItem>> logger)
        {
            _itemsCacheService = itemsCacheService;
            _logger = logger;
        }

        public RefreshCacheItemStatus Status => RefreshCacheItemStatus.Deleted;

        public Task<bool> HandleAsync(TKey key, TCacheItem? cacheItem, CancellationToken cancellationToken)
        {
            var result = _itemsCacheService.TryDelete(key);

            if (!result)
            {
                _logger.LogWarning("Refreshed item was modified concurrently before deletion {CacheItemKey}", key);
            }

            return Task.FromResult(result);
        }
    }
}
