namespace ItemsCache.Core.Abstraction.Interfaces;

public interface ICacheUpdateObserver<TKey, TCacheItem>
    where TKey : notnull
{
    void OnCacheRefreshed(IEnumerable<KeyValuePair<TKey, TCacheItem>> items);
    
    void OnItemUpdated(TKey key, TCacheItem item);
    
    void OnItemDeleted(TKey key);
}
