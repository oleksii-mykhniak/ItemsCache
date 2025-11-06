using ItemsCache.Refresh.Abstraction.Models;

namespace ItemsCache.Refresh.Abstraction.Interfaces;

public interface IRefreshItemCacheHandlerFactory<TCacheItem, TKey>
    where TKey : notnull
{
    IRefreshItemCacheHandler<TCacheItem, TKey> Create(RefreshCacheItemStatus status);
}