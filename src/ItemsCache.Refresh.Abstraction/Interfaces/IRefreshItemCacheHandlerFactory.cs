using ItemsCache.Refresh.Abstraction.Models;

namespace ItemsCache.Refresh.Abstraction.Interfaces;

public interface IRefreshItemCacheHandlerFactory<TKey, TCacheItem>
    where TKey : notnull
{
    IRefreshItemCacheHandler<TKey, TCacheItem> Create(RefreshCacheItemStatus status);
}