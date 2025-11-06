using ItemsCache.Refresh.Abstraction.Models;

namespace ItemsCache.Refresh.Polling.Abstraction.Models;

public class CacheItemRefreshResult<TKey, TCacheItem, TRefreshContext>
    where TKey : notnull
    where TCacheItem : class
{
    public CacheItemRefreshResult(IEnumerable<RefreshCacheItem<TKey, TCacheItem>> items, TRefreshContext? newRefreshContext)
    {
        Items = items;
        NewRefreshContext = newRefreshContext;
    }

    public IEnumerable<RefreshCacheItem<TKey, TCacheItem>> Items { get; }
    
    public TRefreshContext? NewRefreshContext { get; }
}
