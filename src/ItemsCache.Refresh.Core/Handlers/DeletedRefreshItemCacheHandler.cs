using ItemsCache.Core.Abstraction.Interfaces;
using ItemsCache.Refresh.Abstraction.Interfaces;
using ItemsCache.Refresh.Abstraction.Models;
using Microsoft.Extensions.Logging;

namespace ItemsCache.Refresh.Core.Handlers
{
    internal sealed class DeletedRefreshItemCacheHandler<TCacheItem, TKey> : IRefreshItemCacheHandler<TCacheItem, TKey> where TKey : notnull
    {
        private readonly IItemsCacheServiceWithModifications<TCacheItem, TKey> _itemsCacheService;
        private readonly ILogger<DeletedRefreshItemCacheHandler<TCacheItem, TKey>> _logger;

        public DeletedRefreshItemCacheHandler(IItemsCacheServiceWithModifications<TCacheItem, TKey> itemsCacheService, ILogger<DeletedRefreshItemCacheHandler<TCacheItem, TKey>> logger)
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
