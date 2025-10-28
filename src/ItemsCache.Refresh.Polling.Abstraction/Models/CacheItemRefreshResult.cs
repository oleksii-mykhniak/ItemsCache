using ItemsCache.Refresh.Abstraction.Models;

namespace ItemsCache.Refresh.Polling.Abstraction.Models;

public class CacheItemRefreshResult<TCacheItem, TKey, TRefreshContext>
    where TCacheItem : class
    where TKey : notnull
{
    public CacheItemRefreshResult(IEnumerable<RefreshCacheItem<TCacheItem, TKey>> items, TRefreshContext? newRefreshContext)
    {
        Items = items;
        NewRefreshContext = newRefreshContext;
    }

    public IEnumerable<RefreshCacheItem<TCacheItem, TKey>> Items { get; }
    
    public TRefreshContext? NewRefreshContext { get; }
}
