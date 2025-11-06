using ItemsCache.Refresh.Polling.Abstraction.Models;

namespace ItemsCache.Refresh.Polling.Abstraction.Interfaces;

public interface IDataSourceWithRefresh<TKey, TCacheItem, TRefreshContext>
    where TKey : notnull
    where TCacheItem : class
{
    Task<CacheItemRefreshResult<TKey, TCacheItem, TRefreshContext>> GetUpdatedItemsAsync(TRefreshContext? lastRefreshContext, CancellationToken cancellationToken = default);
}
