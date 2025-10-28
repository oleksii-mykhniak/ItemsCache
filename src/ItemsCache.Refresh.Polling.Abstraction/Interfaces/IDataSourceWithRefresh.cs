using ItemsCache.Refresh.Polling.Abstraction.Models;

namespace ItemsCache.Refresh.Polling.Abstraction.Interfaces;

public interface IDataSourceWithRefresh<TCacheItem, TKey, TRefreshContext>
    where TCacheItem : class
    where TKey : notnull
{
    Task<CacheItemRefreshResult<TCacheItem, TKey, TRefreshContext>> GetUpdatedItemsAsync(TRefreshContext? lastRefreshContext, CancellationToken cancellationToken = default);
}
