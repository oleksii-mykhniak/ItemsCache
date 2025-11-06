namespace ItemsCache.Core.Abstraction.Interfaces;

public interface IItemsCacheServiceWithModifications<TKey, TCacheItem> : IItemsCacheService<TKey, TCacheItem>
    where TKey : notnull
{
    bool TryRefresh(IEnumerable<KeyValuePair<TKey, TCacheItem>> items);

    bool TrySet(TKey key, TCacheItem item);

    bool TryDelete(TKey key);
}