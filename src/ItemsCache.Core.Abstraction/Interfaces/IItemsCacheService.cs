namespace ItemsCache.Core.Abstraction.Interfaces
{
    public interface IItemsCacheService<TCacheItem, TKey> where TKey : notnull
    {
        int Count { get; }

        bool TryGetByKey(TKey key, out TCacheItem? item);

        IEnumerable<KeyValuePair<TKey, TCacheItem>> GetAll();
    }
}
