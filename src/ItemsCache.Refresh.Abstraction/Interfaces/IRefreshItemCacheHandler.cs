using ItemsCache.Refresh.Abstraction.Models;

namespace ItemsCache.Refresh.Abstraction.Interfaces;

public interface IRefreshItemCacheHandler<TKey, TCacheItem> where TKey : notnull
{
    RefreshCacheItemStatus Status { get; }

    Task<bool> HandleAsync(TKey key, TCacheItem? cacheItem, CancellationToken cancellationToken);
}